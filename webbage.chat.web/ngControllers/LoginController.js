(function () {
    'use strict';

    angular
        .module('app')
        .controller('LoginController', LoginController);

    LoginController.$inject = ['$scope', 'auth', '$location']; 

    function LoginController($scope, auth, $location) {
        auth.signin({
            popup: true,
            icon: '',
            showIcon: true
        }, function () {
            $location.path('/');
        }, function () {
            // Error callback
        });
    }
})();
