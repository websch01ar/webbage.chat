(function () {
    'use strict';

    angular
        .module('app')
        .controller('LoginController', LoginController);

    LoginController.$inject = ['auth', '$location']; 

    function LoginController(auth, $location) {

        // popup on page present
        auth.signin({
            popup: true
        }, function () {
            $location.path('/');
        }, function () {
            // Error callback
        });

    }
})();
