(function () {
    'use strict';

    angular
        .module('app')
        .controller('LoginController', LoginController);

    LoginController.$inject = ['$scope', 'auth', '$location', '$rootScope']; 

    function LoginController($scope, auth, $location, $root) {

        // popup on page present
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
