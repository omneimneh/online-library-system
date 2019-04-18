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
var http_1 = require("@angular/http");
var OrderRootComponent = /** @class */ (function () {
    function OrderRootComponent(http) {
        var _this = this;
        http.get('api/AccountApi/GetPersonOrders?token=' + $('#Token').val() + '&count=8')
            .subscribe(function (orders) { return _this.orders = orders.json(); });
    }
    OrderRootComponent.prototype.ngOnInit = function () {
    };
    OrderRootComponent = __decorate([
        core_1.Component({
            selector: 'app-order-root',
            templateUrl: './order-root.component.html',
            styleUrls: ['./order-root.component.css']
        }),
        __metadata("design:paramtypes", [http_1.Http])
    ], OrderRootComponent);
    return OrderRootComponent;
}());
exports.OrderRootComponent = OrderRootComponent;
//# sourceMappingURL=order-root.component.js.map