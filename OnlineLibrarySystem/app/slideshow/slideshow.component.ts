import { Component, OnInit } from '@angular/core';

const bigCaptions = [
    'A READER LIVES A THOUSAND LIVES BEFORE HE DIES... THE MAN WHO NEVER READS LIVES ONLY ONE',
    'PLUNGE INTO THE WONDERFUL WORLD OF READING',
    'TODAY A READER, TOMORROW A LEADER'
];

const subCaptions = [
    'George R.R. Martin',
    'Find the book you need with one click!',
    'Margaret Fuller'
];

class Slide {
    bigCaption: string;
    subCaption: string;
    imageUrl: string;

    constructor(bigCap: string, subCap: string, imgUrl: string) {
        this.bigCaption = bigCap;
        this.subCaption = subCap;
        this.imageUrl = imgUrl;
    }
}


@Component({
    selector: 'app-slideshow',
    templateUrl: './slideshow.component.html',
    styleUrls: ['./slideshow.component.css']
})
class SlideshowComponent implements OnInit {

    slides: Slide[];

    constructor() {
        this.slides = [
            new Slide(bigCaptions[0], subCaptions[0], '/Content/img/slideshow_1.jpg'),
            new Slide(bigCaptions[1], subCaptions[1], '/Content/img/slideshow_2.jpg'),
            new Slide(bigCaptions[2], subCaptions[2], '/Content/img/slideshow_3.jpg')
        ];
    }

    ngOnInit() {
    }

}
export { Slide, SlideshowComponent }