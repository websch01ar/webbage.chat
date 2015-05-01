(function () {
    'use strict';

    angular.module('webbage.chat').controller('navCtrl', ['$scope', '$rootScope', '$location', '$log', '$route', 'auth', 'store',
        function ($scope, $root, $location, $log, $route, auth, store) {
            $scope.login = function () {
                auth.signin({},
                    function (profile, token) { // success
                        if (profile.name.trim() === '') {
                            $log.info('No name detected. Setting name to nickname');
                            profile.name = (profile.nickname.trim() !== '' ? profile.nickname : '');
                        }

                        store.set('profile', profile);
                        store.set('token', token);
                        $route.reload();
                    },
                    function () { // error

                    });
            }

            $scope.logout = function () {
                console.log(auth);
                auth.signout();
                store.remove('profile');
                store.remove('token');
                $location.path('/');
            }
        }]);
})();