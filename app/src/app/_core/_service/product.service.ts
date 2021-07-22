import { CURDService } from './CURD.service';
import { Injectable } from '@angular/core';

import { HttpClient } from '@angular/common/http';
import { Product } from '../_model/product';
import { UtilitiesService } from './utilities.service';
@Injectable({
  providedIn: 'root'
})
export class ProductService extends CURDService<Product> {

  constructor(http: HttpClient,utilitiesService: UtilitiesService)
  {
    super(http,"Product", utilitiesService);
  }

}
