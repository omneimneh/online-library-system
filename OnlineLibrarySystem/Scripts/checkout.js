var $reservations;
var $search;

function checkout(resId) {
    $.ajax({
        url: 'api/ApiReservation/Checkout',
        type: 'post',
        data: {
            ReservationId: resId,
            Token: $('#Token').val()
        },
        success: function (res) {
            if (res) {
                loadData('');
            } else {
                Alert(_errorSomethingWentWrong, 'Checkout failed!');
            }
        },
        error: _requestError
    });
}

function confirmCheckout(resId) {
    Confirm('Confirmation', 'Are you sure you want to continue?', function (res) {
        if (res) {
            checkout(resId);
        }
    });
}

function getBtn(book) {
    var btn;
    switch (book.Status) {
        case 3:
            btn = '<span class="oi oi-warning text-warning mr-3 pointer" data-placement="top" data-toggle="popover" data-content="Exceeded Deadline!"></span>' +
                '<button class="btn btn-sm btn-outline-danger" onclick="confirmCheckout(' + book.ReservationId + ')">Confirm book returned ' +
                '</button>';
            break;
        case 2:
            btn = '<button class="btn btn-sm btn-outline-info" onclick="confirmCheckout(' + book.ReservationId + ')">Confirm book returned</button>';
            break;
        case 1:
            btn = '<button class="btn btn-sm btn-outline-primary" onclick="confirmCheckout(' + book.ReservationId + ')">Confirm pickup</button>';
            break;
        default:
    }
    return btn;
}

function loadData(key) {
    Loader('show');
    $reservations.html('<tr><td colspan="6" class="text-center"><p class="my-2">Please wait...</p></td></tr>');
    $.ajax({
        url: 'api/ApiReservation/ReservationSearch?token=' + $('#Token').val() + '&key=' + key + '&today=' + $('#today').prop('checked'),
        success: function (result) {
            if (result) {
                $reservations.html('');
                for (var i = 0; i < result.length; i++) {
                    $reservations.append(
                        '<tr>' +
                        '<td>' + result[i].Username + '</td>' +
                        '<td>' + result[i].BookTitle + '</td>' +
                        '<td>' + result[i].PickupDate + '</td>' +
                        '<td>' + result[i].ReturnDate + '</td>' +
                        '<td>' + result[i].Price + '</td>' +
                        '<td>' +
                        getBtn(result[i]) +
                        '</td>' +
                        '</tr>'
                    );
                }
                if (result.length === 0) {
                    $reservations.html('<tr><td colspan="6">No records were found!</td></tr>');
                }
                initPopovers($reservations);
            } else {
                Alert(_errorSomethingWentWrong, 'Could not retrieve records!');
            }
        },
        error: _requestError
    }).done(function () {
        Loader('hide', 500);
    });
}

$(document).ready(function () {
    $reservations = $('#reservations');
    $search = $('#search');
    $search.on('keyup', function () {
        loadData($search.val());
    });
    loadData('');
});