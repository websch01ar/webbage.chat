(function () {
    'use strict';

    angular.module('webbage.chat', [
        'ngRoute',
        'auth0',
        'angular-storage',
        'angular-jwt',
        'webbage.chat.rooms'
    ])
        .config(['$routeProvider', 'authProvider', function ($routeProvider, authProvider) {
            $routeProvider
                .when('/', {
                    controller: 'loginCtrl',
                    templateUrl: 'app/components/default/views/login.tpl.html'
                });

            authProvider.init({
                domain: 'webbage.auth0.com',
                clientID: 'NjpJcv2innqRppQ7tnaOnP5GSwupT6qw',
                loginUrl: '/'
            });
        }])
        .run(['auth', '$rootScope', 'store', 'jwtHelper', '$location', function (auth, $root, store, jwtHelper, $location) {
            auth.hookEvents();

            $root.openRooms = [];
            $root.auth = auth;

            $root.$on('$locationChangeStart', function () {
                if (!auth.isAuthenticated) {
                    var token = store.get('token');
                    if (token) {
                        if (!jwtHelper.isTokenExpired(token)) {
                            auth.authenticate(store.get('profile'), token);
                        } else {
                            $location.path('/');
                        }
                    }
                }
            })
        }]);
})();