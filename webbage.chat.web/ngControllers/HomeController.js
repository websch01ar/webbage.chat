(function () {
    'use strict';

    angular
        .module('app')
        .controller('HomeController', HomeController);

    HomeController.$inject = ['$scope']; 

    function HomeController($scope) {
        activate();
        function activate() { }
    }
})();
