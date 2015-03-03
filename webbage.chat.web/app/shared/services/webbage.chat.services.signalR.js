(function () {
    'use strict';

    angular.module('webbage.chat.services.signalR', []).factory('hubProxy', ['$log', '$timeout', '$rootScope', function ($log, $timeout, $root) {
        return function (hubName) {
            var connection = $.hubConnection(),
                hub = connection.createHubProxy(hubName);
            
            function connect() {
                connection.start({ logging: true })
                    .done(function () { $log.info('Connection established to ' + hubName + '. Connection ID: ' + connection.id); })
                    .fail(function (error) { $log.error('Error connecting to ' + hubName + ': ' + error); });
            }
            connect();

            connection.error(function (error) { $log.error('SignalR Error: ' + error); });

            connection.disconnected(function () {
                $timeout(function () { connect(); }, 5000);
            })

            return {
                on: function (eventName, callback) {
                    hub.on(eventName, function (result) {
                        $root.$apply(function () {
                            if (callback) {
                                callback(result);
                            }
                        })
                    })
                },
                invoke: function (methodName, callback) {
                    hub.invoke(methodName).done(function (result) {
                        $root.$apply(function () {
                            if (callback) {
                                callback(result);
                            }
                        });
                    });
                },
                connection: connection
            };
        }
    }]);

})();