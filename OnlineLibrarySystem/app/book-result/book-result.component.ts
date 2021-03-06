import { Component, OnInit, Input } from '@angular/core';
declare var $: any;

@Component({
    selector: 'app-book-result',
    templateUrl: './book-result.component.html',
    styleUrls: ['./book-result.component.css']
})
export class BookResultComponent implements OnInit {

    @Input("book") book: any;
    signedIn: boolean;

    constructor() {
        this.signedIn = $('#Token').val() != '';
    }

    openRentModal() {
        $('#rentModal').modal();
        $('#rentBook').val(JSON.stringify(this.book));
        $('#rentBook').click();
    }

    getBackground() {
        if (this.book.ThumbnailImage) {
            return "url('" + this.book.ThumbnailImage + "')";
        }
        return "url('/Media/default.png')";
    }

    ngOnInit() {
    }

}
