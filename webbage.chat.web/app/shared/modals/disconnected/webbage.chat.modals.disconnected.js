(function () {
	'use strict';

	angular.module('webbage.chat.modals').controller('disconnectedCtrl', ['$scope', '$modalInstance', function ($scope, $modalInstance) {
		$scope.yes = function () {
			$modalInstance.close();
		};

		$scope.no = function () {
			$modalInstance.dismiss('cancel');
		};
	}]);
})();