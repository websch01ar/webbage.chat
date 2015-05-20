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
						log: prepareLogFn(_$log.log, className),
						info: prepareLogFn(_$log.info, className),
						warn: prepareLogFn(_$log.warn, className),
						debug: prepareLogFn(_$log.debug, className),
						error: prepareLogFn(_$log.error, className),
						getInstance: getInstance
					};
				};

			$log.log = prepareLogFn($log.log);
			$log.info = prepareLogFn($log.info);
			$log.warn = prepareLogFn($log.warn);
			$log.debug = prepareLogFn($log.debug);
			$log.error = prepareLogFn($log.error);
			$log.getInstance = getInstance;

			return $log;
		}];
	}]);
})();
