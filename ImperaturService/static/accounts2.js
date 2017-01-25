var app = angular.module('imperatur', [])

app.controller('accountListController', function ($scope, $http) {
    $scope.account_info = true;
    $scope.account_transactions = false;
    $scope.account_holdings = true;
    $scope.account_orders = false;
    $scope.showme = true;
    $scope.currentpage = 'account';


    $scope.accountsearchfunction = function (searchstring) {
        $http.get('http://localhost:8090/api/accountsearch/' + searchstring).
      then(function (response) {
          $scope.accounts = response.data;
      })
    };

    $scope.dataLoading = true;
    $http.get('http://localhost:8090/api/account').
		then(function (response) {
		    $scope.accounts = response.data;
		})
		.finally(function () {
		    $scope.dataLoading = false;
		});


    $scope.select = function (identifier) {
        $http.get('http://localhost:8090/api/account/' + identifier).
            then(function (response) {
                $scope.selectedaccount = response.data;
            });
    };

    $scope.showpage = function (page) {
        if (page == $scope.currentpage)
            return true;
    };

    $scope.setpage = function (page) {
        $scope.currentpage = page
    };

    $scope.accountclass = 'pure-u-1-1';

    $scope.changeClass = function () {
        if ($scope.accountclass === 'pure-u-1-1')
            $scope.accountclass = 'pure-u-2-3';
        else
            $scope.accountclass = 'pure-u-1-1';
    };

    app.directive('toggleshow', function () {
        return function (scope,element, attrs) {
            scope.$apply(function () {
                scope.$eval(attrs.myEnter);
            });
        }
        });



    app.directive('entersearch', function () {
        return function (scope, element, attrs) {
            element.bind("keydown keypress", function (event) {
                if (event.which === 13) {
                    scope.$apply(function () {
                        scope.$eval(attrs.myEnter);
                    });
                }
                    if (event.which === 27) {
                        scope.showme = false;
                    }
                    event.preventDefault();
                });
            };
    });
});
    //app.directive('directsearch', function () {
    //    return function (scope, element, attrs) {
    //        element.bind("keydown keypress", function (event) {
    //                scope.$apply(function () {
    //                    scope.$eval(attrs.myEnter);
    //                });

    //        });
    //    };
    //});







