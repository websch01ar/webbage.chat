(function () {
    'use strict';

    angular.module('webbage.chat.rooms').controller('roomsCtrl', ['$scope', '$log', 'hubProxy', function ($scope, $log, hub) {
        var roomsHub = hub('roomHub');

        $scope.rooms = [];
        roomsHub.on('populateRooms', function (rooms) {
            $scope.rooms = rooms;
        });
    }]);
})();