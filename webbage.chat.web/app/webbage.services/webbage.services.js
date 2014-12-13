(function () {

    angular.module('webbage.services', []);

    var app = angular.module('webbage.services');

    app.run(['$rootScope', '$injector', function ($rootScope, $injector) {
        $rootScope.oAuth = {
            token: 'ebadc0f8-6c7d-4b26-b84e-070873a6f343',
            loaded: false
        };

        $injector.get('$http').defaults.transformRequest = function (data, headersGetters) {
            if ($rootScope.oAuth) headersGetters()['Authorization'] = 'Bearer ' + $rootScope.oAuth.token;
        }
    }]);

})();