(function () {
    'use strict';

    angular.module('webbage.chat.rooms').controller('roomsCtrl', ['$scope', '$log', '$timeout', 'hubProxy', function ($scope, $log, $timeout, hub) {
        $scope.isLoading = true;

        var roomsHub = hub('roomHub', [
            {
                eventName: 'userConnected',
                callback: updateRoomUserList
            },
            {
                eventName: 'userDisconnected',
                callback: updateRoomUserList
            }
        ]);

        function updateRoomUserList(room) {
            $log.info(room);
            for (var i = 0; i < $scope.rooms.length; i++) {
                if (room.RoomID === $scope.rooms[i].RoomID && room.RoomKey === $scope.rooms[i].RoomKey) {
                    $scope.rooms[i].Users = room.Users;
                }
            }
        }

        roomsHub.ready(function () {
            roomsHub.invoke('GetRooms', [], function (result) {
                $timeout(function () {
                    $scope.rooms = result;
                    $scope.isLoading = false;
                }, 1000); // show loading for at least 1 second
            });
        });

        $scope.rooms = [];
    }]);
})();