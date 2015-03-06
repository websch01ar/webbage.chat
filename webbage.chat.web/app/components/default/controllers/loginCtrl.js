(function () {
    'use strict';

    angular.module('webbage.chat').controller('loginCtrl', ['$scope', '$location', '$log', 'auth', 'store', function ($scope, $location, $log, auth, store) {
        if (auth.isAuthenticated) {
            $location.path('/rooms');
        }

        $scope.login = function () {
            auth.signin({},
                function (profile, token) { // success
                    if (profile.name.trim() === '') {
                        $log.info('No name detected. Setting name to username');
                        profile.name === (profile.username.trim() !== '' ? profile.username : '');
                    }

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