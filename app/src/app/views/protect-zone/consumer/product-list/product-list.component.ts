import { FilterRequest } from './../../../../_core/_model/product';
import { BaseComponent } from 'src/app/_core/_component/base.component';
import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
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
import { TeamService } from 'src/app/_core/_service/team.service';
@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss']
})
export class ProductListComponent extends BaseComponent implements OnInit {
  base = environment.apiUrl.replace('/api','');
  data: any[] = [];
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
  productName: any;
  avatar: any;
  @ViewChild('preview', { static: true }) previewModal: TemplateRef<any>;
  editSettings = { showDeleteConfirmDialog: false, allowEditing: false, allowAdding: false, allowDeleting: false, mode: 'Normal' };
  teams: any[];
  teamId: any = 0;

  constructor(
    private service: ProductListService,
    private serviceTeam: TeamService,
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
    this.loadTeamData();
    this.loadKindData();
    this.loadStoreData();
    this.loadData();
    this.getCartTotal();

   }

  ngOnInit() {
    // this.Permission(this.route);
    this.teamId = 1;

    this.wrapSettings = { wrapMode: 'Content' };
  }
  search(args) {
    this.grid.search(this.name);
  }
  getAllKindByStore(id) {
    this.serviceKind.getAllByStore(id, localStorage.getItem("lang")).subscribe(res => {
      console.log(res);
      this.kinds = res;
      this.kinds.unshift({
        id:0,
      chineseName: null,
      createdBy: 0,
      createdTime: "0001-01-01T00:00:00",
      englishName: null,
      modifiedBy: null,
      modifiedTime: null,
      name:  localStorage.getItem("lang") == "en"? "All":  localStorage.getItem("lang") == 'vi' ? 'Tất cả' : '所有類別',
      store_ID: 0,
      vietnameseName: null,
      })
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
      this.loadData();
  }
  onChangeTeam(args) {
    this.teamId = args.itemData?.id || 0;
  }
  onChangeKind(args) {
    this.kindId = args.itemData?.id || 0;
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
    const teamId = +this.route.snapshot.params.teamId || 0;
    this.teamId = teamId;
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
      teamId: this.teamId
    };

  }
  gotocart() {
    return this.router.navigate([`/consumer/${this.teamId}/${this.storeId}/${this.kindId}/cart`]);
  }
  actionBegin(args) {
    if (args.requestType === 'save' && args.action === 'edit') {
      this.model = {
        productId: args.data.id,
        accountId: +JSON.parse(localStorage.getItem("user")).id,
        quantity: args.data.quantity,
        teamId: this.teamId
      };
      //this.addCart();
    }
  }
  rowSelected(args) {
    console.log(args);
    if (args.isInteracted) {
      this.model = {
        productId: args.data.id,
        accountId: +JSON.parse(localStorage.getItem("user")).id,
        quantity: args.data.quantity,
        teamId: this.teamId
      };
    }
  }
  ngModelChange(value) {
   this.model.quantity = value;
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
  loadTeamData() {
    this.serviceTeam.getAll().subscribe(data => {
      this.teams = data;
    });
  }
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
      this.stores = data.filter(x => x.status == true);
    });
  }
  getCartTotal() {
    this.serviceCart.cartTotal().subscribe(data => {
      this.cartTotal = data;
    });
  }
  increaseQuantity(data) {
    const index = +data.index;
    const quantity = data.quantity;
    if (quantity === 0 || quantity === 1) {
      this.alertify.warning('The quantity must be a valid number greater than 0!', true);
      return;
    } else {
      this.grid.updateCell(index, 'quantity', data.quantity--);
      this.model.quantity = data.quantity;
      this.model.productId = data.id;
      this.model.teamId = this.teamId;
      if (this.model.teamId == 0) {
        this.alertify.warning('Please select a team! Can not submit!', true);
        for (var i in this.data) {
          if (this.data[i].id == data.productId) {
            this.data[i].quantity = 0;
             break; //Stop this loop, we found it!
          }
        }
        this.grid.dataSource = this.data;
        return;
      }
      this.addCart();

    }
  }
  decreaseQuantity(data) {
    const index = +data.index;
    this.grid.updateCell(index, 'quantity', data.quantity++);
    this.model.quantity = data.quantity;
    this.model.productId = data.id;
    this.model.teamId = this.teamId;
    if (this.model.teamId == 0) {
      this.alertify.warning('Please select a team! Can not submit!', true);
      for (var i in this.data) {
        if (this.data[i].id == data.productId) {
          this.data[i].quantity = 0;
           break; //Stop this loop, we found it!
        }
      }
      this.grid.dataSource = this.data;
      return;
    }
    this.addCart();
  }
  addCart() {
    if (!this.model || this.model.quantity === 0) {
      this.alertify.warning('Can not submit!', true);
      return;
    }

    this.serviceCart.addToCart(this.model).subscribe(
      (res) => {
        if (res.success === true) {
          this.alertify.success(MessageConstants.CREATED_OK_MSG);
          // this.loadData();
          for (var i in this.data) {
            if (this.data[i].id == res.data.productId) {
              this.data[i].quantity = res.data.quantity;
               break; //Stop this loop, we found it!
            }
          }
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
  mouseenter(args,data) {
    this.grid.selectRows([+data.index]);
    this.avatar = data.avatar;
    this.productName = data.name;
    this.modalReference = this.modalService.open(this.previewModal, { size: 'lg'});
  }
}

