(function () {
    'use strict';

    angular.module('webbage.chat.rooms').controller('roomCtrl', ['$scope', '$routeParams', '$rootScope', 'hubProxy', '$log', function ($scope, $routeParams, $root, hub, $log) {
        var chatHub = hub('chatHub');

        chatHub.on('userConnected', function (user) {
            console.log(user);
        });
        
        chatHub.ready(function () {
            chatHub.invoke('UserConnect', [
                { Name: $root.auth.profile.name, Picture: $root.auth.profile.picture },
                { RoomKey: $routeParams.roomKey, RoomID: $routeParams.roomId }
            ]);
        });
    }]);
})();