
// create a scope to avoid variable names conflict
// just make a function and call it immediately (function() {})();
(function () {

    var $global = angular.module("global", []);

    // load navbar ng-controller
    $global.controller("navbarController", function ($scope) {
        $scope.appTitle = "Online Library System";
        $scope.categories = ['Science', 'Biology', 'Computer', 'Medicine'];
        $scope.links = [{ name: 'Home', url: '#' }, { name: 'Browse', url: '#' }, { name: 'Help', url: '#' }];
        $scope.userActions = [{ name: 'Signup', url: '#' }, { name: 'Login', url: '#' }];

    });

    // on page load this function will run
    $(document).ready(function () {

    });


})();