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
        '<input class="form-control-plaintext PublisherId" type="hidden" readonly value="' + book.PublsiherId + '" />' +
        '<input class="form-control-plaintext PublisherName" readonly value="' + (book.PublisherName ? book.PublisherName : 'None') + '" />' +
        '</td>' +
        '<td>' +
        '<input class="form-control-plaintext PublishingDate" type="date" value="' + Date.parse(book.PublishingDate).toString('yyyy-MM-dd') + '" />' +
        '</td>' +
        '<td>' +
        '<input class="form-control-plaintext Quantity" type="number" min="0" value="' + book.Quantity + '" />' +
        '</td>' +
        '<td>' +
        '<div class="form-control-plaintext Options">' +
        '<button class="btn btn-default btn-sm upload"><span class="oi oi-image"></span></button>' +
        '</div>' +
        '</td>' +
        '<td>' +
        '<div class="form-control-plaintext Options">' +
        '<button class="btn btn-default btn-sm delete"><span class="oi oi-trash"></span></button>' +
        '</div>' +
        '</td>' +
        '<td>' +
        '<div class="form-control-plaintext Options">' +
        '<button data-toggle="popover" title="save this row" class="btn btn-outline-primary btn-sm save" disabled>Save</button>' +
        '</div>' +
        '</td>' +
        '</tr>';
}