import { environment } from 'src/environments/environment';
import { StoreService } from './../../../../_core/_service/store.service';
import { KindService } from './../../../../_core/_service/kind.service';
import { ProductService } from './../../../../_core/_service/product.service';
import { Product } from './../../../../_core/_model/product';
import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { GridComponent } from '@syncfusion/ej2-angular-grids';
import { BaseComponent } from 'src/app/_core/_component/base.component';
import { AlertifyService } from 'src/app/_core/_service/alertify.service';
import { MessageConstants } from 'src/app/_core/_constants/system';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { UtilitiesService } from 'src/app/_core/_service/utilities.service';

@Component({
  selector: 'app-product',
  templateUrl: './product.component.html',
  styleUrls: ['./product.component.scss']
})
export class ProductComponent extends BaseComponent implements OnInit {
  base = environment.apiUrl.replace('/api','')
  locale = localStorage.getItem('lang');
  pageSettings = { pageCount: 20, pageSizes: true, pageSize: 10 };
  setFocus: any;
  ProductCreate = {
    id: 0,
    vietnameseName:  null,
    englishName:  null,
    chineseName:  null,
    description:  null,
    originalPrice: 0,
    avatar:  null,
    createdBy: 0,
    createdTime:  null,
    modifiedTime:  null,
    storeId: 0,
    file: null,
    kindId: 0
  };
  ProductUpdate: Product;
  data: Product[] = [];
  storeFields: object = { text: 'name', value: 'id' };
  storeId: 0;
  kindId: 0;
  @ViewChild('grid') public grid: GridComponent;
  @ViewChild('productModal', { static: true })
  productModal: TemplateRef<any>;
  modalReference: NgbModalRef;
  file: File = null
  previewUrl:any = null
  name: string = null
  dataStore: any
  dataKind: any
  dataKindAll: any
  title: string = "Add Product"
  previewUrl2:any = null
  kindFields: object = { text: 'name', value: 'id' };
  img: string | ArrayBuffer;
  noImage = '/assets/img/photo1.png';

  constructor(
    private service: ProductService,
    private service_kind: KindService,
    private service_store: StoreService,
    private alertify: AlertifyService,
    private utilitiesService: UtilitiesService,
    public modalService: NgbModal
  ) { super(); }

  ngOnInit() {
    this.getAllStore();
    this.getAllKind();
    setTimeout(() => {
      this.getAllProduct();
    }, 300);
  }
  updateStatus(id) {
    this.service.updateStatus(id).subscribe(res => {
      if (res) {
        this.alertify.success(MessageConstants.UPDATED_OK_MSG);
        this.getAllProduct();
      } else {
        this.alertify.warning(MessageConstants.SYSTEM_ERROR_MSG);
      }
    })
  }
  save() {
    this.ProductCreate.avatar = this.file;
    this.service.Add(this.ProductCreate).subscribe(res => {
      this.alertify.success("add success");
      this.resetForm();
      this.getAllProduct();
      this.modalReference.close();
    })
  }
  resetForm() {
    this.previewUrl = null,
    this.img = null
    this.file = null
    this.ProductCreate = {
      id: 0,
      vietnameseName:  null,
      englishName:  null,
      chineseName:  null,
      description:  null,
      originalPrice: 0,
      avatar:  null,
      createdBy: 0,
      createdTime:  null,
      modifiedTime:  null,
      storeId: 0,
      kindId: 0,
      file: null
    }
  }
  getAllStore() {
    this.service_store.getAll().subscribe(res => {
      this.dataStore = res
    })
  }
  getAllKind() {
    this.service_kind.getAll().subscribe(res => {
      this.dataKindAll = res
    })
  }
  getAllProduct() {
    this.service.getAll().subscribe((item: any) => {
      this.data = item.map((item: any) => {
        return {
          id: item.id,
          chineseName: item.chineseName,
          name: item.englishName,
          vietnameseName: item.vietnameseName,
          englishName: item.englishName,
          store_ID: item.storeId,
          kind_ID: item.kindId,
          store_name: this.dataStore.filter((a) => a.id === item.storeId)[0]?.name,
          kind_name: this.dataKindAll.filter((a) => a.id === item.kindId)[0]?.englishName,
          avatar: item.avatar,
          price: item.originalPrice,
          status: item.status,
          description: item.description
        }
      });

    })
  }
  showModal(modal) {
    this.title = "Add Product"
    this.getAllStore()
    this.modalReference = this.modalService.open(modal, { size: 'lg'});
  }
  edit(data ,modal) {
    console.log(data);
    this.openModalEdit(modal)
    this.ProductCreate.id = data.id;
    this.ProductCreate.englishName = data.englishName
    this.ProductCreate.vietnameseName = data.vietnameseName
    this.ProductCreate.chineseName = data.chineseName
    this.ProductCreate.description = data.description
    this.ProductCreate.storeId = data.store_ID
    this.ProductCreate.avatar = data.avatar
    this.getAllKindByStore(data.store_ID);
    this.ProductCreate.kindId = data.kind_ID
    this.ProductCreate.originalPrice = data.price
    this.img = this.base + data.avatar
  }
  openModalEdit(modal){
    this.modalReference = this.modalService.open(modal, { size: 'lg'});
  }
  fileProgress(event) {
    this.file = event.target.files[0];
    this.preview();
  }
  fileProgress2(event) {
    this.file = event.target.files[0];
    this.preview2();
  }
  preview2() {
    const mimeType = this.file.type;
    if (mimeType.match(/image\/*/) == null) {
      return;
    }
    const reader = new FileReader()
    reader.readAsDataURL(this.file)
    reader.onload = (_event) => {
      this.previewUrl2 = reader.result
      this.img = reader.result
    }
  }
  onChangeStore(args) {
    this.storeId = args.value
    this.ProductCreate.storeId = args.value
    this.getAllKindByStore(this.storeId)
  }
  getAllKindByStore(id) {
    this.service_kind.getAllByStore(id, this.locale).subscribe(res => {
      this.dataKind = res
    })
  }
  onChangeKind(args) {
    this.ProductCreate.kindId = args.value
    this.kindId = args.value
  }
  preview() {
    const mimeType = this.file.type;
    if (mimeType.match(/image\/*/) == null) {
      return;
    }
    const reader = new FileReader()
    reader.readAsDataURL(this.file)
    reader.onload = (_event) => {
      this.previewUrl = reader.result
    }
  }
  toolbarClick(args) {
    switch (args.item.id) {
      case 'grid_excelexport':
        this.grid.excelExport({ hierarchyExportMode: 'All' });
        break;
      case 'grid_add':
        args.cancel = true;
        this.showModal(this.productModal);
        break;
      default:
        break;
    }
  }
  onDoubleClick(args: any): void {
    this.setFocus = args.column; // Get the column from Double click event
  }


  actionComplete(args) {
    if (args.requestType === 'add') {
      args.form.elements.namedItem('name').focus(); // Set focus to the Target element
    }
  }
  actionBegin(args) {
    if (args.requestType === 'add') {
      this.initialModel();
    }
    if (args.requestType === 'beginEdit') {
      const item = args.rowData;
      this.updateModel(item);
    }
    if (args.requestType === 'save' && args.action === 'add') {
      // this.ProductCreate = {
      //   id: 0,
      //   name: args.data.name,
      //   createdBy: 0,
      //   modifiedBy: 0,
      //   createdTime: new Date().toLocaleDateString(),
      //   modifiedTime: new Date().toLocaleDateString(),
      // };

      // if (args.data.vietnameseName || args.data.chineseName || args.data.englishName  === undefined) {
      //   this.alertify.error('Please key in a Kind name! <br> Vui lòng nhập tên thể loại!');
      //   args.cancel = true;
      //   return;
      // }
      this.create();
    }
    if (args.requestType === 'save' && args.action === 'edit') {
      // this.StoreUpdate = {
      //   id: args.data.id,
      //   name: args.data.name,
      //   createdBy: args.data.createdBy,
      //   modifiedBy: args.data.modifiedBy,
      //   createdTime: args.data.createdTime,
      //   modifiedTime: args.data.modifiedTime
      // };
      this.update();
    }
    if (args.requestType === 'delete') {
      this.delete(args.data[0].id);
    }
  }
  delete(id) {
    this.service.delete(id).subscribe(
      (res) => {
        if (res.success === true) {
          this.alertify.success(MessageConstants.DELETED_OK_MSG);
          this.getAllProduct();
        } else {
           this.alertify.warning(MessageConstants.SYSTEM_ERROR_MSG);
        }
      },
      (err) => this.alertify.warning(MessageConstants.SYSTEM_ERROR_MSG)
    );

  }
  create() {
    this.service.add(this.ProductCreate).subscribe(
      (res) => {
        if (res.success === true) {
          this.alertify.success(MessageConstants.CREATED_OK_MSG);
          this.getAllProduct();
          this.ProductCreate = {} as Product;
        } else {
           this.alertify.warning(MessageConstants.SYSTEM_ERROR_MSG);
        }

      },
      (error) => {
        this.alertify.warning(MessageConstants.SYSTEM_ERROR_MSG);
      }
    );
  }
  initialModel() {
    this.storeId = 0;
    // this.ProductCreate = {
    //   id: 0,
    //   name: null,
    //   createdBy: 0,
    //   modifiedBy: 0,
    //   modifiedTime: null,
    //   createdTime: new Date().toLocaleDateString(),

    // };

  }
  updateModel(data) {
    this.storeId = data.store_ID
  }
  update() {
    this.ProductCreate.file = this.file;
    this.service.Updated(this.ProductCreate).subscribe(res => {
      if (res) {
        this.alertify.success(MessageConstants.UPDATED_OK_MSG);
        this.getAllProduct();
        this.modalReference.close();
      } else {
        this.alertify.warning(MessageConstants.SYSTEM_ERROR_MSG);
      }
    })
    // this.service.update(this.ProductUpdate).subscribe(
    //   (res) => {
    //     if (res.success === true) {
    //       this.alertify.success(MessageConstants.UPDATED_OK_MSG);
    //       this.getAllProduct();
    //     } else {
    //       this.alertify.warning(MessageConstants.SYSTEM_ERROR_MSG);
    //     }
    //   },
    //   (error) => {
    //     this.alertify.warning(MessageConstants.SYSTEM_ERROR_MSG);
    //   }
    // );
  }
  NO(index) {
    return (this.grid.pageSettings.currentPage - 1) * this.pageSettings.pageSize + Number(index) + 1;
  }
  imagePath(data) {
    if (data !== null && this.utilitiesService.checkValidImage(data)) {
      if (this.utilitiesService.checkExistHost(data)) {
        return data;
      } else {
        return this.base + data;
      }
    }
    return this.noImage;
  }
}
