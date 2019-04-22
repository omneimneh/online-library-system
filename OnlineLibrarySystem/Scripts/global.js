// global functions

// simple bootstrap modal
function Alert(title, msg) {
    $('#alertTitle').html(title);
    $('#alertMsg').html(msg);
    $('#alertModal').modal();
}

// simple bootstrap confirmation
function Confirm(title, msg, callback) {
    $('#confirmTitle').html(title);
    $('#confirmMsg').html(msg);
    $('#confirmCancel').click(function () {
        callback(false);
    });
    $('#confirmOk').click(function () {
        callback(true);
    });
    $('#confirmModal').modal();
}

// enables bootstrap popovers
function initPopovers($parent) {
    // find popovers inside $parent or inside document
    var $popovers;
    if (typeof $parent === "object") {
        $popovers = $parent.find('[data-toggle="popover"]').not('[data-html]');
    } else {
        $popovers = $('[data-toggle="popover"]').not('[data-html]');
    }
    // enable bootstrap popovers on hover
    $popovers.mouseover(function () {
        $(this).popover('show');
    });
    $popovers.on('click mouseout', function () {
        $(this).popover('hide');
    });
    // enable bootstrap popovers on click
    $('[data-toggle="popover"][data-html="true"]').popover({ html: true });
}

// create a scope to avoid variable names conflict
// just make a function and call it immediately (function() {})();
(function () {

    // load navbar ng-controller
    _global.controller("navbarController", function ($scope) {
        $scope.appTitle = _appTitle;
        $scope.links = [{ name: 'Home', url: '/' }, { name: 'Browse', url: '/Book/Search' }, { name: 'Help', url: '/Help' }];
        if ($('#Token').val()) {
            $scope.userActions = [{ name: 'Profile', url: '/Account/Profile' }, { name: 'Logout', url: '/Account/Logout' }];
        } else {
            $scope.userActions = [{ name: 'Signup', url: '/Account/Signup' }, { name: 'Login', url: '/Account/Login' }];
        }
    });

    _global.controller("rentModalController", function ($scope) {

        $scope.book = {};

        $scope.getReturnDate = function () {
            if ($scope.book.orderDate) {
                var date = Date.parse($scope.book.orderDate.toString('MM/dd/yyyy'));
                date.addDays(14);
                return date.toString('MM/dd/yyyy');
            }
        };
        $scope.refreshForm = function () {
            $scope.book = JSON.parse($('#rentBook').val());
            $scope.book.orderDate = new Date();
        };
        $scope.removeWarning = function () {
            var date = $scope.book.orderDate;
            if (date > Date.today().addDays(-1) && date <= Date.today().addDays(7)) {
                $('#orderDate').removeClass('is-invalid');
            } else {
                $('#orderDate').addClass('is-invalid');
            }
        };

        $('#rentSubmit').click(function () {
            var date = $scope.book.orderDate;

            if (date < Date.today().addDays(-1) || date > Date.today().addDays(7)) {
                return;
            }

            $.ajax({
                url: '/api/ApiBook/Rent',
                type: 'POST',
                data: {
                    Token: $('#Token').val(),
                    BookId: $scope.book.BookId,
                    PickupDateStr: $scope.book.orderDate.toString('MM/dd/yyyy')
                },
                success: function (res) {
                    if (res) {
                        window.location.href = '/Account/Profile#orders';
                    } else {
                        $('#rentModal').modal('hide');
                        $('#rentFailed').modal();
                    }
                },
                error: function () {
                    console.log('error');
                    $('#rentModal').modal('hide');
                    $('#rentFailed').modal();
                }
            });
        });
    });

    // on page load this function will run
    $(document).ready(function () {
        initPopovers();
    });

})();