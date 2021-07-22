import { CURDService } from './CURD.service';
import { Injectable } from '@angular/core';

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

}
