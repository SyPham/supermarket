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
  Add(model: Product) {
    const formData = new FormData();
    formData.append('UploadedFile', model.avatar);
    formData.append('VietnameseName', model.vietnameseName);
    formData.append('EnglishName', model.englishName);
    formData.append('ChineseName', model.chineseName);
    formData.append('Description', model.description);
    formData.append('OriginalPrice', model.originalPrice.toString());
    formData.append('CreatedBy', model.createdBy.toString());
    formData.append('StoreId', model.storeId.toString());
    formData.append('KindId', model.kindId.toString());
    return this.http.post(this.base + 'Product/Created', formData);
  }
  Updated(model: Product) {
    const formData = new FormData();
    formData.append('Id', model.id.toString());
    formData.append('UploadedFile', model.file);
    formData.append('Avatar', model.avatar);
    formData.append('VietnameseName', model.vietnameseName);
    formData.append('EnglishName', model.englishName);
    formData.append('ChineseName', model.chineseName);
    formData.append('Description', model.description);
    formData.append('OriginalPrice', model.originalPrice.toString());
    formData.append('CreatedBy', model.createdBy.toString());
    formData.append('StoreId', model.storeId.toString());
    formData.append('KindId', model.kindId.toString());
    return this.http.post(this.base + 'Product/Updated', formData);
  }
}
