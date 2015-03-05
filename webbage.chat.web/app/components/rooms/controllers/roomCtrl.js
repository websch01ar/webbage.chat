(function () {
    'use strict';

    angular.module('webbage.chat.rooms').controller('roomCtrl', ['$scope', '$routeParams', '$rootScope', 'hubProxy', '$log', 'hotkeys', function ($scope, $routeParams, $root, hub, $log, hotkeys) {
        //#region variable declaration
        $scope.onlineUsers = [];
        //#endregion

        var queryString = 'roomKey=' + $routeParams.roomKey + '&roomId=' + $routeParams.roomId + '&userName=' + $root.auth.profile.name + '&userPicture=' + $root.auth.profile.picture;
        var chatHub = hub(
            'chatHub',
            [
                { 
                    eventName: 'userConnected', 
                    callback: function (user) {
                        $log.info('User Connected: ' + user.Name + ', ' + user.Picture);
                    }
                },
                {
                    eventName: 'userDisconnected',
                    callback: function (user) {
                        $log.info('User Disconnected: ' + user.Name + ', ' + user.Picture);
                    }
                },
                {
                    eventName: 'updateOnlineUsers',
                    callback: function (users) {
                        $scope.onlineUsers = users;
                    }
                }
            ],
            queryString
        );

        // now that we've established a connection, let everyone else know
        chatHub.ready(function () {
            chatHub.invoke('UserConnect', [])
        });

        //#region hotkeys
        hotkeys.bindTo($scope)
            .add({ // toggle sound
                combo: 'alt+v',
                description: 'Toggle sound in a chatroom',
                callback: function () {
                    $scope.playSound = !$scope.playSound;
                }
            })
            .add({ // toggle code
                combo: 'alt+c',
                description: 'Toggle whether or not a message should be code',
                callback: function () {
                    $scope.messageIsCode = !$scope.messageIsCode;
                }
            })
        //#endregion

        //#region client-to-server events
        $scope.message = '';
        $scope.sendMessage = function () {

        }
        //#endregion

        //#region $scope events
        $scope.$on('$destroy', function () {

            chatHub.invoke('UserDisconnect', []);
        });
        //#endregion
    }]);
})();