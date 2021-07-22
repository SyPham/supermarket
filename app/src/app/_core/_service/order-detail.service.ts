import { CURDService } from './CURD.service';
import { Injectable } from '@angular/core';

import { HttpClient } from '@angular/common/http';
import { OrderDetail } from '../_model/order-detail';
import { UtilitiesService } from './utilities.service';
@Injectable({
  providedIn: 'root'
})
export class OrderDetailService extends CURDService<OrderDetail> {

  constructor(http: HttpClient,utilitiesService: UtilitiesService)
  {
    super(http,"OrderDetail", utilitiesService);
  }

}
