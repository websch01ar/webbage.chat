(function () {
    'use strict';

    angular.module('webbage.chat.rooms').controller('roomsCtrl', ['$scope', '$log', 'hubProxy', function ($scope, $log, hub) {
        var roomsHub = hub('roomHub', [
            {
                eventName: 'userConnected',
                callback: function (room, user) {
                    $log.info(room);
                    $log.info(user);
                }
            },
            {
                eventName: 'userDisconnected',
                callback: function (room, user) {
                    $log.info(room);
                    $log.info(user);
                }
            }
        ]);

        roomsHub.ready(function () {
            roomsHub.invoke('GetRooms', [], function (result) {
                $scope.rooms = result;
            });
        });

        $scope.rooms = [];
    }]);
})();