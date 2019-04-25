/* constant angular modules */
var _global = angular.module("global", []);

/* constant strings */
var _appTitle = 'Online Library System';
var _errorSomethingWentWrong = 'Oops! Something went wrong';
var _errorContactAdmin = 'Please contact your administrator.';
var _confirmation = 'Confirmation';

/* constant actions */
var _requestError = function () { Alert(_errorSomethingWentWrong, _errorContactAdmin); };
var _getPersonType = function (ptype) {
    switch (ptype) {
        case 0:
            return 'Student';
        case 1:
            return 'Professor';
        case 2:
            return 'Librarian';
        case 3:
            return 'Admin';
    }
}