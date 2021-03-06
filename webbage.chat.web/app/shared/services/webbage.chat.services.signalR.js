﻿(function () {
    'use strict';

    angular.module('webbage.chat.services.signalR', ['ui.bootstrap', 'ngRoute'])
        .factory('hubProxy', ['$log', '$timeout', '$rootScope', '$q', '$modal', '$route', function ($log, $timeout, $root, $q, $modal, $route) {
            return function (hubName, watches, queryString) {
                var connection = $.hubConnection(),
                    hub = connection.createHubProxy(hubName),
                    deferred = $q.defer(),
                    promise = deferred.promise,
                    destroyed = true;

                $log = $log.getInstance('webbage.chat.services.signalR({0})'.format(hubName));                
                $log.info('Creating SignalR service for ' + hubName);

                function registerWatches() {
                    var deferred = $q.defer(),
                        promise = deferred.promise;

                    try {
                        if (watches && watches.length > 0) {
                            for (var i = watches.length; i-- > 0;) {
                                var watch = watches[i];
                                registerWatch(watch.eventName, watch.callback);
                            }
                        } else {
                            $log.warn('No watches registered for ' + hubName);
                        }
                    } finally {
                        deferred.resolve();
                    }

                    return promise;
                }
                function registerWatch(eventName, callback) {
                    $log.debug('Registering client watch: ' + eventName + ' on ' + hubName);
                    hub.on(eventName, function (result) {
                        $log.debug('Client watch ' + eventName + ' triggered on ' + hubName);
                        $root.$apply(function () {
                            if (callback) {
                                callback(result);
                            }
                        });
                    });
                }                        

                function connect() {
                    // set the query string before we start the connection
                    if (queryString) {
                        $log.info('Setting query string for ' + hubName + ': ' + queryString);
                        connection.qs = queryString;
                    } else {
                        $log.warn('No query string set for ' + hubName);
                    }

                    // register the watches for events from the server to the client, need to do this before
                    // the connection is started, otherwise we'll miss out on some and it'll cause bugs
                    registerWatches().then(function () {
                        connection.start({ logging: true })
                            .done(function () {
                                $log.info('Connection established. Connection ID: ' + connection.id);
                                deferred.resolve();
                            })
                            .fail(function (error) {
                                $log.error('Error during connection.start(): ' + error);
                                deferred = $q.defer();
                                promise = deferred.promise;
                            });
                    });
                }
                connect();

                function destroy() { // to prevent the popup from showing up when you disconnect manually
                    destroyed = true;
                    connection.stop();
                }
                function kill() { // to disconnect, allows a reconnection to happen
                    destroyed = false;
                    connection.stop();
                }

                connection.error(function (error) { $log.error('webbage.chat.services.signalR(' + hubName + '): SignalR Error: ' + error); });
                connection.disconnected(function () {
                    if (!destroyed) {
                        $log.error('Connection killed.');
						
                        var size = undefined,
                            modal = $modal.open({
                                templateUrl: 'app/shared/modals/disconnected/disconnected.tpl.html',
                                controller: 'disconnectedCtrl',
                                size: size
                            });

                        modal.result.then(
                            function () { // user clicked yes, reconnect
                                $timeout(function () {
                                    $route.reload();
                                })
                            },
                            function () { // user clicked no (what action to take here) //TODO: review
							});
                    } else {
                        $log.error('Connection lost.');
                    }
				});


                return {
                    ready: function (callback) {
                        return promise.then(function () {
                            callback();
                        });
                    },
                    invoke: function (methodName, args, callback) {
                        $log.debug('Invoking ' + methodName + ' on server');
                        return hub.invoke.apply(hub, $.merge([methodName], args))
                            .done(function (result) {
                                $root.$apply(function () {
                                    if (callback) {
                                        $log.debug('Executing callback for ' + methodName);
                                        callback(result);
                                    }
                                });
                            })
                            .fail(function (error) {
                                $log.error('Error invoking ' + methodName + ' on server: ' + error);
                            })
                    },
                    kill: kill,
                    destroy: destroy,
                    connection: connection
                };
            }
        }]);
})();