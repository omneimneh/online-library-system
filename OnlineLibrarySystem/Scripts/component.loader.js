(function () {
    function parseSpeed(speed) {
        var _speed = 400;
        if (typeof speed === 'number') {
            _speed = speed;
        } else if (typeof speed === 'string') {
            switch (speed) {
                case 'slow':
                    _speed = 200;
                    break;
                case 'fast':
                    _speed = 600;
                    break;
            }
        }
        return _speed;
    }
    jQuery.fn.extend({
        _fadeOut: function (speed) {
            $(this).animate({ opacity: 0 }, parseSpeed(speed));
        },
        _fadeIn: function (speed) {
            $(this).animate({ opacity: 1 }, parseSpeed(speed));
        }
    });
})();

function Loader(show, after) {
    if (show === 'hide') {
        setTimeout(function () {
            $('#loader')._fadeOut();
        }, after ? after : 0);
    } else {
        $('#loader')._fadeIn();
    }
}