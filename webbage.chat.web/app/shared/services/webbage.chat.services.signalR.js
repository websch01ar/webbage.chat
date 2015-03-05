(function () {
    'use strict';

    angular.module('webbage.chat.services.signalR', []).factory('hubProxy', ['$log', '$timeout', '$rootScope', '$q', function ($log, $timeout, $root, $q) {
        return function (hubName) {
            var connection = $.hubConnection(),
                hub = connection.createHubProxy(hubName),
                loadDeferment = $q.defer(),
                isLoaded = loadDeferment.promise;

            function connect() {
                connection.start({ logging: true })
                    .done(function () {
                        $log.info('Connection established to ' + hubName + '. Connection ID: ' + connection.id);
                        loadDeferment.resolve();
                    })
                    .fail(function (error) {
                        $log.error('Error connecting to ' + hubName + ': ' + error);
                        loadDeferment = $q.defer();
                        isLoaded = loadDeferment.promise;
                    });
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
                        });
                    });
                },
                ready: function (callback) {
                    return isLoaded.then(function () {
                        callback();
                    });
                },
                invoke: function (methodName, args, callback) {
                    return hub.invoke.apply(hub, $.merge([methodName], $.makeArray(args))).done(function (result) {
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