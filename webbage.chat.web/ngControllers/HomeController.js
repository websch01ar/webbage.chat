(function () {
    'use strict';

    angular
        .module('app')
        .controller('HomeController', HomeController);

    HomeController.$inject = ['$scope', '$rootScope']; 

    function HomeController($scope, $root) {


        activate();
        function activate() { }
    }
})();
