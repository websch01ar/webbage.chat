(function () {
    'use strict';

    angular.module('webbage.chat.rooms').controller('roomsCtrl', ['$scope', '$log', '$timeout', '$modal', 'hubProxy',
        function ($scope, $log, $timeout, $modal, hub) {
            //#region Variable Declaration
            var roomsHub = undefined;
            //#endregion

            function init() {
                $scope.isLoading = true;
                $scope.rooms = [];

                roomsHub = hub('roomHub', [
                    {
                        eventName: 'userConnected',
                        callback: updateRoomUserList
                    },
                    {
                        eventName: 'userDisconnected',
                        callback: updateRoomUserList
                    },
                    {
                        eventName: 'getNewRoom',
                        callback: addRoomToList
                    }
                ]);

                roomsHub.ready(function () {
                    roomsHub.invoke('GetRooms', [], function (result) {
                        $timeout(function () {
                            $scope.rooms = result;
                            $scope.isLoading = false;
                        }, 1000); // show loading for at least 1 second
                    });
                });
            }
            init();

            function updateRoomUserList(room) {
                $log.info(room);
                for (var i = 0; i < $scope.rooms.length; i++) {
                    if (room.RoomID === $scope.rooms[i].RoomID && room.RoomKey === $scope.rooms[i].RoomKey) {
                        $scope.rooms[i].Users = room.Users;
                    }
                }
            }

            function addRoomToList(room) {
                $log.info(room);
                $scope.rooms.push(room);
            }

            $scope.addRoom = function () {
                var modal = $modal.open({
                    animation: false,
                    size: undefined,
                    templateUrl: 'app/shared/modals/addRoom/addRoom.tpl.html',
                    controller: 'addRoomCtrl'
                });

                modal.result.then(
                    function (room) { // user clicked save, send it to server
                        roomsHub.invoke('AddRoom', [room]);
                    },
                    function () { // user clicked cancel, do nothing

                    })
            }
        }]);
})();