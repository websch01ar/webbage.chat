(function () {

    angular.module('webbage.directives').directive('roomDisplay', function () {
        return {
            restrict: 'E',
            require: '^ngModel',
            scope: {
                ngModel: '@'  
            },
            templateUrl: 'app/webbage.directives/templates/room-display.html'
        }
    });

})();