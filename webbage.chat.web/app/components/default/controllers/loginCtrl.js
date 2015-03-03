(function () {
    'use strict';

    angular.module('webbage.chat').controller('loginCtrl', ['$scope', '$location', 'auth', 'store', function ($scope, $location, auth, store) {
        if (auth.isAuthenticated) {
            $location.path('/rooms');
        }

        $scope.login = function () {
            auth.signin({},
                function (profile, token) { // success
                    store.set('profile', profile);
                    store.set('token', token);
                    store.set('authed', true);
                    $location.path('/rooms');
                },
                function () { // error

                });
        }
    }])
})();