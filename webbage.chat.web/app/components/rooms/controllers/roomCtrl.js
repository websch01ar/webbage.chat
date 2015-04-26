(function () {
    'use strict';

    angular.module('webbage.chat.rooms').controller('roomCtrl', ['$scope', '$routeParams', '$rootScope', 'hubProxy', '$log', 'hotkeys', '$timeout',
        function ($scope, $routeParams, $root, hub, $log, hotkeys) {
            //#region variable declaration
            $scope.onlineUsers = [];
            $scope.messages = [];
            $scope.playSound = true;
            $scope.messageIsCode = false;
            //#endregion

            var messageNotification = new Audio('/content/media/new-message-notification.mp3');

            $scope.consecutive = 0;
            //#region hub instantiation
            var queryString = 'roomKey=' + $routeParams.roomKey + '&roomId=' + $routeParams.roomId + '&userName=' + $root.auth.profile.name + '&userPicture=' + $root.auth.profile.picture;
            var chatHub = hub(
                'chatHub',
                [
                    {
                        eventName: 'updateOnlineUsers',
                        callback: function (users) {
                            $log.info('webbage.chat.rooms.roomCtrl(): updateOnlineUsers(): ', users);
                            $scope.onlineUsers = users;
                        }
                    },
                    {
                        eventName: 'receiveMessage',
                        callback: function (message) {
                            // determine if this message is from the same user as the last message
                            if ($scope.messages.length > 0) {                            
                                message.isConsecutive =
                                    $scope.messages[$scope.messages.length - 1].Sender.Name === message.Sender.Name &&
                                    $scope.messages[$scope.messages.length - 1].Sender.Picture === message.Sender.Picture
                            }
                            $scope.messages[$scope.messages.length] = message;

                            // determine if we want to play sound
                            var isMyMessage = $root.auth.profile.name === message.Sender.Name &&
                                              $root.auth.profile.picture === message.Sender.Picture;

                            if ($scope.playSound) {
                                messageNotification.play();
                            }
                        }
                    }
                ],
                queryString
            );
            //#endregion

            // now that we've established a connection, let everyone else know
            chatHub.ready(function () {
                $log.info('webbage.chat.rooms.roomCtrl(): chatHub is ready');
                //chatHub.invoke('UserConnect', [])
            });

            //#region hotkeys
            hotkeys.bindTo($scope)
                .add({ // toggle sound
                    combo: 'alt+v',
                    description: 'Toggle sound in a chatroom',
                    allowIn: ['TEXTAREA'],
                    callback: function () {
                        $scope.playSound = !$scope.playSound;
                    }
                })
                .add({ // toggle code
                    combo: 'alt+c',
                    description: 'Toggle whether or not a message should be code',
                    allowIn: ['TEXTAREA'],
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
                    var message = {
                        Sender: {
                            Name: $root.auth.profile.name,
                            Picture: $root.auth.profile.picture
                        },
                        Content: $scope.message
                    };

                    chatHub.invoke('BroadcastMessage', [message]);
                    $scope.message = '';
                }
            }
            //#endregion
        
            $scope.$on('$destroy', function () {
                chatHub.invoke('UserDisconnect', []);
            })
        }]);
})();