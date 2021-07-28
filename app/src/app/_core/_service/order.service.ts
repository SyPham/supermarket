import { CURDService } from './CURD.service';
import { OperationResult } from '../_model/operation.result'
import { Injectable } from '@angular/core'
import { Observable } from 'rxjs'
import { catchError } from 'rxjs/operators'
import { HttpClient } from '@angular/common/http';
import { Order } from '../_model/order';
import { UtilitiesService } from './utilities.service';
@Injectable({
  providedIn: 'root'
})
export class OrderService extends CURDService<Order> {

  constructor(http: HttpClient,utilitiesService: UtilitiesService)
  {
    super(http,"Order", utilitiesService);
  }
  placeOrder(): Observable<OperationResult> {
    return this.http.post<OperationResult>(`${this.base}Order/PlaceOrder`, {}).pipe(
      catchError(this.handleError)
    );
  }
  getProductsInOrder(): Observable<any> {
    return this.http.get<any>(`${this.base}Order/GetProductsInOrder?langId=${localStorage.getItem("lang")}`);
  }
  getProductsInOrderByAdmin(): Observable<any> {
    return this.http.get<any>(`${this.base}Order/getProductsInOrderByAdmin?langId=${localStorage.getItem("lang")}`);
  }
  getProductsInOrderPendingByAdmin(): Observable<any> {
    return this.http.get<any>(`${this.base}Order/getProductsInOrderPendingByAdmin?langId=${localStorage.getItem("lang")}`);
  }
  getProductsInOrderByingByAdmin(): Observable<any> {
    return this.http.get<any>(`${this.base}Order/GetProductsInOrderBuyingByAdmin?langId=${localStorage.getItem("lang")}`);
  }
  getProductsInOrderCompleteByAdmin(): Observable<any> {
    return this.http.get<any>(`${this.base}Order/getProductsInOrderCompleteByAdmin?langId=${localStorage.getItem("lang")}`);
  }
  transferByList(model) {
    return this.http.put(`${this.base}Order/TransferBuyList`, model);
  }
  transferComplete(model) {
    return this.http.put(`${this.base}Order/TransferComplete`, model);
  }
  getProductsForCartStatus(): Observable<any> {
    return this.http.get<any>(`${this.base}Order/GetProductsForCartStatus?langId=${localStorage.getItem("lang")}`);
  }
}
