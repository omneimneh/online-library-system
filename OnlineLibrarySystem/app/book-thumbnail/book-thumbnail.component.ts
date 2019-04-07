import { Component, OnInit, Input } from '@angular/core';
import { Book, Category } from '../Model';


@Component({
    selector: 'app-book-thumbnail',
    templateUrl: './book-thumbnail.component.html',
    styleUrls: ['./book-thumbnail.component.css']
})
export class BookThumbnailComponent implements OnInit {
    @Input("book") book: Book;

    openDetails() {
        console.log(this.book);
    }

    searchCategory(c: Category) {
        console.log(c);
    }

    constructor() { }

    ngOnInit() {
    }
}