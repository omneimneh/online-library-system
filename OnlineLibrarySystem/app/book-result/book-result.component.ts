import { Component, OnInit, Input } from '@angular/core';

@Component({
    selector: 'app-book-result',
    templateUrl: './book-result.component.html',
    styleUrls: ['./book-result.component.css']
})
export class BookResultComponent implements OnInit {

    @Input("book") book: any;

    constructor() { }

    ngOnInit() {
    }

}
