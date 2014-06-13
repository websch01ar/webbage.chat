$(document).ready(function () {
    $('.chat-room-display').click(function () {        
        // get the chatroom attribute
        location.replace("rooms/" + $(this).data('chatroom'));
    });
});