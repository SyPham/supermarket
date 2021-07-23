import { CURDService } from './CURD.service';
import { Injectable } from '@angular/core';

import { HttpClient } from '@angular/common/http';
import { AddToCartRequest, Cart, DeleteCartRequest, UpdateQuantityRequest } from '../_model/cart';
import { UtilitiesService } from './utilities.service';
import { Observable } from 'rxjs';
import { OperationResult } from '../_model/operation.result';
import { catchError } from 'rxjs/operators';
@Injectable({
  providedIn: 'root'
})
export class CartService extends CURDService<Cart> {

  constructor(http: HttpClient,utilitiesService: UtilitiesService)
  {
    super(http,"Cart", utilitiesService);
  }
  getProductsInCart(): Observable<any[]> {
    return this.http.get<any[]>(`${this.base}Cart/GetProductsInCart?langId=${localStorage.getItem("lang")}`);
  }
  cartTotal(): Observable<number> {
    return this.http.get<number>(`${this.base}Cart/CartTotal`);
  }
  updateQuantity(res: UpdateQuantityRequest): Observable<OperationResult> {
    return this.http.put<OperationResult>(`${this.base}Cart/UpdateQuantity`, res).pipe(
      catchError(this.handleError)
    );
  }
  addToCart(res: AddToCartRequest): Observable<OperationResult> {
    return this.http.post<OperationResult>(`${this.base}Cart/AddToCart`, res).pipe(
      catchError(this.handleError)
    );
  }
  deleteCart(res: DeleteCartRequest): Observable<OperationResult> {
    const query = this.utilitiesService.serialize(res);
    return this.http.delete<OperationResult>(`${this.base}Cart/deleteCart?${query}`).pipe(
      catchError(this.handleError)
    );
  }
  clearCart(): Observable<OperationResult> {
    return this.http.delete<OperationResult>(`${this.base}Cart/ClearCart`).pipe(
      catchError(this.handleError)
    );
  }
}
