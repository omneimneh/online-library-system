"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var Model_1 = require("./Model");
var someCategories = [
    new Model_1.Category('math'), new Model_1.Category('computer-science'), new Model_1.Category('medicine')
];
exports.someCategories = someCategories;
var someBooks = [
    new Model_1.Book('MVC web apps', 'John Smith', someCategories),
    new Model_1.Book('Get over it', 'virus.exe', someCategories),
    new Model_1.Book('Angular', 'Google', someCategories),
    new Model_1.Book('React js', 'Facebook', someCategories),
    new Model_1.Book('AWS', 'Amazon', someCategories),
    new Model_1.Book('Intro to physics', 'Newton', someCategories)
];
exports.someBooks = someBooks;
var appTitle = "Online Library System";
exports.appTitle = appTitle;
//# sourceMappingURL=const.js.map