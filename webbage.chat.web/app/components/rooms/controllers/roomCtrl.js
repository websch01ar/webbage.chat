(function () {
    'use strict';

    angular.module('webbage.chat.rooms').controller('roomCtrl', ['$scope', '$routeParams', '$rootScope', 'hubProxy', '$log', 'hotkeys', function ($scope, $routeParams, $root, hub, $log, hotkeys) {
        //#region variable declaration
        $scope.onlineUsers = [];
        $scope.messages = [];
        //#endregion

        $scope.consecutive = 0;
        //#region hub instantiation
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
                },
                {
                    eventName: 'receiveMessage',
                    callback: function (message) {
                        //message.Sent = new Date(message.Sent).format('MMMM D, hh:mm');

                        if ($scope.messages.length > 0) {
                            message.isConsecutive =
                                $scope.messages[$scope.messages.length - 1].Sender.Name === message.Sender.Name &&
                                $scope.messages[$scope.messages.length - 1].Sender.Picture === message.Sender.Picture
                        }
                        $scope.messages[$scope.messages.length] = message;
                    }
                }
            ],
            queryString
        );
        //#endregion

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
            .add({
                combo: 'enter',
                description: 'Send message when enter is pressed',
                allowIn: ['TEXTAREA'],
                callback: function (event, hotkey) {
                    event.preventDefault();
                    $scope.sendMessage();
                }
            });
        //#endregion

        //#region client-to-server events
        $scope.message = '';
        $scope.sendMessage = function  () {
            if ($scope.message.trim() !== '') {
                chatHub.invoke('BroadcastMessage', [{ Name: $root.auth.profile.name, Picture: $root.auth.profile.picture }, $scope.message]);
                $scope.message = '';
            }
        }
        //#endregion
        
        $scope.$on('$destroy', function () {
            chatHub.invoke('UserDisconnect', []);
        })
    }]);
})();