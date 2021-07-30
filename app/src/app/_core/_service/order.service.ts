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
  reportBuyPersion(){
    return this.http.get(`${this.base}Order/ReportBuyPersion/${localStorage.getItem("lang")}`,{responseType: 'blob'});
  }
  reportBuyItem(){
    return this.http.get(`${this.base}Order/ReportBuyItem/${localStorage.getItem("lang")}`,{responseType: 'blob'});
  }
  download(url: string){
    return this.http.get(url,{responseType: 'blob'})
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
  GetUserDelevery(startDate, endDate): Observable<any> {
    return this.http.get<any>(`${this.base}Order/GetUserDelevery/${localStorage.getItem("lang")}/${startDate}/${endDate}`);
  }
  GetBuyingBuyPerson(): Observable<any> {
    return this.http.get<any>(`${this.base}Order/GetBuyingBuyPerson/${localStorage.getItem("lang")}`);
  }
  getProductsInOrderByingByAdmin(): Observable<any> {
    return this.http.get<any>(`${this.base}Order/GetProductsInOrderBuyingByAdmin?langId=${localStorage.getItem("lang")}`);
  }
  getProductsInOrderCompleteByAdmin(startDate, endDate): Observable<any> {
    return this.http.get<any>(`${this.base}Order/getProductsInOrderCompleteByAdmin/${localStorage.getItem("lang")}/${startDate}/${endDate}`);
  }
  transferByList(model) {
    return this.http.put(`${this.base}Order/TransferBuyList`, model);
  }
  transferComplete(model) {
    return this.http.put(`${this.base}Order/TransferComplete`, model);
  }
  CancelBuying(model) {
    return this.http.put(`${this.base}Order/CancelBuyingList`, model);
  }
  CancelPending(model) {
    return this.http.put(`${this.base}Order/CancelPendingList`, model);
  }
  getProductsForCartStatus(): Observable<any> {
    return this.http.get<any>(`${this.base}Order/GetProductsForCartStatus?langId=${localStorage.getItem("lang")}`);
  }

  getProductsForCartStatusByCompleteStatus(): Observable<any> {
    return this.http.get<any>(`${this.base}Order/GetProductsForCartStatusByCompleteStatus?langId=${localStorage.getItem("lang")}`);
  }

  getProductsForCartStatusByBuyingAndPenidngStatus(): Observable<any> {
    return this.http.get<any>(`${this.base}Order/GetProductsForCartStatusByBuyingAndPenidngStatus?langId=${localStorage.getItem("lang")}`);
  }
}
