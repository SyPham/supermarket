import { FilterRequest } from './../../../../_core/_model/product';
import { BaseComponent } from 'src/app/_core/_component/base.component';
import { Component, OnInit, ViewChild } from '@angular/core';
import { AlertifyService } from 'src/app/_core/_service/alertify.service';
import { GridComponent } from '@syncfusion/ej2-angular-grids';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { Router, ActivatedRoute } from '@angular/router';
import { Account } from 'src/app/_core/_model/account';
import { MessageConstants } from 'src/app/_core/_constants/system';
import { CartService } from 'src/app/_core/_service/cart.service';
import { Cart, DeleteCartRequest, UpdateQuantityRequest } from 'src/app/_core/_model/cart';
import { OrderService } from 'src/app/_core/_service/order.service';
import { environment } from 'src/environments/environment';
import { UtilitiesService } from 'src/app/_core/_service/utilities.service';
@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.scss']
})
export class CartComponent extends BaseComponent implements OnInit {
  base = environment.apiUrl.replace('/api','');
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
  deleteCartRequest: DeleteCartRequest;
  noImage = '/assets/img/photo1.png';
  fullName: any;
  editSettings = { showDeleteConfirmDialog: false, allowEditing: false, allowAdding: false, allowDeleting: false, mode: 'Normal' };
  constructor(
    private service: CartService,
    private serviceOrder: OrderService,
    public modalService: NgbModal,
    private alertify: AlertifyService,
    private utilitiesService: UtilitiesService,
    private router: Router,
    private route: ActivatedRoute,
  ) { super(); }

  ngOnInit() {
    // this.Permission(this.route);
    this.wrapSettings = { wrapMode: 'Content' };
    this.loadData();
  }
  search(args) {
    console.log(args);
    this.grid.search(this.name);
  }
  rowSelected(args) {
    console.log(args);
    if (args.isInteracted) {
      this.updateQuantityRequest = {
        productId: args.data.productId,
        accountId: args.data.accountId,
        quantity: args.data.quantity
      };
    }
  }
  actionBegin(args) {
    if (args.requestType === 'save' && args.action === 'edit') {
      this.updateQuantityRequest = {
        productId: args.data.productId,
        accountId: args.data.accountId,
        quantity: args.data.quantity
      };
      this.updateQuantity();
    }
  }
  back() {
    const storeId = +this.route.snapshot.params.storeId || 0;
    const kindId = +this.route.snapshot.params.kindId || 0;
    const teamId = +this.route.snapshot.params.teamId || 0;
    return this.router.navigate([`/consumer/product-list/${teamId}/${storeId}/${kindId}`]);

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
    this.service.getProductsInCart().subscribe(data => {
      this.data = data || [];
      if ( this.data.length > 0) {
        this.totalPrice = data.map(x=> x.amountValue).reduce((previousValue,currentValue)=>previousValue + currentValue).toLocaleString();
      } else {
        this.totalPrice = 0;
      }
    });
  }

  updateQuantity() {
    this.service.updateQuantity(this.updateQuantityRequest).subscribe(
      (res) => {
        if (res.success === true) {
          this.alertify.success(MessageConstants.UPDATED_OK_MSG);
            // this.loadData();
            for (var i in this.data) {
              if (this.data[i].id == res.data.productId) {
                this.data[i].quantity = res.data.quantity;
                 break; //Stop this loop, we found it!
              }
            }
            this.service.getProductsInCart().subscribe(data => {
              if (data.length > 0) {
                this.totalPrice = data.map(x=> x.amountValue).reduce((previousValue,currentValue)=>previousValue + currentValue).toLocaleString();
              } else {
                this.totalPrice = 0;
              }
            });
        } else {
          this.alertify.warning(MessageConstants.SYSTEM_ERROR_MSG);
        }
      },
      (error) => {
        this.alertify.warning(MessageConstants.SYSTEM_ERROR_MSG);
      }
    );
  }
  placeOrder() {
    this.serviceOrder.placeOrder().subscribe(
      (res) => {
        if (res.success === true) {
          this.alertify.success(MessageConstants.CREATED_OK_MSG);
          const storeId = +this.route.snapshot.params.storeId || 0;
          const kindId = +this.route.snapshot.params.kindId || 0;
          this.router.navigate([`/consumer/buy-list`]);

        } else {
          this.alertify.warning(MessageConstants.SYSTEM_ERROR_MSG);
        }
      },
      (error) => {
        this.alertify.warning(MessageConstants.SYSTEM_ERROR_MSG);
      }
    );
  }
  deleteCart() {
    this.service.deleteCart(this.deleteCartRequest).subscribe(
      (res) => {
        if (res.success === true) {
          this.alertify.success(MessageConstants.DELETED_OK_MSG);
          this.loadData();
        } else {
          this.alertify.warning(MessageConstants.SYSTEM_ERROR_MSG);
        }
      },
      (error) => {
        this.alertify.warning(MessageConstants.SYSTEM_ERROR_MSG);
      }
    );
  }
  edit() {
    const index =  this.grid.getSelectedRowIndexes()[0] as any;
    if (index == undefined) {
      this.alertify.warning("No records selected for edit operation", true);
        return;
     }
    this.grid.selectRow(index);
    this.grid.startEdit();
  }
  delete() {
   const item =  this.grid.getSelectedRecords()[0] as any;
   if (item == undefined) {
    this.alertify.warning("No records selected for delete operation", true);
      return;
   }
   this.deleteCartRequest = {
    productId : item.productId,
    accountId : item.accountId
  }
  this.deleteCart();
  }
  clear() { this.clearCart();}
  clearCart() {
    this.service.clearCart().subscribe(
      (res) => {
        if (res.success === true) {
          this.alertify.success(MessageConstants.DELETED_OK_MSG);
          this.loadData();
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
  increaseQuantity(data) {
    const index = +data.index;
    const quantity = data.quantity;
    if (quantity === 0 || quantity === 1) {
      this.alertify.warning('The quantity must be a valid number greater than 0!', true);
      return;
    } else {
      this.grid.updateCell(index, 'quantity', data.quantity--);
      this.updateQuantityRequest = {
        productId: data.productId,
        accountId: data.accountId,
        quantity: data.quantity
      };
      this.updateQuantity();
    }
  }
  decreaseQuantity(data) {
    const index = +data.index;
    this.grid.updateCell(index, 'quantity', data.quantity++);
    this.updateQuantityRequest = {
      productId: data.productId,
      accountId: data.accountId,
      quantity: data.quantity
    };
    this.updateQuantity();
  }
  ngModelChange(value) {
   this.updateQuantityRequest.quantity = value;
  }
}

