var app  = angular.module('imperatur', [])

app.controller('accountsection', function($scope, $http) {
	$http.get('http://localhost:8090/Accounts').
		then(function(response) {
            $scope.accounts = response.data;
        });
});