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
var bigCaptions = [
    'A READER LIVES A THOUSAND LIVES BEFORE HE DIES... THE MAN WHO NEVER READS LIVES ONLY ONE',
    'PLUNGE INTO THE WONDERFUL WORLD OF READING',
    'TODAY A READER, TOMORROW A LEADER'
];
var subCaptions = [
    'George R.R. Martin',
    'Find the book you need with one click!',
    'Margaret Fuller'
];
var Slide = /** @class */ (function () {
    function Slide(bigCap, subCap, imgUrl) {
        this.bigCaption = bigCap;
        this.subCaption = subCap;
        this.imageUrl = imgUrl;
    }
    return Slide;
}());
exports.Slide = Slide;
var SlideshowComponent = /** @class */ (function () {
    function SlideshowComponent() {
        this.slides = [
            new Slide(bigCaptions[0], subCaptions[0], '/Content/img/slideshow_1.jpg'),
            new Slide(bigCaptions[1], subCaptions[1], '/Content/img/slideshow_2.jpg'),
            new Slide(bigCaptions[2], subCaptions[2], '/Content/img/slideshow_3.jpg')
        ];
    }
    SlideshowComponent.prototype.ngOnInit = function () {
    };
    SlideshowComponent = __decorate([
        core_1.Component({
            selector: 'app-slideshow',
            templateUrl: './slideshow.component.html',
            styleUrls: ['./slideshow.component.css']
        }),
        __metadata("design:paramtypes", [])
    ], SlideshowComponent);
    return SlideshowComponent;
}());
exports.SlideshowComponent = SlideshowComponent;
//# sourceMappingURL=slideshow.component.js.map