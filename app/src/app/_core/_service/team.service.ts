import { Team } from './../_model/team';
import { CURDService } from './CURD.service';
import { Injectable } from '@angular/core';

import { HttpClient } from '@angular/common/http';
import { UtilitiesService } from './utilities.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TeamService extends CURDService<Team> {

  constructor(http: HttpClient,utilitiesService: UtilitiesService)
  {
    super(http,"Team", utilitiesService);
  }

  updateStatus(id) {
    return this.http.post(this.base + `Team/UpdateStatus/${id}` ,{});
  }

  getAll(): Observable<any[]> {
    return this.http.get<any[]>(`${this.base}Team/GetAll`);
  }

}
