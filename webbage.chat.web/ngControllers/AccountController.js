(function () {
    'use strict';

    angular
        .module('app')
        .controller('AccountController', AccountController);

    AccountController.$inject = ['$scope', 'auth']; 

    function AccountController($scope) {
        $scope.auth = auth;

        activate();
        function activate() { }
    }
})();
