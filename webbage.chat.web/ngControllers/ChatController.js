(function () {
    'use strict';

    angular
        .module('app')
        .controller('ChatController', ChatController);

    ChatController.$inject = ['$scope', 'hubcontext'];

    function ChatController($scope, $hub) {
        $hub.connect();

        $scope.sendMessage = function () {
            $hub.sendMessage(null);
        }
        
        $scope.messages = [];
        $scope.$on('broadcastMessage', function (event, message) {
            $scope.messages.push(message);
            $scope.$apply(); // call this to insure changes on scope are reflected in DOM
        });

        activate();
        function activate() { }
    }
})();
