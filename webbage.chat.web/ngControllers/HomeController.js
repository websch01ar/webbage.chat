(function () {
    'use strict';

    angular
        .module('app')
        .controller('HomeController', HomeController);

    HomeController.$inject = ['$scope', 'auth']; 

    function HomeController($scope, auth) {
        $scope.auth = auth;

        activate();
        function activate() { }
    }
})();
