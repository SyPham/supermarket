import { CURDService } from './CURD.service';
import { Injectable } from '@angular/core';

import { HttpClient } from '@angular/common/http';
import { Store } from '../_model/store';
import { UtilitiesService } from './utilities.service';
@Injectable({
  providedIn: 'root'
})
export class StoreService extends CURDService<Store> {

  constructor(http: HttpClient,utilitiesService: UtilitiesService)
  {
    super(http,"Store", utilitiesService);
  }

  updateStatus(id) {
    return this.http.post(this.base + `Store/UpdateStatus/${id}` ,{});
  }

}
