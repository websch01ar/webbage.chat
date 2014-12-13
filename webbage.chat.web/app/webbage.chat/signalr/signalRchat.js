(function () {

    angular.module('webbage.chat').factory('signalRchat', function ($rootScope) {
        var connection;
        var proxy;

        var initialize = function () {
            connection = $.hubConnection();
            proxy = connection.createHubProxy('chatHub');

            // SERVER EVENT LISTENING //////////////////////////////////
            // message received from other user
            proxy.on('broadcastMessage', function (user, message) {
                console.log('received message');
                $rootScope.$emit('broadcastMessage', { user: user, message: message });
            });
            ////////////////////////////////////////////////////////////

            connection.start()
                .done(function () {
                    console.log('Connection established. Enjoy!');
                })
                .fail(function () {
                    console.log('Could not connect. Try refreshing.');
                });
        };

        var connect = function (user) {
            proxy.invoke('connect', user)
                .fail(function (error) {
                    console.log('Invocation of "connect" failed. ' + error);
                });
        }

        var disconnect = function (user) {
            proxy.invoke('disconnect', user)
                .fail(function (error) {
                    console.log('Invocation of "disconnect" failed. ' + error);
                });
        }

        var sendMessage = function (message) {
            proxy.invoke('sendMessage', message)
                .fail(function (error) {
                    console.log('Invocation of "sendMessage" failed. ' + error);
                });
        }

        return {
            init: initialize,
            connect: connect,
            disconnect: disconnect,
            send: sendMessage
        };
    });

})();