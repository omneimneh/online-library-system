import { Component, OnInit, Input } from '@angular/core';
declare var $: any;

@Component({
    selector: 'app-book-root',
    templateUrl: './book-root.component.html',
    styleUrls: ['./book-root.component.css']
})
export class BookRootComponent implements OnInit {

    @Input("book") book: any;
    signedIn: boolean;

    constructor() {
        this.book = JSON.parse($('#book').val());
        console.log(this.book);
        this.signedIn = $('#Token').val() != '';
    }

    openRentModal() {
        $('#rentModal').modal();
        $('#rentBook').val(JSON.stringify(this.book));
        $('#rentBook').click();
    }

    formatDate(date: any) {
        return date;
    }

    ngOnInit() {
    }

}
