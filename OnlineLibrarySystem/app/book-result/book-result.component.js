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
var BookResultComponent = /** @class */ (function () {
    function BookResultComponent() {
    }
    BookResultComponent.prototype.ngOnInit = function () {
    };
    __decorate([
        core_1.Input("book"),
        __metadata("design:type", Object)
    ], BookResultComponent.prototype, "book", void 0);
    BookResultComponent = __decorate([
        core_1.Component({
            selector: 'app-book-result',
            templateUrl: './book-result.component.html',
            styleUrls: ['./book-result.component.css']
        }),
        __metadata("design:paramtypes", [])
    ], BookResultComponent);
    return BookResultComponent;
}());
exports.BookResultComponent = BookResultComponent;
//# sourceMappingURL=book-result.component.js.map