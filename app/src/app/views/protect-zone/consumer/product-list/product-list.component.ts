import { FilterRequest } from './../../../../_core/_model/product';
import { BaseComponent } from 'src/app/_core/_component/base.component';
import { Component, OnInit, ViewChild } from '@angular/core';
import { AlertifyService } from 'src/app/_core/_service/alertify.service';
import { EditService, ToolbarService, PageService, GridComponent } from '@syncfusion/ej2-angular-grids';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { ActivatedRoute, Router } from '@angular/router';
import { Account } from 'src/app/_core/_model/account';
import { MessageConstants } from 'src/app/_core/_constants/system';
import { ProductListService } from 'src/app/_core/_service/product-list.service';
import { CartService } from 'src/app/_core/_service/cart.service';
import { AddToCartRequest, Cart } from 'src/app/_core/_model/cart';
import { KindService } from 'src/app/_core/_service/kind.service';
import { StoreService } from 'src/app/_core/_service/store.service';
import { Store } from 'src/app/_core/_model/store';
import { DropDownListComponent } from '@syncfusion/ej2-angular-dropdowns';
import { UtilitiesService } from 'src/app/_core/_service/utilities.service';
import { environment } from 'src/environments/environment';
@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss']
})
export class ProductListComponent extends BaseComponent implements OnInit {
  base = environment.apiUrl.replace('/api','');
  data: Account[] = [];
  password = '';
  modalReference: NgbModalRef;
  fields: object = { text: 'name', value: 'id' };
  toolbarOptions = ['Search'];
  wrapSettings= { wrapMode: 'Content' };
  pageSettings = { pageCount: 20, pageSizes: true, pageSize: 10 };
  @ViewChild('grid') public grid: GridComponent;
  model: AddToCartRequest;
  filterRequest: FilterRequest;
  locale = localStorage.getItem('lang');
  name: any;
  kindId: number;
  storeId: number;
  kinds: any[];
  stores: Store[];
  cartTotal = 0;
  noImage = '/assets/img/photo1.png';
  dataKind: any[];
  constructor(
    private service: ProductListService,
    private serviceCart: CartService,
    private serviceStore: StoreService,
    private serviceKind: KindService,
    private utilitiesService: UtilitiesService,
    public modalService: NgbModal,
    private alertify: AlertifyService,
    private route: ActivatedRoute,
    private router: Router,
  ) { super();
    this.initialModel();
    this.loadKindData();
    this.loadStoreData();
    this.loadData();
    this.getCartTotal();

   }

  ngOnInit() {
    // this.Permission(this.route);
    this.wrapSettings = { wrapMode: 'Content' };
  }
  search(args) {
    this.grid.search(this.name);
  }
  getAllKindByStore(id) {
    this.serviceKind.getAllByStore(id, this.locale).subscribe(res => {
      console.log(res);
      this.kinds = res
    })
  }
  onChangeStore(args) {
      this.storeId = args.itemData.id;
      this.filterRequest = {
        storeId: this.storeId,
        langId: localStorage.getItem("lang"),
        kindId: this.kindId
      };
      this.getAllKindByStore(this.storeId)
      // this.loadData();
  }
  onChangeKind(args) {
      this.kindId = args.itemData.id;
      this.filterRequest = {
        storeId: this.storeId,
        langId: localStorage.getItem("lang"),
        kindId: this.kindId
      };
      this.loadData();
  }
  initialModel() {
    const storeId = +this.route.snapshot.params.storeId || 0;
    const kindId = +this.route.snapshot.params.kindId || 0;
    this.storeId = storeId;
    this.kindId = kindId;
    this.filterRequest = {
      storeId: this.storeId,
      langId: localStorage.getItem("lang"),
      kindId: this.kindId
    };

    this.model = {
      productId: 0,
      accountId: +JSON.parse(localStorage.getItem("user")).id,
      quantity: 0,
    };

  }
  gotocart() {
    return this.router.navigate([`/consumer/${this.storeId}/${this.kindId}/cart`]);
  }
  actionBegin(args) {
    if (args.requestType === 'save' && args.action === 'edit') {
      this.model = {
        productId: args.data.id,
        accountId: +JSON.parse(localStorage.getItem("user")).id,
        quantity: args.data.quantity,
      };
      this.addCart();
    }
  }

  toolbarClick(args) {
    switch (args.item.id) {
      case 'grid_excelexport':
        this.grid.excelExport({ hierarchyExportMode: 'All' });
        break;
      default:
        break;
    }
  }
  actionComplete(args) {
    if (args.requestType === 'beginEdit') {
      args.form.elements.namedItem('quantity').focus(); // Set focus to the Target element
    }
  }

  // end life cycle ejs-grid

  // api

  loadData() {
    this.service.getProductsForConsumer(this.filterRequest).subscribe(data => {
      this.data = data;
    });
  }
  loadKindData() {
    this.serviceKind.getAllByLang().subscribe(data => {
      this.kinds = data;
    });
  }
  loadStoreData() {
    this.serviceStore.getAll().subscribe(data => {
      this.stores = data;
    });
  }
  getCartTotal() {
    this.serviceCart.cartTotal().subscribe(data => {
      this.cartTotal = data;
    });
  }
  addCart() {
    this.serviceCart.addToCart(this.model).subscribe(
      (res) => {
        if (res.success === true) {
          this.alertify.success(MessageConstants.CREATED_OK_MSG);
          this.loadData();
          this.getCartTotal();
        } else {
          this.alertify.warning(MessageConstants.SYSTEM_ERROR_MSG);
        }
      },
      (error) => {
        this.alertify.warning(MessageConstants.SYSTEM_ERROR_MSG);
      }
    );
  }
  // end api
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

