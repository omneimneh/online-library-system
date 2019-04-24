jQuery.fn.extend({
    // jquery has an autocomplete component but we need a custom one
    // so we called it "_autocomplete" to avoid conflict in case we needed to use both
    // don't use it for repeated data, it's slow for large data, use "_attachAutocomplete" instead!
    _autocomplete: function (contentData, options) {
        var auto = new _Autocomplete(contentData, options);
        $(this)._attachAutocomplete(auto);
    },
    // attach an extisting "_Autocomplete" for an input
    _attachAutocomplete: function (_object, options) {
        $(this).off('click._autocomplete');
        $(this).on('click._autocomplete', function (event) {
            event.stopPropagation();
            _object.show($(this), options);
        });
    }
});

function _Autocomplete(contentData, options) {
    var $this;
    var i;
    var dataSnap;
    var defaultOptions = {
        enableSearch: true,
        enableAdd: true,
        filter: function (searchKey, currentContent) {
            return currentContent.data.toLowerCase().includes(searchKey.toLowerCase());
        },
        addAction: function () { },
        selectAction: function () { },
        deleteAction: function () { }
    };

    dataSnap = Array(contentData.length);
    for (i = 0; i < contentData.length; i++) {
        dataSnap[i] = {
            data: contentData[i].data,
            value: contentData[i].value
        };
    }

    function initOptions(opts) {
        if (!opts) {
            opts = defaultOptions;
        } else {
            Object.keys(defaultOptions).forEach(function (key) {
                if (!opts[key]) {
                    opts[key] = defaultOptions[key];
                }
            });
        }
        options = opts;
    }
    initOptions(options);

    var placeholder = 'Type a value';

    var $html = $(
        '<div class="autocomplete-container hidden">' +
        '<div class="form-group inner-addon-sm right-addon">' +
        '<input class="form-control form-control-sm autocomplete-search" placeholder="' + placeholder + '" />' +
        '<span class="oi oi-magnifying-glass glyphicon"></span>' +
        '</div>' +
        '<ul class="autocomplete-list m-0">' +
        '</ul>' +
        '</div>'
    );
    var $new = $(
        '<li class="autocomplete-separator border-top my-2"></li>' +
        '<li class="autocomplete-new">' +
        '<div class="autocomplete-add">Add new value</div>' +
        '<span class="oi oi-plus glyphicon"></span>' +
        '</li>'
    );

    var $list = $html.find('.autocomplete-list');
    var $search = $html.find('.autocomplete-search');
    var $add = $new.find('.autocomplete-add');

    for (i = 0; i < dataSnap.length; i++) {
        var $elem = $('<li class="row m-0 p-0" data-value="' + dataSnap[i].value + '">' +
            '<div class="col autocomplete-item">' + dataSnap[i].data + '</div>' +
            '<div class="col-auto p-1 m-0"><button class="btn btn-default btn-sm px-1 autocomplete-delete"><span class="oi oi-trash"></span></button></div>' +
            '</li>');
        (function (currentContent) {
            $elem.find('.autocomplete-item').click(function () {
                options.selectAction(currentContent, $this);
                $this.val(currentContent.data);
                $this.change();
                $html.fadeOut('fast');
                $search.val('');
                $search.keyup();
            });
            $elem.find('.autocomplete-delete').click(function () {
                options.deleteAction(currentContent, $this);
            });
        })(dataSnap[i]);
        $list.append($elem);
        dataSnap[i].$elem = $elem;
    }
    $list.append($new);

    $new.hide();
    if (options.enableSearch) {
        $search.on('keyup change', function () {
            var searchKey = $(this).val();
            if (searchKey === '') {
                for (i = 0; i < dataSnap.length; i++) {
                    dataSnap[i].$elem.show();
                }
            } else {
                for (i = 0; i < dataSnap.length; i++) {
                    if (options.filter(searchKey, dataSnap[i])) {
                        dataSnap[i].$elem.show();
                    } else {
                        dataSnap[i].$elem.hide();
                    }
                }
            }
            $new.hide();
            if (options.enableAdd && searchKey !== '') {
                $new.show();
                $add.html('Add new value <b>"' + searchKey + '"</b>');
            }
        });
    } else {
        $search.hide();
    }

    $new.click(function () {
        options.addAction($search.val(), $this);
        $this.val($search.val());
        $this.change();
        $html.fadeOut('fast');
        $search.val('');
        $search.keyup();
    });

    $html.click(function (event) {
        event.stopPropagation();
    });

    $(window).click(function () {
        $search.val('');
        $search.keyup();
        $html.fadeOut('fast');
    });

    function init($input) {
        $this = $input;
        $this.after($html);
    }

    return {
        show: function ($input, reOptions) {
            init($input);
            $('.autocomplete-container').fadeOut('fast');
            $html.fadeIn('fast');
            $html.removeClass('hidden');
            if (reOptions !== undefined) {
                initOptions(reOptions);
            }
        },
        remove: function () {
            $html.remove();
        }
    };
}