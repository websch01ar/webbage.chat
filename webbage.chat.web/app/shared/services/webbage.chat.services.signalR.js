(function () {
    'use strict';

    angular.module('webbage.chat.services.signalR', []).factory('hubProxy', ['$log', '$timeout', '$rootScope', '$q', function ($log, $timeout, $root, $q) {
        return function (hubName, watches, queryString) {
            var connection = $.hubConnection(),
                hub = connection.createHubProxy(hubName),
                loadDeferment = $q.defer(),
                isLoaded = loadDeferment.promise;

            $log.info('Creating SignalR service for ' + hubName);

            // register the watches for events from the server to the client, need to do this before
            // the connection is started, otherwise we'll miss out on some and it'll cause bugs
            if (watches && watches.length > 0) {
                for (var i = 0; i < watches.length; i++) {
                    var watch = watches[i];
                    registerWatch(watch.eventName, watch.callback);
                }
            } else {
                $log.info('No watches registered for ' + hubName);
            }

            // set the query string before we start the connection
            if (queryString) {
                $log.info('Setting query string for ' + hubName + ': ' + queryString);
                connection.qs = queryString;
            } else {
                $log.info('No query string set for ' + hubName);
            }

            function registerWatch(eventName, callback) {
                $log.info('Registering client watch: ' + eventName + ' on ' + hubName);
                hub.on(eventName, function (result) {
                    $log.info('Client watch ' + eventName + ' triggered on ' + hubName);
                    $root.$apply(function () {
                        if (callback) {
                            callback(result);
                        }
                    });
                });
            }

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
            connection.disconnected(function () { $timeout(function () { connect(); }, 5000); });

            return {
                // on: registerWatch,       // removed because watches should be registered during construction
                ready: function (callback) {
                    return isLoaded.then(function () {
                        callback();
                    });
                },
                invoke: function (methodName, args, callback) {
                    $log.info('Invoking ' + methodName + ' on ' + hubName);
                    return hub.invoke.apply(hub, $.merge([methodName], args))
                        .done(function (result) {
                            $root.$apply(function () {
                                if (callback) {
                                    $log.info('Executing callback for ' + methodName + ' on ' + hubName);
                                    $log.info(result);
                                    callback(result);
                                }
                            });
                        })
                        .fail(function (error) {
                            $log.error('Error invoking ' + methodName + ' on ' + hubName + ' on server: ' + error );
                        })
                },
                connection: connection
            };
        }
    }]);
})();