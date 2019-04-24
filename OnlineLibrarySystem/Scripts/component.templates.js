function createBookRow(book) {
    return '<tr data-bookId="' + book.BookId + '">' +
        '<td>' +
        '<input class="form-control-plaintext BookTitle" type="text" value="' + book.BookTitle + '" />' +
        '</td>' +
        '<td>' +
        '<input class="form-control-plaintext BookDescription" type="text" value="' + book.BookDescription + '" />' +
        '</td>' +
        '<td>' +
        '<input class="form-control-plaintext AuthorId" type="hidden" value="' + book.AuthorId + '" />' +
        '<input class="form-control-plaintext AuthorName" readonly value="' + (book.AuthorName ? book.AuthorName : 'None') + '" />' +
        '</td>' +
        '<td>' +
        '<input class="form-control-plaintext PublisherId" type="hidden" readonly value="' + book.PublisherId + '" />' +
        '<input class="form-control-plaintext PublisherName" readonly value="' + (book.PublisherName ? book.PublisherName : 'None') + '" />' +
        '</td>' +
        '<td>' +
        '<input class="form-control-plaintext PublishingDate" type="date" value="' + Date.parse(book.PublishingDate).toString('yyyy-MM-dd') + '" />' +
        '</td>' +
        '<td>' +
        '<input class="form-control-plaintext Quantity" type="number" min="0" value="' + book.Quantity + '" />' +
        '</td>' +
        '<td>' +
        '<div class="form-control-plaintext Upload">' +
        '<input class="hidden ThumbnailImage" type="file" accept="image/jpeg, image/png" />' +
        '<button class="btn btn-default btn-sm upload" data-toggle="popover" data-content="Upload cover image"><span class="oi oi-image"></span></button>' +
        '</div>' +
        '</td>' +
        '<td>' +
        '<div class="form-control-plaintext Delete">' +
        '<button class="btn btn-default btn-sm delete" data-toggle="popover" data-content="Delete this book"><span class="oi oi-trash"></span></button>' +
        '</div>' +
        '</td>' +
        '<td>' +
        '<div class="form-control-plaintext Save">' +
        '<button class="btn btn-outline-primary btn-sm save" data-toggle="popover" data-content="Save updated info" disabled>Save</button>' +
        '</div>' +
        '</td>' +
        '</tr>';
}