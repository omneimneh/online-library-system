(function () {

    var authors;
    var publishers;
    var autoAuthors;
    var autoPublishers;
    var books;
    var totalCount;
    var page = 1;
    var lastPage;
    var pageSize = 20;
    var key = '';

    // load data to be used
    function loadData() {
        // show loader
        Loader('show');
        $('#table').html('<tr><td colspan="9" class="text-center"><p class="my-3">Please wait...</p></td></tr>');

        // load authors
        var a1 = $.ajax({
            url: 'api/ApiBook/GetAuthors',
            success: function (res) {
                authors = [{ value: -1, data: 'None' }];
                for (var i = 0; i < res.length; i++) {
                    authors.push({
                        value: res[i].AuthorId,
                        data: res[i].AuthorName
                    });
                }
                initAutoAuthorsAdd();
            }
        });
        // load publishers
        var a2 = $.ajax({
            url: 'api/ApiBook/GetPublishers',
            success: function (res) {
                publishers = [{ value: -1, data: 'None' }];
                for (var i = 0; i < res.length; i++) {
                    publishers.push({
                        value: res[i].PublisherId,
                        data: res[i].PublisherName
                    });
                }
                initAutoPublishersAdd();
            }
        });
        // load books
        var a3 = $.ajax({
            url: 'api/ApiBook/LibrarianSearch',
            data: {
                key: key,
                page: page,
                pageSize: pageSize
            },
            success: function (res) {
                books = res.Results;
                totalCount = res.TotalCount;
            }
        });
        // after all ajax calls are done
        $.when(a1, a2, a3).then(fillData);
    }

    function deleteAuthor(currentContent, $input) {
        if (currentContent.value === -1) return;
        Confirm(_confirmation, 'Are you sure you want to delete <b>' + currentContent.data + '</b>?', function (confirm) {
            if (confirm) {
                $.ajax({
                    url: 'api/ApiBook/DeleteAuthor',
                    data: {
                        Token: $('#Token').val(),
                        AuthorId: currentContent.value
                    },
                    type: 'post',
                    success: function (res) {
                        if (res) {
                            loadData();
                        } else {
                            Alert(_errorContactAdmin, 'Delete author failed!');
                        }
                    },
                    error: _requestError
                });
            }
        });
    }

    function deletePublisher(currentContent, $input) {
        if (currentContent.value === -1) return;
        Confirm(_confirmation, 'Are you sure you want to delete <b>' + currentContent.data + '</b>?', function (confirm) {
            if (confirm) {
                $.ajax({
                    url: 'api/ApiBook/DeletePublisher',
                    type: 'post',
                    data: {
                        Token: $('#Token').val(),
                        PublisherId: currentContent.value
                    },
                    success: function (res) {
                        if (res) {
                            loadData();
                        } else {
                            Alert(_errorContactAdmin, 'Delete publisher failed!');
                        }
                    },
                    error: _requestError
                });
            }
        });
    }

    // fill books in table
    function fillData() {
        var $table = $('#table');

        // clear old content
        $table.html('');

        // transform float to closest greater than or equal integer
        // ex: 0.6 | 1 = 1, 2.0 | 1 = 2 etc...
        lastPage = totalCount / pageSize | 1;
        $('#page').attr('max', lastPage);
        $('#outOf').html(lastPage);

        // disable pagination next or prev case current page is first or last page
        if (page <= 1) $('#prev').attr('disabled', true);
        else $('#prev').attr('disabled', null);
        if (page >= lastPage) $('#next').attr('disabled', true);
        else $('#next').attr('disabled', null);

        for (var i = 0; i < books.length; i++) {

            // add a table row
            var $tr = $(createBookRow(books[i]));
            $table.append($tr);
            // reference the row in the books array
            books[i].$elem = $tr;

            // pass a copy of $tr to this inner function and call instantly
            (function (currentTr, currentBook) {
                // activate autocomplete custom component
                initAutoAuthors(currentTr, currentBook);
                initAutoPublishers(currentTr, currentBook);

                // handle any change in inputs
                currentTr.find('.form-control-plaintext').on('change keyup', function () {
                    // this input has changed, send this to the server when save is clicked
                    $(this).addClass('changed');
                    currentTr.find('.save').attr('disabled', null);
                    // disable other rows, focus on this one
                    for (var j = 0; j < books.length; j++) {
                        if (books[j].BookId !== currentBook.BookId) {
                            books[j].$elem.css('opacity', 0.5);
                            books[j].$elem.find('input').attr('disabled', true);
                        }
                    }
                });

                // handle delete action
                currentTr.find('.delete').click(function () {
                    Confirm(
                        'This action cannot be undone',
                        'Are you sure you want to remove <b>' + currentBook.BookTitle + '</b>?',
                        function (confirmed) {
                            if (confirmed) {
                                Loader('show');
                                $.ajax({
                                    url: 'api/ApiBook/Delete',
                                    type: "post",
                                    data: {
                                        BookId: currentBook.BookId,
                                        Token: $('#Token').val()
                                    },
                                    success: function (res) {
                                        if (res) {
                                            loadData();
                                        } else {
                                            Alert(_errorSomethingWentWrong, 'Please try again');
                                        }
                                    },
                                    error: _requestError
                                }).done(function () {
                                    Loader('hide', 500);
                                });
                            }
                        }
                    );
                });

                // handle image upload
                currentTr.find('.upload').click(function () {
                    currentTr.find('.ThumbnailImage').click();
                });
                currentTr.find('.ThumbnailImage').change(function () {
                    var formData = new FormData();
                    formData.append(this.files[0].name, this.files[0]);
                    Loader('show');
                    $.ajax({
                        url: 'api/ApiBook/UpdateImage?token=' + $('#Token').val() + '&bookId=' + currentBook.BookId,
                        type: 'post',
                        data: formData,
                        processData: false,
                        contentType: false,
                        success: function (res) {
                            if (res) {
                                Alert('Sucess', 'Cover image was updated successfully');
                                currentTr.find('.save').attr('disabled', true);
                                for (var j = 0; j < books.length; j++) {
                                    if (books[j].BookId !== currentBook.BookId) {
                                        books[j].$elem.css('opacity', 1);
                                        books[j].$elem.find('input').attr('disabled', false);
                                    }
                                }
                            } else {
                                Alert(_errorSomethingWentWrong, 'Upload failed');
                            }
                        },
                        error: _requestError
                    }).done(function () {
                        Loader('hide', 500);
                    });
                });

                // handle save action
                currentTr.find('.save').click(function () {
                    var $BookTitle = currentTr.find('.BookTitle');
                    var $BookDescription = currentTr.find('.BookDescription');
                    var $AuthorId = currentTr.find('.AuthorId');
                    var $PublisherId = currentTr.find('.PublisherId');
                    var $PublishingDate = currentTr.find('.PublishingDate');
                    var $Quantity = currentTr.find('.Quantity');
                    function getChangedData($dom) {
                        return $dom.hasClass('changed') ? $dom.val() : null;
                    }
                    Loader('show');
                    $.ajax({
                        url: 'api/ApiBook/UpdateBook',
                        type: "post",
                        data: {
                            BookId: currentTr.attr('data-bookId'),
                            BookTitle: getChangedData($BookTitle),
                            BookDescription: getChangedData($BookDescription),
                            AuthorId: getChangedData($AuthorId),
                            PublisherId: getChangedData($PublisherId),
                            PublishingDate: getChangedData($PublishingDate),
                            Quantity: getChangedData($Quantity),
                            Token: $('#Token').val()
                        },
                        success: function (res) {
                            if (res) {
                                loadData();
                            } else {
                                Alert(_errorSomethingWentWrong, 'Make sure your data is valid!');
                            }
                        },
                        error: _requestError
                    }).done(function () {
                        Loader('hide', 500);
                    });
                });
            })($tr, books[i]);

        }
        // popover hint for upload image, and delete
        initPopovers($('#table'));

        // dismiss loader
        Loader('hide', 500);
    }

    function initSearch() {
        $('#search').on('keyup', refreshTable);
        $('#page').on('change keyup', refreshTable);
        $('#next').click(function () {
            $('#page')[0].value++;
            refreshTable();
        });
        $('#prev').click(function () {
            $('#page')[0].value--;
            refreshTable();
        });
        $('#reload').click(loadData);
    }

    function refreshTable() {
        key = $('#search').val();
        page = $('#page').val();
        loadData();
    }

    // setup autocomplete popup in table
    function initAutoAuthors(currentTr, currentBook) {
        currentTr.find('.AuthorName')._attachAutocomplete(autoAuthors, {
            selectAction: function (currentContent, $input) {
                var $tmp = currentTr.find('.AuthorId');
                $tmp.val(currentContent.value);
                $tmp.addClass('changed');
            },
            addAction: function (newVal) {
                Loader('show');
                $.ajax({
                    url: 'api/ApiBook/AddAuthor',
                    type: 'post',
                    data: {
                        Token: $('#Token').val(),
                        AuthorName: newVal
                    },
                    success: function (res) {
                        if (res !== -1) {
                            currentTr.find('.AuthorId').val(res);
                            currentTr.find('.AuthorId').change();
                            var addedAuthor = { data: newVal, value: res };
                            authors.push(addedAuthor);
                            currentTr.find('.AuthorName')._autocomplete('remove');
                            initAutoAuthors(currentTr, currentBook);
                        } else {
                            Alert(_errorSomethingWentWrong, 'Failed to add author');
                        }
                    },
                    error: _requestError
                }).done(function () {
                    Loader('hide', 500);
                });
            },
            deleteAction: deleteAuthor
        });
    }

    // setup autocomplete popup in table
    function initAutoPublishers(currentTr, currentBook) {
        currentTr.find('.PublisherName')._attachAutocomplete(autoPublishers, {
            selectAction: function (currentContent, $input) {
                var $tmp = currentTr.find('.PublisherId');
                $tmp.val(currentContent.value);
                $tmp.addClass('changed');
            },
            addAction: function (newVal) {
                Loader('show');
                $.ajax({
                    url: 'api/ApiBook/AddPublisher',
                    type: 'post',
                    data: {
                        Token: $('#Token').val(),
                        PublisherName: newVal
                    },
                    success: function (res) {
                        if (res !== -1) {
                            currentTr.find('.PublisherId').val(res);
                            currentTr.find('.PublisherId').change();
                            var addedPublisher = { data: newVal, value: res };
                            publishers.push(addedPublisher);
                            currentTr.find('.PublisherName')._autocomplete('remove');
                            initAutoPublishers(currentTr, currentBook);
                        } else {
                            Alert(_errorSomethingWentWrong, 'Failed to add publisher');
                        }
                    },
                    error: _requestError
                }).done(function () {
                    Loader('hide', 500);
                });
            },
            deleteAction: deletePublisher
        });
    }

    // setup autocomplete popup in add book modal
    function initAutoAuthorsAdd() {
        autoAuthors = new _Autocomplete(authors);
        $('#nAuthorName')._attachAutocomplete(autoAuthors, {
            selectAction: function (currentContent, $input) {
                $('#nAuthorId').val(currentContent.value);
            },
            addAction: function (newVal) {
                Loader('show');
                $.ajax({
                    url: 'api/ApiBook/AddAuthor',
                    type: 'post',
                    data: {
                        Token: $('#Token').val(),
                        AuthorName: newVal
                    },
                    success: function (res) {
                        if (res !== -1) {
                            $('#nAuthorId').val(res);
                            var addedAuthor = { data: newVal, value: res };
                            authors.push(addedAuthor);
                            $('#nAuthorName')._autocomplete('remove');
                            initAutoAuthorsAdd();
                        } else {
                            Alert(_errorSomethingWentWrong, 'Failed to add author');
                        }
                    },
                    error: _requestError
                }).done(function () {
                    Loader('hide', 500);
                });
            },
            deleteAction: deleteAuthor
        });
    }

    // setup autocomplete popup in add book modal
    function initAutoPublishersAdd() {
        autoPublishers = new _Autocomplete(publishers);
        $('#nPublisherName')._attachAutocomplete(autoPublishers, {
            selectAction: function (currentContent, $input) {
                $('#nPublisherId').val(currentContent.value);
            },
            addAction: function (newVal) {
                Loader('show');
                $.ajax({
                    url: 'api/ApiBook/AddPublisher',
                    type: 'post',
                    data: {
                        Token: $('#Token').val(),
                        PublisherName: newVal
                    },
                    success: function (res) {
                        if (res !== -1) {
                            $('#nPublisherId').val(res);
                            var addedPublisher = { data: newVal, value: res };
                            publishers.push(addedPublisher);
                            $('#nPublisherName')._autocomplete('remove');
                            initAutoPublishersAdd();
                        } else {
                            Alert(_errorSomethingWentWrong, 'Failed to add publisher');
                        }
                    },
                    error: _requestError
                }).done(function () {
                    Loader('hide', 500);
                });
            },
            deleteAction: deletePublisher
        });
    }

    function initAddNewBook() {

        function isValid(val) {
            return val !== null && val !== "";
        }

        function removeError() {
            $(this).removeClass('is-invalid');
        }

        var $BookTitle = $('#nBookTitle');
        var $BookDescription = $('#nBookDescription');
        var $AuthorId = $('#nAuthorId');
        var $AuthorName = $('#nAuthorName');
        var $PublisherId = $('#nPublisherId');
        var $PublisherName = $('#nPublisherName');
        var $PublishingDate = $('#nPublishingDate');
        var $ThumbnailImage = $('#nThumbnailImage');

        $BookTitle.on('change keyup', removeError);
        $AuthorName.on('change keyup', removeError);
        $PublisherName.on('change keyup', removeError);
        $PublishingDate.on('change keyup', removeError);

        $('#nThumbnailImage').change(function () {
            var file = this.files[0];
            var reader = new FileReader();
            reader.onloadend = function () {
                $('#nImageDisplay')[0].style.backgroundImage = "url(" + reader.result + ")";
            };
            if (file) {
                reader.readAsDataURL(file);
            } else {
                $('#nImageDisplay')[0].style.backgroundImage = "none";
            }
        });

        $('#nBookReset').click(function () {
            $('#nBookForm').trigger("reset");
            $('#nThumbnailImage').change();
        });

        $('#nSubmit').click(function () {
            // verify if data is valid
            var valid = true;
            if (!isValid($BookTitle.val())) {
                $BookTitle.addClass('is-invalid');
                valid = false;
            }
            // notice $AuthorId is hidden displaying error on AuthorName
            if (!isValid($AuthorId.val())) {
                $AuthorName.addClass('is-invalid');
                valid = false;
            }
            // notice $PublisherId is hidden displaying error on PublisherName
            if (!isValid($PublisherId.val())) {
                $PublisherName.addClass('is-invalid');
                valid = false;
            }
            if (!isValid($PublishingDate.val())) {
                $PublishingDate.addClass('is-invalid');
                valid = false;
            }

            // if user uploaded a cover image
            var formData = new FormData();
            if ($ThumbnailImage[0].files.length > 0 && $ThumbnailImage[0].files[0]) {
                formData.append($ThumbnailImage[0].files[0].name, $ThumbnailImage[0].files[0]);
            }

            if (valid) {
                Loader('show');
                $.ajax({
                    url: 'api/ApiBook/AddBook?Token=' + $('#Token').val() + '&' + $('#nBookForm').serialize(),
                    type: 'post',
                    processData: false,
                    contentType: false,
                    data: formData,
                    success: function (res) {
                        if (res) {
                            $('#addBookModal').modal('hide');
                            $('#nBookForm').trigger("reset");
                            loadData();
                        } else {
                            Alert(_errorSomethingWentWrong, 'Please try again');
                        }
                    },
                    error: _requestError
                }).done(function () {
                    Loader('hide', 500);
                });
            }

        });
    }

    $(document).ready(function () {
        loadData();
        initSearch();
        initAddNewBook();
    });

})();