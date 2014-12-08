function () {
    'use strict';

    angular
        .module('app')
        .factory('interceptor', interceptor);

    interceptor.$inject = ['$q', '$location', '$rootScope'];

    function interceptor($q, $location, $root) {
        var service = {
            responseError: responseError
        }
        return service;


        function responseError(response) {
            switch (response.status) {
                case 403: // unauthorized
                    $root.errorMessage = 'Access to this resource is forbidden';
                    $location.path('/login');
                    $location.replace();
                    break;
                default: break;
            }
            return $q.reject(response);
        }
    }

    angular.module('app').config(['$httpProvider', function ($httpProvider) {
        $httpProvider.interceptors.push('interceptor');
    }]);

})();