import { Component, OnInit } from '@angular/core';
import { Http } from '@angular/http';
declare var $: any;

@Component({
    selector: 'app-order-root',
    templateUrl: './order-root.component.html',
    styleUrls: ['./order-root.component.css']
})
export class OrderRootComponent implements OnInit {

    orders: any[];

    constructor(http: Http) {
        http.get('api/AccountApi/GetPersonOrders?token=' + $('#Token').val() + '&count=8')
            .subscribe(orders => this.orders = orders.json());
    }

    ngOnInit() {
    }

}
