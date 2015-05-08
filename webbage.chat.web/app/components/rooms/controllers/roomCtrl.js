(function () {
    'use strict';

    angular.module('webbage.chat.rooms').controller('roomCtrl', ['$scope', '$routeParams', '$rootScope', 'hubProxy', '$log', 'hotkeys', '$timeout', '$window', '$location',
        function ($scope, $routeParams, $root, hub, $log, hotkeys, $timeout, $window) {
            //#region variable declaration
            $scope.isLoading = true;
            $scope.onlineUsers = [];
            $scope.messages = [];
            $scope.playSound = true;
            $scope.isMessageCode = false;
            $scope.isTextAreaFocused = false;
            $scope.showContextMenu = $root.auth.profile.isGod;
            console.log($scope.showContextMenu);
            $scope.tabs = [
                { name: 'Room', canClose: false, hub: chatHub, active: true }
            ];
            
            var messageNotification = new Audio('/content/media/new-message-notification.mp3'),
                connectionNotification = new Audio('/content/media/connection-notification.mp3'),
                newMessageCount = 0,
                isTabActive = true,
                tabTitle = $routeParams.roomId + ' - webbage.chat',
                sentMessagesIndex = 0,
                sentMessages = [];
            //#endregion

            //#region toggle tab dis/active
            $window.document.title = tabTitle

            angular.element($window)
                .bind('focus', function () { 
                    isTabActive = true;
                    newMessageCount = 0;
                    $window.document.title = tabTitle
                })
                .bind('blur', function () {
                    isTabActive = false;
                })
            //#endregion

            var messageNotification = new Audio('/content/media/new-message-notification.mp3'),
                connectionNotification = new Audio('/content/media/connection-notification.mp3');

            $scope.consecutive = 0;
            //#region hub instantiation
            var queryString = 'roomKey=' + $routeParams.roomKey + '&roomId=' + $routeParams.roomId + '&userName=' + $root.auth.profile.name + '&userPicture=' + $root.auth.profile.picture;
            var chatHub = hub(
                'chatHub',
                [                    
                    {
                        eventName: 'receiveMessage',
                        callback: function (message) {
                            // determine if this message is from the same user as the last message
                            if ($scope.messages.length > 0) {                            
                                message.isConsecutive =
                                    $scope.messages[$scope.messages.length - 1].Sender.Name === message.Sender.Name &&
                                    $scope.messages[$scope.messages.length - 1].Sender.Picture === message.Sender.Picture
                            }
                            $scope.messages.push(message);

                            // determine if we need to play sound and if so what sound to play
                            if ($scope.playSound) {
                                var isMyMessage = $root.auth.profile.name === message.Sender.Name &&
                                                  $root.auth.profile.picture === message.Sender.Picture;

                                var isConnectionMessage = message.Content.endsWith('has connected') &&
                                                          message.Sender.Name === 'room' &&
                                                          message.Sender.Picture === '';

                                // message notification should only play when
                                //  1. Messages isn't my own
                                //  2. Message isn't a connection notification
                                //  3. Tab is not the active on (browser window DOESN'T have to be active for this)
                                if (!isMyMessage && !isConnectionMessage && !isTabActive) {
                                    $timeout(function () { messageNotification.play(); });                                
                                } else if (isConnectionMessage) {
                                    $timeout(function () { connectionNotification.play(); });
                                }
                            }

                            // determine if we need to increment the notification count on the tab
                            if (!isTabActive) {
                                $window.document.title = '(' + ++newMessageCount + ') ' + tabTitle
                            }
                        }
                    },
                    {
                        eventName: 'kill',
                        callback: function () {
                            $scope.messages.push({
                                Sender: {
                                    Name: 'room',
                                    Picture: ''
                                },
                                Content: 'You have been disconnected',
                                Sent: ''
                            });
                            chatHub.kill();
                        }
                    },
                    {
                        eventName: 'updateOnlineUsers',
                        callback: function (users) {
                            $log.info('webbage.chat.rooms.roomCtrl(): updateOnlineUsers(): ', users);
                            $scope.onlineUsers = users;
                        }
                    }
                ],
                queryString
            );
            //#endregion

            // now that we've established a connection, let everyone else know
            chatHub.ready(function () {
                $log.info('webbage.chat.rooms.roomCtrl(): chatHub is ready');
                $log.info('webbage.chat.rooms.roomCtrl(): Your profile: ', $root.auth.profile);
                $scope.isLoading = false;
                //chatHub.invoke('UserConnect', [])
            });

            //#region hotkeys
            hotkeys.bindTo($scope)
                .add({ // toggle sound
                    combo: 'shift+ctrl+v',
                    description: 'Toggle sound in a chatroom',
                    allowIn: ['TEXTAREA'],
                    callback: function (event, hotkey) {
                        event.preventDefault();
                        $scope.playSound = !$scope.playSound;
                    }
                })
                .add({ // toggle code
                    combo: 'shift+ctrl+c',
                    description: 'Toggle whether or not a message should be code',
                    allowIn: ['TEXTAREA'],
                    callback: function (event, hotkey) {
                        event.preventDefault();
                        $scope.isMessageCode = !$scope.isMessageCode;
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
                })
                .add({
                    combo: 'up',
                    description: 'Scroll up through previously sent messages',
                    allowIn: ['TEXTAREA'],
                    callback: function (event, hotkey) {
                        if (sentMessagesIndex > 0) {
                            sentMessagesIndex--;
                        }

                        if ($scope.isTextAreaFocused && sentMessagesIndex >= 0) {
                            $scope.message = sentMessages[sentMessagesIndex].Content;
                        }
                    }
                })
                .add({
                    combo: 'down',
                    description: 'Scroll down through previously sent messages',
                    allowIn: ['TEXTAREA'],
                    callback: function (event, hotkey) {
                        if (sentMessagesIndex < sentMessages.length - 1) {
                            sentMessagesIndex++;
                        }

                        if ($scope.isTextAreaFocused && sentMessagesIndex < sentMessages.length) {
                            $scope.message = sentMessages[sentMessagesIndex].Content;
                        }
                    }
                })
            //#endregion

            //#region client-to-server events
            $scope.message = '';
            $scope.sendMessage = function  () {
                if ($scope.message.trim() !== '') {
                    var message = {
                        Sender: {
                            Name: $root.auth.profile.name,
                            Picture: $root.auth.profile.picture,
                            IsAdmin: $root.auth.profile.isGod
                        },
                        Content: $scope.message,
                        IsCode: $scope.isMessageCode
                    };

                    chatHub.invoke('BroadcastMessage', [message]);

                    sentMessages.push(message);
                    sentMessagesIndex = sentMessages.length;

                    $scope.message = '';
                }
            }
            $scope.kickUser = function (user) {
                chatHub.invoke('RemoveUser', [user]);
            }
            //#endregion

            $scope.clearChat = function (user) {
                $(".message-container").remove();
                $scope.messages = [];
            }

            $scope.$on('$destroy', function () {
                //chatHub.invoke('UserDisconnect', []); // take out, see what happens
                chatHub.destroy();
            });
        }]);
})();