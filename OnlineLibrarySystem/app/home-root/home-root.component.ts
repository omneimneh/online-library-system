import { Component, OnInit } from '@angular/core';
import { appTitle } from '../const';
import { Http } from '@angular/http';
declare var $: any;
declare var navigator: any;

@Component({
    selector: 'app-home-root',
    templateUrl: './home-root.component.html',
    styleUrls: ['./home-root.component.css']
})
export class HomeRootComponent implements OnInit {

    books: any[];
    appTitle: string = appTitle;
    signedIn: boolean;

    constructor(http: Http) {
        http.get('/api/ApiBook/GetMostPopular?count=8').subscribe(books => this.books = books.json());
        this.signedIn = $('#Token').val() != '';
    }

    ngOnInit() {
        $('.carousel').carousel({
            interval: 3000
        });
    }
}
