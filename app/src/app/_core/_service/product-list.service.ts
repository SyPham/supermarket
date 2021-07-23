import { CURDService } from './CURD.service';
import { Injectable } from '@angular/core';

import { HttpClient } from '@angular/common/http';
import { FilterRequest, Product } from '../_model/product';
import { UtilitiesService } from './utilities.service';
import { Observable } from 'rxjs';
@Injectable({
  providedIn: 'root'
})
export class ProductListService extends CURDService<Product> {

  constructor(http: HttpClient,utilitiesService: UtilitiesService)
  {
    super(http,"Product", utilitiesService);
  }
  getProductsForConsumer(request: FilterRequest): Observable<any[]> {
    const query = this.utilitiesService.serialize(request);
    return this.http.get<any[]>(`${this.base}Product/GetProductsForConsumer?${query}`);
  }
}
