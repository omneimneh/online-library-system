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
    $('#confirmCancel').on('click.callback', function () {
        callback(false);
        $('#confirmCancel').off('click.callback');
    });
    $('#confirmOk').on('click.callback', function () {
        callback(true);
        $('#confirmOk').off('click.callback');
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
        var personType = $('#TokenPersonType').val();
        switch (personType) {
            case 'Student':
            case 'Professor':
                $scope.links = [{ name: 'Home', url: '/' }, { name: 'Browse', url: '/Book/Search' }, { name: 'Help', url: '/Help' }];
                break;
            case 'Librarian':
                $scope.links = [{ name: 'Home', url: '/' }, { name: 'Librarian', url: '/Librarian' }, { name: 'Checkout', url: '/Librarian/Checkout' } ];
                break;
            case 'Admin':
                $scope.links = [{ name: 'Manage', url: '/Admin/Manage' }, { name: 'Librarian', url: '/Librarian' }, { name: 'Checkout', url: '/Librarian/Checkout' }];
        }
        if ($('#Token').val()) {
            $scope.userActions = [{ name: 'Profile', url: '/Account/Profile' }, { name: 'Logout', url: '/Account/Logout' }];
        } else {
            $scope.userActions = [{ name: 'Signup', url: '/Account/Signup' }, { name: 'Login', url: '/Account/Login' }];
        }
    });

    // load rent modal ng-controller
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

    // remove somee.com 'hosted by' advertisement
    function __removeAd() {
        $("div[style='opacity: 0.9; z-index: 2147483647; position: fixed; left: 0px; bottom: 0px; height: 65px; right: 0px; display: block; width: 100%; background-color: #202020; margin: 0px; padding: 0px;']").remove();
        $("div[style='margin: 0px; padding: 0px; left: 0px; width: 100%; height: 65px; right: 0px; bottom: 0px; display: block; position: fixed; z-index: 2147483647; opacity: 0.9; background-color: rgb(32, 32, 32);']").remove();
        $("div[onmouseover='S_ssac();']").remove();
        $("center").remove();
        $("div[style='height: 65px;']").remove();
    }

    // on page load this function will run
    $(document).ready(function () {
        initPopovers();
        Loader('hide', 1500);
        __removeAd();
    });

})();
