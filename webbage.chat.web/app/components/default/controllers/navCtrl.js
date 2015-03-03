(function () {
    'use strict';

    angular.module('webbage.chat').controller('navCtrl', ['$scope', '$rootScope', '$location', 'auth', 'store', function ($scope, $root, $location, auth, store) {
        $scope.login = function () {
            auth.signin({},
                function (profile, token) { // success
                    store.set('profile', profile);
                    store.set('token', token);
                    $location.path('/rooms');
                },
                function () { // error

                });
        }

        $scope.logout = function () {
            auth.signout();
            store.remove('profile');
            store.remove('token');
            $location.path('/');
        }
    }]);
})();