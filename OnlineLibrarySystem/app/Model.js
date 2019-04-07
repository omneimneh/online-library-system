"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var Category = /** @class */ (function () {
    function Category(name) {
        this.name = name;
    }
    return Category;
}());
exports.Category = Category;
var Book = /** @class */ (function () {
    function Book(title, author, categories) {
        this.author = author;
        this.title = title;
        this.categories = categories;
    }
    return Book;
}());
exports.Book = Book;
//# sourceMappingURL=Model.js.map