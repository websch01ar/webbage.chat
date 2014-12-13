(function () {

    angular.module('webbage.chat').factory('signalRroom', function ($rootScope) {
        var connection;
        var proxy;

        var initialize = function (callback) {
            connection = $.hubConnection();
            proxy = connection.createHubProxy('roomHub');

            // SERVER EVENT LISTENING //////////////////////////////////
            // receive room listing from server
            proxy.on('updateRooms', function (rooms) {
                console.log('emitting');
                console.log(rooms);
                $rootScope.$broadcast('updateRooms', { rooms: rooms });
            });
            ////////////////////////////////////////////////////////////

            connection.start()
                .done(function () {
                    console.log('Connection established. Enjoy!');
                    callback();
                })
                .fail(function () {
                    console.log('Could not connect. Try refreshing.');
                });
        };

        var getRooms = function () {
            proxy.invoke('getRooms')
                .fail(function (error) {
                    console.log('Invocation of "getRooms" failed. ' + error);
                });
        }

        return {
            init: initialize,
            getRooms: getRooms
        }
    });

})();