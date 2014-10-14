(function () {
    'use strict';

    angular
        .module('app')
        .controller('AccountController', AccountController);

    AccountController.$inject = ['$scope', 'auth']; 

    function AccountController($scope, auth) {
        $scope.auth = auth;

        activate();
        function activate() { }
    }
})();
