(function () {

    var authors;
    var publishers;
    var books;
    var totalCount;
    var page = 1;
    var lastPage;
    var pageSize = 20;
    var key = '';

    // load data to be used
    function loadData() {
        var $table = $('#table');

        // clear old content
        $table.html('');
        $('.popover').remove();

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
                $('#nAuthorName')._autocomplete(authors, {
                    selectAction: function (currentContent, $input) {
                        $('#nAuthorId').val(currentContent.value);
                    }
                });
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
                $('#nPublisherName')._autocomplete(publishers, {
                    selectAction: function (currentContent, $input) {
                        $('#nPublisherId').val(currentContent.value);
                    }
                });
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
                currentTr.find('.AuthorName')._autocomplete(authors, {
                    selectAction: function (currentContent, $input) {
                        var $tmp = currentTr.find('.AuthorId');
                        $tmp.val(currentContent.value);
                        $tmp.addClass('changed');
                    },
                    addAction: function () {
                        // add a new author call api
                    }
                });
                currentTr.find('.PublisherName')._autocomplete(publishers, {
                    selectAction: function (currentContent, $input) {
                        var $tmp = currentTr.find('.PublisherId');
                        $tmp.val(currentContent.value);
                        $tmp.addClass('changed');
                    },
                    addAction: function () {
                        // add a new publisher call api
                    }
                });

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
                    });
                });
            })($tr, books[i]);

        }
        // popover hint for upload image, and delete
        initPopovers($('#table'));
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
    }

    function refreshTable() {
        key = $('#search').val();
        page = $('#page').val();
        loadData();
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