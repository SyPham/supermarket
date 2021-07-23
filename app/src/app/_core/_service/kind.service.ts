import { CURDService } from './CURD.service';
import { Injectable } from '@angular/core';

import { HttpClient } from '@angular/common/http';
import { Kind } from '../_model/kind';
import { UtilitiesService } from './utilities.service';
import { Observable } from 'rxjs';
@Injectable({
  providedIn: 'root'
})
export class KindService extends CURDService<Kind> {

  constructor(http: HttpClient,utilitiesService: UtilitiesService)
  {
    super(http,"Kind", utilitiesService);
  }

  getAll() {
    return this.http.get<any[]>(`${this.base}Kind/GetAll`);
  }

  getAllStore() {
    return this.http.get<any[]>(`${this.base}Store/GetAll`);
  }

  getAllByStore(id ,lang) {
    return this.http.get<any[]>(`${this.base}Kind/GetAllByStoreLang/${id}/${lang}`);
  }

  getAllByLang(): Observable<any[]> {
    return this.http.get<any[]>(`${this.base}Kind/GetAllByLang?langId=${localStorage.getItem("lang")}`);
  }
}
