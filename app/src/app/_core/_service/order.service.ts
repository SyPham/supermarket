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
}
