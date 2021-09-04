import { CURDService } from './CURD.service';
import { Injectable } from '@angular/core';

import { HttpClient } from '@angular/common/http';
import { UtilitiesService } from './utilities.service';
import { Group } from '../_model/group';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class GroupService extends CURDService<Group> {

  constructor(http: HttpClient,utilitiesService: UtilitiesService)
  {
    super(http,"Group", utilitiesService);
  }

  updateStatus(id) {
    return this.http.post(this.base + `Store/UpdateStatus/${id}` ,{});
  }
  getAll(): Observable<any[]> {
    return this.http.get<any[]>(`${this.base}Group/GetAll`);
  }

}
