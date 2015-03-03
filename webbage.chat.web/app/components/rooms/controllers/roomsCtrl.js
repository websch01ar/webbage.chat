(function () {
    'use strict';

    angular.module('webbage.chat.rooms').controller('roomsCtrl', ['$scope', 'hubProxy', function ($scope, hub) {
        var roomsHub = hub('roomHub');

        $scope.rooms = [];
        roomsHub.on('populateRooms', function (rooms) {
            $scope.rooms = rooms;
        });
    }]);
})();