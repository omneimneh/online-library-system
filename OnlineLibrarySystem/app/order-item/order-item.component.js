"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var OrderItemComponent = /** @class */ (function () {
    function OrderItemComponent() {
    }
    OrderItemComponent.prototype.badgeClass = function () {
        switch (this.order.OrderType) {
            case 0:
                return 'badge-success';
            case 1:
                return 'badge-info';
            case 2:
                return 'badge-warning';
            case 3:
                return 'badge-danger';
        }
    };
    OrderItemComponent.prototype.badgeTxt = function () {
        switch (this.order.OrderType) {
            case 0:
                return 'done';
            case 1:
                return 'ready';
            case 2:
                return 'rented';
            case 3:
                return 'late';
        }
    };
    OrderItemComponent.prototype.ngOnInit = function () {
    };
    __decorate([
        core_1.Input("order"),
        __metadata("design:type", Object)
    ], OrderItemComponent.prototype, "order", void 0);
    OrderItemComponent = __decorate([
        core_1.Component({
            selector: 'app-order-item',
            templateUrl: './order-item.component.html',
            styleUrls: ['./order-item.component.css']
        }),
        __metadata("design:paramtypes", [])
    ], OrderItemComponent);
    return OrderItemComponent;
}());
exports.OrderItemComponent = OrderItemComponent;
//# sourceMappingURL=order-item.component.js.map