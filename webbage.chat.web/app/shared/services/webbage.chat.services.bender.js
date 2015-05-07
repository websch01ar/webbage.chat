(function () {
	'use strict';

	angular.module('webbage.chat.services.bender', [
		'webbage.chat.services.signalR',		
	])
		.factory('bender.commands', function () {
			return {
				'!help': {},
				'!lmgtfy': {},
				'!insult': {},

			}
		})
		.factory('bender', ['$log', '$q', '$routeParams', 'bender.commands', 'hubProxy',
			function ($log, $q, $routeParams, commands, hub) {
				return function () {
					var deferred = $q.defer(),
						promise = deferred.promise,
						queryString = 'roomKey=' + $routeParams.roomKey + '&roomId=' + $routeParams.roomID + '&userName=bender',
						chatHub = hub('chatHub', null, queryString); // pass null for watches, don't need to watch anything here (for the time being)

					chatHub.ready(function () {
						$log.info('webbage.chat.service.bender: Bender up and ready. Bite my shiny metal ass!');
						deferred.resolve();
					})

					// this function accepts a message, gets the desired command from it, validates/parses/executes that command,
					// and then broadcasts the original message. Once the message has been broadcasted the command result will be
					// broadcasted as well
					function processCommand(message) {

					}

					function validateCommand(cmd) {

					}
					function parseCommand(cmd) {

					}
					function executeCommand(cmd) {

					}

					return {
						ready: function(fn) {
							return promise.then(function() {
								fn();
							});
						},
						processCommand: processCommand
					};
				}
			}]);
})();