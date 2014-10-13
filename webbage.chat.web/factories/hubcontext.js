(function () {
    'use strict';

    angular
        .module('app')
        .factory('hubcontext', hubcontext);

    hubcontext.$inject = ['$http'];

    function hubcontext($http) {
        var service = {
            getData: getData
        };
        return service;

        function getData() { }
    }
})();