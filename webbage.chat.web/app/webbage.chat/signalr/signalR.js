(function () {

    angular.module('webbage.chat').factory('signalR', function ($rootScope) {
        var connection;
        var proxy;

        var initialize = function () {
            connection = $.hubConnection();
            proxy = connection.createHubProxy('chatHub');

            // listen for events from the server
            proxy.on('broadcastMessage', function (user, message) {
                console.log('received message');
                $rootScope.$emit('broadcastMessage', { user: user, message: message });
            });

            connection.start()
                .done(function () {
                    console.log('Connection established. Enjoy!');
                })
                .fail(function () {
                    console.log('Could not connect. Try refreshing.');
                });
        };

        var sendMessage = function (user, message) {
            proxy.invoke('send', user, message)
                .done(function () {
                    
                })
                .fail(function (error) {
                    console.log('Invocation of "send" failed. ' + error);
                });
        };

        return {
            init: initialize,
            send: sendMessage
        };
    });

})();