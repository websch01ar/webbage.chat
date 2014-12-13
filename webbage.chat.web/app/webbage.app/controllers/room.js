(function () {
    'use strict';

    angular.module('webbage.app').controller('room', room);

    

    function room() {
        $scope.connected = false;

        activate();
        function activate() { };
    };

})();