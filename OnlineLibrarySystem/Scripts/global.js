
// create a scope to avoid variable names conflict
// just make a function and call it immediately (function() {})();
(function () {

    var $global = angular.module("global", []);

    // load navbar ng-controller
    $global.controller("navbarController", function ($scope) {
        $scope.appTitle = "Online Library System";
        $scope.links = [{ name: 'Home', url: '/' }, { name: 'Browse', url: '/Book/Search' }, { name: 'Help', url: '/Help' }];
        if ($('#Token').val()) {
            $scope.userActions = [{ name: 'Logout', url: '/Account/Logout' }];
        } else {
            $scope.userActions = [{ name: 'Signup', url: '/Account/Signup' }, { name: 'Login', url: '/Account/Login' }];
        }
    });

    // on page load this function will run
    $(document).ready(function () {

    });


})();