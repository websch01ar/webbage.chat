(function () {
	'use strict';

	angular.module('webbage.chat.providers.log', []).provider('$log', ['$logProvider', function ($logProvider) {
	    this.$get = ['$injector', function ($injector) {
	        var $log = $injector.invoke($logProvider.$get),
				_$log = (function ($log) {
				    return {
				        log: $log.log,
				        info: $log.info,
				        warn: $log.warn,
				        debug: $log.debug,
				        error: $log.error
				    }
				})($log),
                _prepareLogFn = function (logFn, className) {
                    if (logLevels['log']) {
                        return prepareLogFn(logFn, className);
                    }
                    else return function () { angular.noop(); };
                },
                _prepareInfoFn = function (logFn, className) {
                    if (logLevels['info']) {
                        return prepareLogFn(logFn, className);
                    }
                    else return function () { angular.noop(); };
                },
                _prepareWarnFn = function (logFn, className) {
                    if (logLevels['warn']) {
                        return prepareLogFn(logFn, className);
                    }
                    else return function () { angular.noop(); };
                },
                _prepareDebugFn = function (logFn, className) {
                    if (logLevels['debug']) {
                        return prepareLogFn(logFn, className);
                    }
                    else return function () { angular.noop(); };
                },
                _prepareErrorFn = function (logFn, className) {
                    if (logLevels['error']) {
                        return prepareLogFn(logFn, className);
                    }
                    else return function () { angular.noop(); };
                },
				prepareLogFn = function (logFn, className) {
				    var enhanceLogFn = function () {
				        var args = Array.prototype.slice.call(arguments),
							now = new Date().toLocaleString();
                        
				        args[0] = '{0} :: {1}{2}'.format(now, className, args[0]);

				        logFn.call(null, args[0]);
				    }
				    return enhanceLogFn;
				},
				getInstance = function (className) {
				    className = (className ? className + ' :: ' : '');

				    return {
				        log: _prepareLogFn(_$log.log, className),
				        info: _prepareInfoFn(_$log.info, className),
				        warn: _prepareWarnFn(_$log.warn, className),
				        debug: _prepareDebugFn(_$log.debug, className),
				        error: _prepareErrorFn(_$log.error, className),
				        getInstance: getInstance
				    };
				};

	        $log.log = _prepareLogFn($log.log);
	        $log.info = _prepareInfoFn($log.info);
	        $log.warn = _prepareWarnFn($log.warn);
	        $log.debug = _prepareDebugFn($log.debug);
	        $log.error = _prepareErrorFn($log.error);
	        $log.getInstance = getInstance;

	        return $log;
	    }];
	}]);
})();
