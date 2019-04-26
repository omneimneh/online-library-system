import { Component, OnInit, Input } from '@angular/core';

@Component({
    selector: 'app-order-item',
    templateUrl: './order-item.component.html',
    styleUrls: ['./order-item.component.css']
})
export class OrderItemComponent implements OnInit {

    @Input("order") order: any;

    constructor() {
    }

    badgeClass() {
        switch (this.order.Status) {
            case 0:
                return 'badge-success';
            case 1:
                return 'badge-info';
            case 2:
                return 'badge-warning';
            case 3:
                return 'badge-danger';
        }
    }

    badgeTxt() {
        switch (this.order.Status) {
            case 0:
                return 'done';
            case 1:
                return 'ready';
            case 2:
                return 'rented';
            case 3:
                return 'late';
        }
    }

    ngOnInit() {

    }

}
