(function () {
    'use strict';

    angular
        .module('app')
        .controller('LoginController', LoginController);

    LoginController.$inject = ['$scope', 'auth']; 

    function LoginController($scope, auth) {

        $scope.login = function () {
            auth.signin({
                popup: true
            }, function () {
                // Success callback
            }, function () {
                // Error callback
            });
        }
    }
})();
