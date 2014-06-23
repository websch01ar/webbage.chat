﻿$(function () {
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
        console.log(messageId);

        var chatRoomMessage = encodeMessageMaster(name, message, pm, isCode, messageId);
        appendMessage(name, chatRoomMessage, senderConnectionId, messageId);
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
    function appendMessage(name, message, senderConnectionId, messageId) {
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
        if (needsShowMore($ele)) {
            $('#chat :last-child').children('.messages').append('<div id="show-' + messageId + '" class="show-more">{show full text}</div>');
        }
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
    function htmlEncodeOnlineUser(value) {
        if (value == $displayName.val()) {
            return '<div class="online-user active-user me ' + htmlEncodeValue(value, false) + '">' + htmlEncodeValue(value, false) + '</div>';
        } else {
            return '<div class="online-user active-user ' + htmlEncodeValue(value, false) + '">' + htmlEncodeValue(value, false) + '</div>';
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
    function needsShowMore(ele) {
        return ele.prop('scrollHeight') > 103;
    };
    ///////////////////////////////////////////////////////////////////////////////

});