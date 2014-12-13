(function () {

    angular.module('webbage.app').controller('home-logged-in', homeLoggedIn);

    homeLoggedIn.$inject = ['$scope', 'auth', 'signalRroom'];

    function homeLoggedIn($scope, auth, signalRroom) {
        // initialize the connection and get the rooms once connected
        signalRroom.init(function () {
            signalRroom.getRooms();
        });
        
        $scope.$on('updateRooms', function (e, args) {
            console.log(args);
        });

        activate();
        function activate() { };
    };

})();