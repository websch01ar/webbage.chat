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
    chat.client.userConnected = function (name) {
        // notify room they joined        
        var chatRoomMessage = encodeMessageMaster("room", name + ' has joined the room', false, false);
        appendMessage(chatRoomMessage);
    };
    chat.client.userDisconnected = function (name) {
        // remove from online-user-list
        var $div = $('#online-user-list .online-user.' + name);
        $div.remove();

        // notify room they left
        var chatRoomMessage = encodeMessageMaster("room", name + ' has left the building', false, false);
        appendMessage(chatRoomMessage);
    };
    chat.client.addNewMessageToPane = function (name, message, pm, isCode) {
        var chatRoomMessage = encodeMessageMaster(name, message, pm, isCode);
        appendMessage(chatRoomMessage);
    };
    chat.client.updateOnlineUsers = function (users) {
        $.each($.parseJSON(users), function () {            
            if (!($('#online-user-list .online-user.' + this.UserName).length)) {                
                var onlineUserContent = htmlEncodeOnlineUser(this.UserName);
                $onlineUserList.append(onlineUserContent);
            }
        });
    };

    // add desired message to the chat pane
    function appendMessage(message) {
        if (($chatDisplay[0].scrollHeight - $chatDisplay.scrollTop()) == $chatDisplay.outerHeight()) {
            $chatDisplay.append(message);
            $chatDisplay.animate({ scrollTop: $chatDisplay[0].scrollHeight }, 500);
        } else {
            $chatDisplay.append(message);
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
    }
    $codeMessage.on('click', function () {
        codeMessage = !codeMessage;
        toggleCodeSwitch(codeMessage);
        $message.focus();
    });
    ///////////////////////////////////////////////////////////////////////////////


    //////////////////////////////////////////////////////////////// MISC FUNCTIONS
    function encodeMessageMaster(name, message, pm, code) {
        var nameDiv = htmlEncodeName(name);
        var messageDiv = '';
        if (pm) {
            messageDiv = htmlEncodePrivateMessage(message, code);
        } else {
            switch (name) {
                case userName: messageDiv = htmlEncodeMyMessage(message, code); break;
                case "room": messageDiv = htmlEncodeRoomMessage(message, code); break;
                case "bot": messageDiv = htmlEncodeBotMessage(message, code); break;
                default: messageDiv = htmlEncodeMessage(message, code); break;
            }
        }

        return $('<div class="chat-room-message-wrapper">' + nameDiv +  messageDiv + '<div class="clear"></div></div>')
    }
    function htmlEncodeName(value) {        
        return '<div class="chat-room-message-name">' + htmlEncodeValue(value) + '</div>';
    };
    function htmlEncodeMessage(value, code) {
        return '<div class="chat-room-message-message">' + htmlEncodeValue(value, code) + '</div>';
    };
    function htmlEncodeMyMessage(value, code) {
        return '<div class="chat-room-message-message mine">' + htmlEncodeValue(value, code) + '</div>';
    };
    function htmlEncodeRoomMessage(value, code) {
        return '<div class="chat-room-message-message room">' + htmlEncodeValue(value, code) + '</div>';
    };
    function htmlEncodeBotMessage(value, code) {
        return '<div class="chat-room-message-message bot">' + htmlEncodeValue(value, code) + '</div>';
    };
    function htmlEncodePrivateMessage(value, code) {
        return '<div class="chat-room-message-message private">' + htmlEncodeValue(value, code) + '</div>';
    };
    function htmlEncodeOnlineUser(value) {
        if (value == $displayName.val()) {
            return '<div class="online-user active-user me ' + htmlEncodeValue(value) + '">' + htmlEncodeValue(value) + '</div>';
        } else {
            return '<div class="online-user active-user ' + htmlEncodeValue(value) + '">' + htmlEncodeValue(value) + '</div>';
        }
    };
    function htmlEncodeValue(value, code) {
        return ((code == true) ? '<pre><code>' : '') + $('<div/>').text(value).html() + ((code == true) ? '</pre></code>' : '');
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
    ///////////////////////////////////////////////////////////////////////////////

});