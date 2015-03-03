(function () {
    'use strict';

    angular.module('webbage.chat.directives.userDisplay', []).directive('userDisplay', function () {
        return {
            restrict: 'E',
            templateUrl: 'app/shared/directives/userDisplay/userDisplay.tpl.html',
            scope: {
                user: '=',
                displaySize: '@'
            },
            controller: ['$scope', '$rootScope', '$location', function ($scope, $root, $location) {
                
            }],
            link: function (scope, ele, attrs) {
                console.log('here');
                scope.$watch('user', function (user) {
                    scope.user = user;
                });

                scope.$watch('displaySize', function (displaySize) {
                    scope.displaySize = displaySize;
                })
            }
        }
    })
})();