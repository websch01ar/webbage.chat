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
            $root.user = auth.profile;
            console.log($root.user);
        }, function () {
            // Error callback
        });

    }
})();
