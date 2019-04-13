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
    totalCount: number;
    page: number;
    pageSize: number;
    start: boolean;
    end: boolean;

    constructor(http: Http) {
        var data: String = $('#searchForm').serialize();
        http.get('api/ApiBook/Search?' + data).subscribe(x => {
            var json: any = x.json();
            this.books = json.Results;
            this.totalCount = json.TotalCount;

            this.page = $('#page').val();
            this.pageSize = $('#pageSize').val();
            this.start = this.page == 1;
            this.end = this.page >= (this.totalCount / this.pageSize);
        });
    }

    ngOnInit() {

    }

    goToPage(page: number) {
        $('#page').val(page);
        $('#searchForm').submit();
    }

}
