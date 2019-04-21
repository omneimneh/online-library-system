jQuery.fn.extend({
    // jquery has an autocomplete component but we need a custom one
    // so we called it "_autocomplete" to avoid conflict in case we needed to use both
    _autocomplete: function (contentData, options) {
        var $this = $(this);
        var dataSnap = Array(contentData.length);
        var i;

        for (i = 0; i < contentData.length; i++) {
            dataSnap[i] = {
                data: contentData[i].data,
                value: contentData[i].value
            };
        }

        var defaultOptions = {
            enableSearch: true,
            enableAdd: true,
            filter: function (searchKey, currentContent) {
                return currentContent.data.toLowerCase().includes(searchKey.toLowerCase());
            },
            addAction: function () { },
            selectAction: function () { }
        };

        if (!options) {
            options = defaultOptions;
        } else {
            Object.keys(defaultOptions).forEach(function (key) {
                if (!options[key]) {
                    options[key] = defaultOptions[key];
                }
            });
        }

        var placeholder = $this.attr('data-placeholder') ? $this.attr('data-placeholder') : 'Type a value';

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
            var $elem = $('<div class="autocomplete-item" data-value="' + dataSnap[i].value + '">'
                + dataSnap[i].data + '</div>');
            (function (currentContent) {
                $elem.click(function () {
                    options.selectAction();
                    $this.val(currentContent.data);
                    $this.change();
                    $html.fadeOut('fast');
                    $search.val('');
                    $search.keyup();
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
            options.addAction();
            $this.val($search.val());
            $html.fadeOut('fast');
            $search.val('');
            $search.keyup();
        });

        $(window).click(function () {
            $search.val('');
            $search.keyup();
            $html.fadeOut('fast');
        });

        $html.click(function (event) {
            event.stopPropagation();
        });

        $this.after($html);
        $this.click(function (event) {
            $('.autocomplete-container').fadeOut('fast');
            $html.fadeIn('fast');
            $html.removeClass('hidden');
            event.stopPropagation();
        });
    }
});