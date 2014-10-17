(function () {
    'use strict';

    angular
        .module('app')
        .factory('hubcontext', hubcontext);

    hubcontext.$inject = ['$', '$rootScope'];

    function hubcontext($, $root) {
        var proxy;
        var connection;

        var context = {
            connect: connectHub,
            isConnecting: function () {
                return connection.state === 0;
            },
            isConnected: function () {
                return connection.state === 1;
            },
            sendMessage: function (message) {
                proxy.invoke('sendMessage', message);
            }
        }

        function connectHub() {
            connection = $.hubConnection();
            proxy = connection.createHubProxy('chatHub');

            proxy.on('broadcastMessage', function (message) {
                $rootScope.$broadcast('broadcastMessage', message);
            });

            connection.start();
        }
    }
})();