// page visible/hidden logic
(function () {
    var hidden = "hidden";

    // Standards:
    if (hidden in document)
        document.addEventListener("visibilitychange", onchange);
    else if ((hidden = "mozHidden") in document)
        document.addEventListener("mozvisibilitychange", onchange);
    else if ((hidden = "webkitHidden") in document)
        document.addEventListener("webkitvisibilitychange", onchange);
    else if ((hidden = "msHidden") in document)
        document.addEventListener("msvisibilitychange", onchange);
        // IE 9 and lower:
    else if ('onfocusin' in document)
        document.onfocusin = document.onfocusout = onchange;
        // All others:
    else
        window.onpageshow = window.onpagehide
            = window.onfocus = window.onblur = onchange;

    oldTitle = document.title;

    function onchange(evt) {
        var v = 'visible', h = 'hidden',
            evtMap = {
                focus: v, focusin: v, pageshow: v, blur: h, focusout: h, pagehide: h
            };

        evt = evt || window.event;
        if (evt.type in evtMap)
            document.body.className = evtMap[evt.type];
        else
            document.body.className = this[hidden] ? "hidden" : "visible";
        
        if (document.body.className == 'visible')
            resetTitle();
    }
})();

// tab notification logic
var oldTitle, notificationCount = 0, firstTime = true;
function updateTitle() {
    if (firstTime)
        firstTime = false;

    notificationCount++;
    document.title = '(' + notificationCount + ') ' + oldTitle;
    (new Audio('/Content/Sound/new-message-notification.mp3')).play();
};
function resetTitle() {
    firstTime = true;
    document.title = oldTitle;
    notificationCount = 0;
}

function setSelectionRange(input, selectionStart, selectionEnd) {
    if (input.setSelectionRange) {
        input.focus();
        input.setSelectionRange(selectionStart, selectionEnd);
    }
    else if (input.createTextRange) {
        var range = input.createTextRange();
        range.collapse(true);
        range.moveEnd('character', selectionEnd);
        range.moveStart('character', selectionStart);
        range.select();
    }
}
function setCaretToPos(input, pos) {    
    setSelectionRange(input, pos, pos);
}

$(function () {
    var chat            = $.connection.chatHub;
    var $displayName    = $('#displayName');
    var $chatDisplay    = $('#chat');    
    var $message        = $('#message');
    var $sendMessage    = $('#sendMessage');
    var $codeMessage    = $('#sendCodeMessage');
    var $onlineUserList = $('#online-user-list');
    var codeMessage     = false;

    // set height of the main chat div on initial load and any window resize
    $chatDisplay.height(($(window).height() - 200) + 'px');    
    $(window).on('resize', function () {
        $chatDisplay.height(($(window).height() - 200) + 'px');
    });

    // set up that roomId of $chatDisplay
    var roomId = window.location.pathname.split('/')[3]

    // get the display name
    // TODO: Replacement code will be needed when accounts are implemented, just replace userName
    while (!$displayName.val()) {
        $displayName.val(prompt('Enter your name:', '').trim().replace(' ',''));
    };
    var userName = $displayName.val();
    
    // connection start and store user as online
    $.connection.hub.qs = "userName=" + userName + "&roomId=" + roomId;
    $.connection.hub.start().done(function () {
        // no implementation yet
    });


    ///////////////////////////////////////////////////////////////// S2C FUNCTIONS
    chat.client.userConnected = function (name, messageId) {
        // notify room they joined        
        var chatRoomMessage = encodeMessageMaster("room", name + ' has joined the room', false, false, messageId);
        appendMessage("room", chatRoomMessage, '0', messageId);
    };
    chat.client.userDisconnected = function (name, messageId) {
        // remove from online-user-list
        var $div = $('#online-user-list .online-user.' + name);
        $div.remove();

        // notify room they left
        var chatRoomMessage = encodeMessageMaster("room", name + ' has left the building', false, false, messageId);
        appendMessage("room", chatRoomMessage, 0, messageId);
    };
    chat.client.addNewMessageToPane = function (name, message, pm, isCode, senderConnectionId, messageId) {
        // get the content of what we want to display and pass it to appendMessage
        // appendMessage() will decide where it needs to be displayed
        var chatRoomMessage = encodeMessageMaster(name, message, pm, isCode, messageId);
        appendMessage(name, chatRoomMessage, senderConnectionId, messageId, isCode);
    };
    chat.client.updateOnlineUsers = function (users) {
        $.each($.parseJSON(users), function () {            
            if (!($('#online-user-list .online-user.' + this.UserName).length)) {                
                var onlineUserContent = htmlEncodeOnlineUser(this.UserName, this.ConnectionId);
                $onlineUserList.append(onlineUserContent);
            }
        });
    };

    // add desired message to the chat pane
    function appendMessage(name, message, senderConnectionId, messageId, isCode) {
        // determine if we need to append it to the last `messages` group or not
        var appendToLast = isLastMatched(senderConnectionId);
        
        if (($chatDisplay[0].scrollHeight - $chatDisplay.scrollTop()) == $chatDisplay.outerHeight()) {
            if (appendToLast)
                $('#chat :last-child').children('.messages').append(message);
            else
                $chatDisplay.append($('<div class="chat-room-message-wrapper ' + senderConnectionId + '"><div class="chat-room-message-name">' + name + '</div><div class="messages">' + message + '</div><div class="clear"></div></div>'));
            
            $chatDisplay.animate({ scrollTop: $chatDisplay[0].scrollHeight }, 500);
        } else {
            if (appendToLast)
                $('#chat :last-child').children('.messages').append(message);
            else
                $chatDisplay.append($('<div class="chat-room-message-wrapper ' + senderConnectionId + '"><div class="chat-room-message-name">' + name + '</div><div class="messages">' + message + '</div><div class="clear"></div></div>'));
        }

        var $ele = $('.chat-room-message-message.' + messageId);
        if (needsShowMore($ele, isCode)) {
            $('#chat :last-child').children('.messages').append('<div id="show-' + messageId + '" class="show-more">{show full text}</div>');
        }       

        if (document.body.className == 'hidden' && name != 'room') {
            updateTitle();
        }
    };
    ///////////////////////////////////////////////////////////////////////////////  


    ///////////////////////////////////////////////////////////////// C2S FUNCTIONS
    $message.keypress(function (e) {
        if (e.keyCode == 13) {
            sendMessage($message.val(), codeMessage);
            e.preventDefault();
        }
    });
    $sendMessage.on('click', function () {
        sendMessage($message.val(), codeMessage);
        $message.val('').focus();
    });
    function sendMessage(message, codeToggle) {
        chat.server.sendMessage(message, codeToggle, roomId);
        $message.val('').focus();
        codeMessage = false;
        toggleCodeSwitch(codeMessage);
    };
    $codeMessage.on('click', function () {
        codeMessage = !codeMessage;
        toggleCodeSwitch(codeMessage);
        $message.focus();
    });
    ///////////////////////////////////////////////////////////////////////////////


    //////////////////////////////////////////////////////////////// MISC FUNCTIONS
    function encodeMessageMaster(name, message, pm, code, messageId) {        
        if (pm) {
            return htmlEncodePrivateMessage(message, code, messageId);
        } else {
            switch (name) {
                case userName: return htmlEncodeMyMessage(message, code, messageId);
                case "room": return htmlEncodeRoomMessage(message, code, messageId);
                case "bot": return htmlEncodeBotMessage(message, code, messageId);
                default: return htmlEncodeMessage(message, code, messageId);
            }
        }
    }
    function htmlEncodeName(value) {        
        return '<div class="chat-room-message-name">' + htmlEncodeValue(value) + '</div>';
    };
    function htmlEncodeMessage(value, code, messageId) {
        return '<div class="chat-room-message-message ' + messageId + '">' + htmlEncodeValue(value, code) + '</div>';
    };
    function htmlEncodeMyMessage(value, code, messageId) {
        return '<div class="chat-room-message-message mine ' + messageId + '">' + htmlEncodeValue(value, code) + '</div>';
    };
    function htmlEncodeRoomMessage(value, code, messageId) {
        return '<div class="chat-room-message-message room ' + messageId + '">' + htmlEncodeValue(value, code) + '</div>';
    };
    function htmlEncodeBotMessage(value, code, messageId) {
        return '<div class="chat-room-message-message bot ' + messageId + '">' + htmlEncodeValue(value, code) + '</div>';
    };
    function htmlEncodePrivateMessage(value, code, messageId) {
        return '<div class="chat-room-message-message private ' + messageId + '">' + htmlEncodeValue(value, code) + '</div>';
    };
    function htmlEncodeOnlineUser(value, connectionId) {
        if (value == $displayName.val()) {
            return '<div id="user-' + connectionId + '" class="online-user active-user me ' + htmlEncodeValue(value, false) + '">' + htmlEncodeValue(value, false) + '</div>';
        } else {
            return '<div id="user-' + connectionId + '" class="online-user active-user ' + htmlEncodeValue(value, false) + '">' + htmlEncodeValue(value, false) + '</div>';
        }
    };
    function htmlEncodeValue(value, code) {
        return ((code == true) ? '<pre><code>' : '') + value + ((code == true) ? '</pre></code>' : '');
    };
    function toggleCodeSwitch(toggle) {
        if (toggle) {
            $codeMessage.removeClass('concrete');
            $codeMessage.addClass('carrot');
        } else {
            $codeMessage.removeClass('carrot');
            $codeMessage.addClass('concrete');
        }
    };
    function isLastMatched(senderConnectionId) {
        return $('#chat :last-child').hasClass(senderConnectionId);
    };
    function needsShowMore(ele, isCode) {
        var lineCount = ele.text().split('\n').length;
        var charCount = ele.text().length;
                
        return (lineCount > 4 && isCode) || charCount > 956;
    };

    $(document).on('click', '.show-more, .show-less', function () {
        var $this = $(this);
        var messageId = $this.attr('id').replace('show-', '');
        var $message = $('.chat-room-message-message.' + messageId);

        // determine if we need to show more or show less
        if ($this.hasClass('show-more')) {
            $message.css({ maxHeight: '10000000000px' });
            $this.text('{show minimized text}');
        } else {
            $message.css({ maxHeight: '102px' });
            $this.text('{show full text}');
        }
        $this.toggleClass('show-more');
        $this.toggleClass('show-less');

    });
    $(document).on('click', '.chat-room-message-wrapper > .messages > .chat-room-message-message > img', function (e) {
        $.colorbox({
            href: $(this).attr('src'),
            closeButton: false,
            photo: true,
            maxHeight: '800px',
            maxWidth: '800px'
        });
    });
    $(document).on('click', '.online-user', function (e) {
        // beginning of popup user context menu
        var $this = $(this);
        if (!($this.hasClass('me'))) {
            $message.val('*' + $this.text() + ' ').focus();
            setCaretToPos(document.getElementById('message'), $message.val().length);
        }
    })
    ///////////////////////////////////////////////////////////////////////////////

});