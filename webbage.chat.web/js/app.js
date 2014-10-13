(function () {
    'use strict';

    angular.module('app', [
        'ngAnimate',
        'ngRoute',
        'ui.bootstrap'
    ]).config(['$routeProvider', function ($routeProvider) {

        function checkLoggedIn($q, $log, authservice) {
            var deferred = $q.defer();
            // check to see if authenticated, this gets set during login
            if (!authservice.isAuthenticated()) {
                deferred.reject({ needsAuthentication: true});
            } else deferred.resolve();

            return deferred.promise;
        }

        $routeProvider.whenAuthenticated = function(path, route) {
            route.resolve = route.resolve || {};
            // extend route.resolve to include checkLoggedIn when we use 'whenAuthenticated'
            angular.extend(route.resolve, {isLoggedIn: ['$q', '$log', 'authservice', checkLoggedIn]});
            return $routeProvider.when(path, route);
        }

        $routeProvider
            .when('/login', { controller: 'LoginController', templateUrl: 'views/login.html', })
            .when('/register', { controller: 'RegisterController', templateUrl: 'views/register.html' })
            .whenAuthenticated('/', { controller: 'HomeController', templateUrl: 'views/home.html' });

    }]).run(['$location', '$rootScope', '$log', 'authservice', '$route',
        function ($location, $rootScope, $log, authservice, $route) {
            $rootScope.$on('$routeChangeError', function (ev, current, previous, rejection) {
                // if rejection is because of authentication
                if (rejection && rejection.needsAuthentication) {
                    var returnUrl = $location.url();
                    $location.path('/login').search({ returnUrl: returnUrl });
                }
            });
    }]);

})();