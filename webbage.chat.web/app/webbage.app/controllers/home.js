(function () {
    'use strict';

    angular.module('webbage.app').controller('home', home);

    home.$inject = ['$scope', 'signalR']; 

    function home($scope, signalR) {
        signalR.init();

        $scope.sendMessage = function () {
            signalR.send('test', 'test');
        }

        activate();
        function activate() { }
    }
})();
