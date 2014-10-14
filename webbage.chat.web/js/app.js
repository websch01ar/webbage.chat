(function () {
    'use strict';

    angular.module('app', [
        'ngAnimate',
        'ngRoute',
        'ui.bootstrap',
        'auth0'
    ]).config(function (authProvider, $routeProvider, $locationProvider) {
        $routeProvider
            .when('/login', { controller: 'LoginController', templateUrl: 'views/login.html', })            
            .when('/', { controller: 'HomeController', templateUrl: 'views/home.html', requiresLogin: true });

        authProvider.init({
            domain: 'webbage.auth0.com',
            clientID: 'NjpJcv2innqRppQ7tnaOnP5GSwupT6qw',
            callbackURL: location.href,
            loginUrl: '/login'
        });
    }).run(function($rootScope) {
        $rootScope.signout = function () {
            alert(authProvider);
            authProvider.signout();
        }
    });

})();