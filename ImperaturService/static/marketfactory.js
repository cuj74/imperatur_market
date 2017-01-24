(function () {
    'use strict';

    angular
        .module('imperatur')
        .factory('market', market);

    market.$inject = ['$http'];

    function market($http) {
        var service = {
            getData: getData
        };

        return service;

        function getData() { }
    }
})();