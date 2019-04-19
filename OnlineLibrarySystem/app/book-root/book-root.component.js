"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var BookRootComponent = /** @class */ (function () {
    function BookRootComponent() {
        this.book = JSON.parse($('#book').val());
        console.log(this.book);
        this.signedIn = $('#Token').val() != '';
    }
    BookRootComponent.prototype.openRentModal = function () {
        $('#rentModal').modal();
        $('#rentBook').val(JSON.stringify(this.book));
        $('#rentBook').click();
    };
    BookRootComponent.prototype.formatDate = function (date) {
        return date;
    };
    BookRootComponent.prototype.ngOnInit = function () {
    };
    __decorate([
        core_1.Input("book"),
        __metadata("design:type", Object)
    ], BookRootComponent.prototype, "book", void 0);
    BookRootComponent = __decorate([
        core_1.Component({
            selector: 'app-book-root',
            templateUrl: './book-root.component.html',
            styleUrls: ['./book-root.component.css']
        }),
        __metadata("design:paramtypes", [])
    ], BookRootComponent);
    return BookRootComponent;
}());
exports.BookRootComponent = BookRootComponent;
//# sourceMappingURL=book-root.component.js.map