$(function () {
    var chat = $.connection.chatHub;
    var $displayName = $('#displayName');
    var $chatDisplay = $('#chat');
    var $message = $('#message');
    var $sendMessage = $('#sendMessage');
    var $onlineUserList = $('#online-user-list');

    // set height of the main chat div                 
    $chatDisplay.height(($(window).height() - 200) + 'px');

    // get the display name
    // TODO: Replacement code will be needed when accounts are implemented, just replace userName
    while (!$displayName.val()) {
        $displayName.val(prompt('Enter your name:', '').trim().replace(' ',''));
    };
    var userName = $displayName.val();
    
    // connection start and store user as online
    $.connection.hub.qs = "userName=" + userName;
    $.connection.hub.start().done(function () {
        // no implementation yet
    });

    ///////////////////////////////////////////////////////////////// S2C FUNCTIONS
    chat.client.userConnected = function (name) {
        // notify room they joined        
        var chatRoomMessage = $('<div class="chat-room-message-wrapper">' + htmlEncodeName("room") + htmlEncodeRoomMessage(name + ' has joined the room.') + '<div class="clear"></div></div>');
        appendMessage(chatRoomMessage);
    };
    chat.client.userDisconnected = function (name) {
        // remove from online-user-list
        var $div = $('#online-user-list .online-user.' + name);
        $div.remove();

        // notify room they left
        var chatRoomMessage = $('<div class="chat-room-message-wrapper">' + htmlEncodeName("room") + htmlEncodeRoomMessage(name + ' has left the building.') + '<div class="clear"></div></div>');
        appendMessage(chatRoomMessage);
    };
    chat.client.addNewMessageToPane = function (name, message, pm) {
        var chatRoomMessage = '';
        if (!(pm)) {
            if (name == $displayName.val()) {
                chatRoomMessage = $('<div class="chat-room-message-wrapper">' + htmlEncodeName(name) + htmlEncodeMyMessage(message) + '<div class="clear"></div></div>');
            } else if (name == 'room') {
                chatRoomMessage = $('<div class="chat-room-message-wrapper">' + htmlEncodeName(name) + htmlEncodeRoomMessage(message) + '<div class="clear"></div></div>');
            } else if (name == 'bot') {
                chatRoomMessage = $('<div class="chat-room-message-wrapper">' + htmlEncodeName(name) + htmlEncodeBotMessage(message) + '<div class="clear"></div></div>');
            } else {
                chatRoomMessage = $('<div class="chat-room-message-wrapper">' + htmlEncodeName(name) + htmlEncodeMessage(message) + '<div class="clear"></div></div>');
            }
        } else {
            chatRoomMessage = $('<div class="chat-room-message-wrapper">' + htmlEncodeName(name) + htmlEncodePrivateMessage(message) + '<div class="clear"></div></div>');
        }
        appendMessage(chatRoomMessage);
    };
    chat.client.updateOnlineUsers = function (users) {
        $.each($.parseJSON(users), function () {
            if (!($('#online-user-list .online-user.' + this.Name).length)) {
                var onlineUserContent = htmlEncodeOnlineUser(this.Name);
                $onlineUserList.append(onlineUserContent);
            }
        });
    };

    // add desired message to the chat pane
    function appendMessage(message) {
        $chatDisplay.append(message);
        $chatDisplay.animate({ scrollTop: $chatDisplay[0].scrollHeight }, 500);
        $message.val('').focus();
    };
    ///////////////////////////////////////////////////////////////////////////////  

    ///////////////////////////////////////////////////////////////// C2S FUNCTIONS
    $message.keypress(function (e) {
        if (e.keyCode == 13) {
            determineMessageRoute($message.val());
        }
    });
    $sendMessage.on('click', function () {
        determineMessageRoute($message.val());
    });
    function determineMessageRoute(message) {        
        if (message.substring(0, 1) == "*") {
            var recipientName = message.split(' ')[0].replace('*', '');
            message = message.replace('*' + recipientName + ' ', '');
            sendPrivateMessage(recipientName, message);
        } else {
            sendRoomMessage(message);
        }
    };
    function sendRoomMessage(message) {
        chat.server.sendToRoom(message);
    };
    function sendPrivateMessage(recipient, message) {
        chat.server.sendToUser(recipient, message);
    };
    ///////////////////////////////////////////////////////////////////////////////

    //////////////////////////////////////////////////////////////// MISC FUNCTIONS
    function htmlEncodeName(value) {
        return '<div class="chat-room-message-name">' + value + '</div>'
    };
    function htmlEncodeMessage(value) {
        return '<div class="chat-room-message-message">' + value + '</div>';
    };
    function htmlEncodeMyMessage(value) {
        return '<div class="chat-room-message-message mine">' + value + '</div>';
    };
    function htmlEncodeRoomMessage(value) {
        return '<div class="chat-room-message-message room">' + value + '</div>';
    };
    function htmlEncodeBotMessage(value) {
        return '<div class="chat-room-message-message bot">' + value + '</div>';
    };
    function htmlEncodePrivateMessage(value) {
        return '<div class="chat-room-message-message private">' + value + '</div>';
    };
    function htmlEncodeOnlineUser(value) {
        if (value == $displayName.val()) {
            return '<div class="online-user active-user me ' + value + '">' + value + '</div>';
        } else {
            return '<div class="online-user active-user ' + value + '">' + value + '</div>';
        }
    };
    ///////////////////////////////////////////////////////////////////////////////

});