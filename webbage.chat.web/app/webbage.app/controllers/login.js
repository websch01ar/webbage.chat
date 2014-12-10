(function () {
    'use strict';

    angular.module('webbage.app').controller('login', login);

    login.$inject = ['$scope', 'store', '$location', 'auth'];

    function login($scope, store, $location, auth) {
        auth.signin({}, function (profile, token) {
            store.set('profile', profile);
            store.set('token', token);
            $location.path('/');
        }, function () { });

        activate();
        function activate() { }
    }
})();
