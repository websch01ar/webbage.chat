﻿(function () {
    'use strict';

    angular
        .module('app')
        .controller('HomeController', HomeController);

    HomeController.$inject = ['$scope', 'auth']; 

    function HomeController($scope, auth) {
        $scope.user = auth.profile;
        console.log(auth);
        console.log($scope.user);

        activate();
        function activate() { }
    }
})();
