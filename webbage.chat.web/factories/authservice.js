(function () {
    'use strict';

    angular
        .module('app')
        .factory('authservice', authservice);

    authservice.$inject = ['$http'];

    function authservice($http) {
        var user = {
            isAuthenticated: false,
            name: ''
        };

        var service = {
            init: init,
            isAuthenticated: isAuthenticated,
            login: login,
            logout: logout,
            register: register
        };
        return service;

        function init(isAuthenticated, name) {
            user.isAuthenticated = isAuthenticated;
            user.name = name;
        }

        function isAuthenticated() {
            return user.isAuthenticated;
        }

        function login() {

        }

        function logout() {

        }

        function register() {

        }
    }
})();