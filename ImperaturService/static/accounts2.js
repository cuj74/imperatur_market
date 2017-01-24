var app  = angular.module('imperatur', [])

app.controller('accountListController', function ($scope, $http) {
	$http.get('http://localhost:8090/api/account').
		then(function(response) {
            $scope.accounts = response.data;
		});

	$scope.select = function (identifier) {
	    $http.get('http://localhost:8090/api/account/' + identifier).
            then(function (response) {
                $scope.selectedaccount = response.data;
            });
	}

});

//app.controller('accountController', ['$scope', function ($scope, $http) {
//    $scope.select = function (identifier) {
//        $http.get('http://localhost:8090/api/account/' + identifier).
//            then(function (response) {
//                $scope.selectedaccount = response.data;
//            });
//    }


//}]);



