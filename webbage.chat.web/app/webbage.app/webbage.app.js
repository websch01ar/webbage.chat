(function () {

    angular.module('webbage.app', ['ngRoute', 'auth0', 'angular-storage', 'angular-jwt']); // 'webbage.services', 'webbage.directives', 'webbage.users', 'webbage.rooms'

    var app = angular.module('webbage.app');

    app.config(function ($routeProvider, authProvider) {
        $routeProvider
            .when('/', { controller: 'home', templateUrl: '/app/webbage.app/views/home.html' })
            .when('/login', { controller: 'login', templateUrl: '/app/webbage.app/views/login.html' });

        authProvider.init({
            domain: 'webbage.auth0.com',
            clientID: 'NjpJcv2innqRppQ7tnaOnP5GSwupT6qw'
        });
    }).run(function (auth) {
        auth.hookEvents();
    });

})();