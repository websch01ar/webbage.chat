﻿(function () {
    'use strict';

    //lol need APP Harbor to pull and re-build so im not muted. 
    
    angular.module('webbage.chat.directives.message', []).directive('message', function () {
        return {
            restrict: 'E',
            templateUrl: 'app/shared/directives/message/message.tpl.html',
            scope: {
                messageItem: '='
            },
            controller: ['$scope', '$log', '$rootScope', function ($scope, $log, $root) {
                $scope.isMyMessage = $scope.messageItem.Sender.Name === $root.auth.profile.name && $scope.messageItem.Sender.Picture === $root.auth.profile.picture;
                $scope.messageItem.Content = $scope.messageItem.Content.linkify();
                $scope.messageItem.Sent = new Date($scope.messageItem.Sent).convertToLocalDate().toLocaleString();
                $log.info('webbage.chat.directives.message(): received message, isMyMessage: ', $scope.isMyMessage, $scope.messageItem);
            }],
            link: function (scope, ele, attrs) {
                scope.$watch('messageItem', function (messageItem) {
                    scope.messageItem = messageItem;
                });
            }
        }
    });
})();
