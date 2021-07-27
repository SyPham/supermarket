import { FilterRequest } from './../../../../_core/_model/product';
import { BaseComponent } from 'src/app/_core/_component/base.component';
import { Component, OnInit, ViewChild } from '@angular/core';
import { AlertifyService } from 'src/app/_core/_service/alertify.service';
import { GridComponent } from '@syncfusion/ej2-angular-grids';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { Router, ActivatedRoute } from '@angular/router';
import { Cart, UpdateQuantityRequest } from 'src/app/_core/_model/cart';
import { OrderService } from 'src/app/_core/_service/order.service';
import { environment } from 'src/environments/environment';
import { UtilitiesService } from 'src/app/_core/_service/utilities.service';
@Component({
  selector: 'app-buy-list',
  templateUrl: './buy-list.component.html',
  styleUrls: ['./buy-list.component.scss']
})
export class BuyListComponent extends BaseComponent implements OnInit {
  base = environment.apiUrl.replace('/api','');
  editSettings = { showDeleteConfirmDialog: false, allowEditing: false, allowAdding: false, allowDeleting: false, mode: 'Normal' };
  data: any[] = [];
  password = '';
  modalReference: NgbModalRef;
  fields: object = { text: 'name', value: 'id' };
  toolbarOptions = ['Search'];
  wrapSettings= { wrapMode: 'Content' };
  pageSettings = { pageCount: 20, pageSizes: true, pageSize: 10 };
  @ViewChild('grid') public grid: GridComponent;
  model: Cart;
  filterRequest: FilterRequest;
  locale = localStorage.getItem('lang');
  totalPrice = 0;
  name: any;
  updateQuantityRequest: UpdateQuantityRequest;
  fullName: any;
  noImage = '/assets/img/photo1.png';

  constructor(
    private service: OrderService,
    public modalService: NgbModal,
    private alertify: AlertifyService,
    private router: Router,
    private utilitiesService: UtilitiesService,
    private route: ActivatedRoute,
  ) { super(); }

  ngOnInit() {
    // this.Permission(this.route);
    this.fullName = JSON.parse(localStorage.getItem("user")).fullName;
    this.wrapSettings = { wrapMode: 'Content' };
    this.loadData();
  }
  search(args) {
    console.log(args);
    this.grid.search(this.name);
  }

  actionBegin(args) {

  }
  back() {
    return this.router.navigate([`/consumer/product-list`]);

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
    this.service.getProductsInOrder().subscribe(res => {
      this.data = res.data || [];
      this.totalPrice = res.totalPrice || 0;
    });
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

