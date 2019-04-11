import { Component, OnInit, Input } from '@angular/core';

@Component({
    selector: 'app-book-thumbnail',
    templateUrl: './book-thumbnail.component.html',
    styleUrls: ['./book-thumbnail.component.css']
})
export class BookThumbnailComponent implements OnInit {
    @Input("book") book: any;

    openDetails() {
        console.log(this.book);
    }

    constructor() { }

    ngOnInit() {
    }
}