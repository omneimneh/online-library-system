import { Component, OnInit, Input } from '@angular/core';
declare var $: any;

@Component({
    selector: 'app-book-thumbnail',
    templateUrl: './book-thumbnail.component.html',
    styleUrls: ['./book-thumbnail.component.css']
})
export class BookThumbnailComponent implements OnInit {
    @Input("book") book: any;
    signedIn: boolean;

    openRentModal() {
        if (!this.signedIn || this.book.Quantity < 1) return;
        $('#rentModal').modal();
        $('#rentBook').val(JSON.stringify(this.book));
        $('#rentBook').click();
    }

    constructor() {
        this.signedIn = $('#Token').val() != '';
    }

    ngOnInit() {
    }
}