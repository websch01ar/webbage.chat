﻿(function () {

    angular.module('webbage.app', ['ngRoute', 'auth0', 'angular-storage', 'angular-jwt', 'webbage.services', 'webbage.directives', 'webbage.chat']);

    var app = angular.module('webbage.app');

    app.config(function ($routeProvider, authProvider) {
        $routeProvider
            .when('/', { controller: 'home', templateUrl: '/app/webbage.app/views/home.html' })
            .otherwise({ controller: 'error', templateUrl: 'app/webbage.app/views/home.html' });

        authProvider.init({
            domain: 'webbage.auth0.com',
            clientID: 'NjpJcv2innqRppQ7tnaOnP5GSwupT6qw',
            loginUrl: '/'
        });
    }).run(function ($rootScope, auth, store, jwtHelper, $location) {
        auth.hookEvents();

        $rootScope.auth = auth;        
        $rootScope.login = function () {
            auth.signin({}, function (profile, token) {
                store.set('profile', profile);
                store.set('token', token);
                console.log(store.get('profile'));
                $location.path('/');
            }, function () { });
        }
        $rootScope.logout = function () {
            auth.signout();
            store.remove('profile');
            store.remove('token');
        }

        // keep logged in on refresh using localStorage
        $rootScope.$on('$locationChangeStart', function () {
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
    });

})();