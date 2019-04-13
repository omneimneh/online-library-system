import { Component, OnInit } from '@angular/core';
import { Http } from '@angular/http';
declare var $: any;

@Component({
    selector: 'app-search-root',
    templateUrl: './search-root.component.html',
    styleUrls: ['./search-root.component.css']
})
export class SearchRootComponent implements OnInit {

    books: any[] = [];
    len: number;

    constructor(http: Http) {
        this.len = 8;
        http.get('api/ApiBook/GetMostPopular?count=8').subscribe(x => this.books = x.json());
    }

    ngOnInit() {
    }

}
