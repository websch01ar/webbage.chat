(function () {
    'use strict';

    angular.module('webbage.chat.modals').controller('addRoomCtrl', ['$scope', '$modalInstance',
        function ($scope, $modalInstance) {
            function init() {
                $scope.newRoomName = '';
                $scope.newRoomDesc = '';
            }
            init();

            $scope.save = function () {
                var createRoom = true;
                if ($scope.newRoomName === '') {
                    alert('You must specify a name, it cannot be blank.');
                    createRoom = false;
                } else if ($scope.newRoomDesc === '') {
                    alert('You must specify a description, it cannot be blank.');
                    createRoom = false;
                }

                if (createRoom) {
                    $modalInstance.close({ Name: $scope.newRoomName, Description: $scope.newRoomDesc });
                }
            }
            $scope.cancel = function () {
                $modalInstance.dismiss('cancel');
            }
        }]);
})();
