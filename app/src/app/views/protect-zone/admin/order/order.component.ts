import { DatePipe } from '@angular/common';
import { NgxSpinnerService } from 'ngx-spinner';
import { FilterRequest } from './../../../../_core/_model/product';
import { BaseComponent } from 'src/app/_core/_component/base.component';
import { Component, ElementRef, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { AlertifyService } from 'src/app/_core/_service/alertify.service';
import { GridComponent, QueryCellInfoEventArgs } from '@syncfusion/ej2-angular-grids';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { Router, ActivatedRoute } from '@angular/router';
import { Cart, UpdateQuantityRequest } from 'src/app/_core/_model/cart';
import { OrderService } from 'src/app/_core/_service/order.service';
import { MessageConstants } from 'src/app/_core/_constants/system';
import jsPDF from 'jspdf';
import html2canvas from 'html2canvas';
import { UtilitiesService } from 'src/app/_core/_service/utilities.service';
import { environment } from 'src/environments/environment';
import { DomSanitizer } from '@angular/platform-browser';
import imageToBase64 from 'image-to-base64/browser';
import { ExportAsConfig, ExportAsService } from 'ngx-export-as';
import { EmitType } from '@syncfusion/ej2-base';
import { saveAs } from 'file-saver';
@Component({
  selector: 'app-order',
  templateUrl: './order.component.html',
  styleUrls: ['./order.component.scss'],
  providers:[DatePipe]
})
export class OrderComponent extends BaseComponent implements OnInit {
  editSettings = { showDeleteConfirmDialog: false, allowEditing: false, allowAdding: false, allowDeleting: false, mode: 'Normal' };
  editSettingsDisPatch = { showDeleteConfirmDialog: false, allowEditing: true, allowAdding: false, allowDeleting: false, mode: 'Normal' };
  data: any[] = [];
  password = '';
  modalReference: NgbModalRef;
  fields: object = { text: 'name', value: 'id' };
  toolbarOptions = ['Search'];
  wrapSettings= { wrapMode: 'Content' };
  pageSettings = { pageCount: 20, pageSizes: true, pageSize: 10 };
  @ViewChild('grid') public grid: GridComponent;
  @ViewChild('gridBuying') public gridBuying: GridComponent;
  @ViewChild('gridComplete') public gridComplete: GridComponent;
  model: Cart;
  filterRequest: FilterRequest;
  locale = localStorage.getItem('lang');
  totalPrice = 0;
  name: any;
  updateQuantityRequest: UpdateQuantityRequest;
  fullName: any;
  dataPicked = [];
  dataPickedDitchPatch = [];
  @ViewChild('htmlData2') public htmlData:ElementRef;
  dataAdd: any
  dataBuyingAdd: any
  public enableVirtualization: boolean = true;
  pendingTab: boolean =  false;
  buyingTab: boolean = true;
  completeTab: boolean = false;
  dispatchData: any
  @ViewChild('dispatchModal', { static: true })
  public dispatchModal: TemplateRef<any>;
  @ViewChild('gridDisPatch')
  gridDisPatch: GridComponent;
  ModelCreate: { productId: number; consumerId: number; qtyTamp: number; };
  pendingTabClass: any = "btn btn-default"
  buyingTabClass: any = "btn btn-success"
  completeTabClass: any = "btn btn-default"
  base = environment.apiUrl
  noImage = '/assets/img/photo1.png';
  img: any
  dataFake: any
  databuyingPersion: any
  startDate = new Date();
  endDate = new Date();
  configItem: ExportAsConfig = {
    type: 'xlsx',
    elementIdOrContent: 'buyItem',
  };
  configPersion: ExportAsConfig = {
    type: 'xlsx',
    elementIdOrContent: 'byPerson',
  };
  constructor(
    private service: OrderService,
    private spinner: NgxSpinnerService,
    private exportAsService: ExportAsService,
    private sanitizer: DomSanitizer,
    public modalService: NgbModal,
    private alertify: AlertifyService,
    private router: Router,
    private datePipe: DatePipe,
    private utilitiesService: UtilitiesService,
    private route: ActivatedRoute,
  ) { super(); }
  public queryCellInfoEvent: EmitType<QueryCellInfoEventArgs> = (args: QueryCellInfoEventArgs) => {
    const data = args.data as any;
    const fields = ['consumerId', 'fullName', 'totalPrice'];
    if (fields.includes(args.column.field)) {
      args.rowSpan = this.databuyingPersion.filter(
        item => item.consumerId === data.consumerId &&
          item.fullName === data.fullName &&
          item.totalPrice === data.totalPrice
      ).length;
    }
  }
  ngOnInit() {
    // this.Permission(this.route);
    this.removeLocalStore("dispatch")
    this.fullName = JSON.parse(localStorage.getItem("user")).fullName;
    this.wrapSettings = { wrapMode: 'Content' };
    this.loadDataBuying();
  }
  CancelBuying() {
    if (this.dataPickedDitchPatch.length > 0) {
      this.alertify.confirm2('Cancel Order! <br>!', 'Are you sure you want to cancel order this ?', () => {
        this.service.CancelBuying(this.dataPickedDitchPatch).subscribe(res => {
          if(res) {
            this.alertify.success(MessageConstants.CREATED_OK_MSG);
            this.loadDataBuying();
            this.dataPickedDitchPatch = []
          }else {
            this.alertify.warning(MessageConstants.SYSTEM_ERROR_MSG);
          }
        })
      }, () => {
        // cancelCallback();
      });

    } else {
      this.alertify.warning(MessageConstants.SYSTEM_ERROR_MSG);
    }
  }
  CancelPending() {
    if (this.dataPicked.length > 0 ) {
      this.alertify.confirm2('Cancel Order! <br>!', 'Are you sure you want to cancel order this ?', () => {
        this.service.CancelPending(this.dataPicked).subscribe(res => {
          if(res) {
            this.alertify.success(MessageConstants.CREATED_OK_MSG);
            this.dataPicked = []
            this.loadDataPending();
          }
        })
      }, () => {
        // cancelCallback();
      });

    } else {
      this.alertify.warning(MessageConstants.SYSTEM_ERROR_MSG);
    }
  }
  getbuyingPersion(){
    this.service.GetBuyingBuyPerson().subscribe(res => {
      this.databuyingPersion = res.data || []
    })
  }
  exportBuyItem() {
    this.spinner.show();
    this.service.reportBuyItem().subscribe(data =>{
      (saveAs(data,'exportBuyItem.xlsx'))
      this.alertify.success(MessageConstants.CREATED_OK_MSG);
      this.spinner.hide();
      // setTimeout(() => {
      // }, 2000);
    })
    // get the data as base64 or json object for json type - this will be helpful in ionic or SSR
    // this.exportAsService.get(this.config).subscribe(content => {
    //   console.log(content);
    // });

  }
  exportBuyPersion() {
    // this.exportAsService.save(this.configPersion, 'BuyingPerisonItem').subscribe(() => {
    //   // save started
    // });
    this.spinner.show();
    this.service.reportBuyPersion().subscribe(data =>{
      (saveAs(data,'exportBuyPersion.xlsx'))
      this.alertify.success(MessageConstants.CREATED_OK_MSG);
      this.spinner.hide();
      // setTimeout(() => {
      // }, 2000);
    })
  }
  onClickDefault() {
    this.startDate = new Date();
    this.endDate = new Date();
    this.loadDataComplete()
  }

  startDateOnchange(args) {
    this.startDate = (args.value as Date);
    this.loadDataComplete()
  }

  endDateOnchange(args) {
    this.endDate = (args.value as Date);
    this.loadDataComplete()
  }
  loadDataComplete() {
    this.data = [];
    this.service.getProductsInOrderCompleteByAdmin(this.startDate.toDateString() , this.endDate.toDateString()).subscribe(res => {
      this.data = res.data || [];
      this.totalPrice = res.totalPrice || 0;
    });
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
  public getSantizeUrl(url : string) {
    return this.sanitizer.bypassSecurityTrustUrl(url);
  }
  loadDataBuying() {
    this.data = [];
    this.service.getProductsInOrderByingByAdmin().subscribe(res => {
      this.data = res.data || [];
      console.log(this.data);
      this.totalPrice = res.totalPrice || 0;
    });
  }


  removeLocalStore(key: string) {
    localStorage.removeItem(key);
  }
  onDoubleClickDone(args: any): void {
    this.dispatchData = args.rowData.consumers
    if (args.column.field === 'consumers') {
      // const value = args.rowData as IToDoList;
      // this.openDispatchModalDoneList(value);
      this.showModal(this.dispatchModal);
      this.setLocalStore("dispatch",args.rowData.consumers)
    }
  }
  initialModel() {
    this.ModelCreate = {
      productId: 0,
      consumerId: 0,
      qtyTamp: 0
    };

  }
  DisPatchCheckBox() {
    if (this.dataPickedDitchPatch.length > 0) {
      this.service.transferComplete(this.dataPickedDitchPatch).subscribe(res => {
        if(res) {
          this.alertify.success(MessageConstants.CREATED_OK_MSG);
          this.loadDataBuying();
          this.dataPickedDitchPatch = []
        }else {
          this.alertify.warning(MessageConstants.SYSTEM_ERROR_MSG);
        }
      })
    } else {
      this.alertify.warning(MessageConstants.SYSTEM_ERROR_MSG);
    }
  }
  saveDisPatch(){
    const data = this.getLocalStore("dispatch");
    this.service.transferComplete(data).subscribe(res => {
      if(res) {
        this.alertify.success(MessageConstants.CREATED_OK_MSG);
        this.loadDataBuying();
        this.dataPickedDitchPatch = []
        this.modalReference.close()
        this.removeLocalStore("dispatch");
      }else {
        this.alertify.warning(MessageConstants.SYSTEM_ERROR_MSG);
      }
    })
  }
  setLocalStore(key: string, value: any) {
    localStorage.removeItem(key);
    let details = value || [];
    for (let key in details) {
      details[key].status = true;
    }
    const result = JSON.stringify(details);
    localStorage.setItem(key, result);
  }

  getLocalStore(key: string) {
    const data = JSON.parse(localStorage.getItem(key)) || [];
    return data;
  }
  actionBeginDisPatch(args) {
    if (args.requestType === 'save' && args.action === 'edit') {
      this.dataPickedDitchPatch = this.getLocalStore("dispatch");
      if(this.dataPickedDitchPatch.length >= 0) {
        for (var i = 0; i < this.dataPickedDitchPatch.length; i++) {
          if (this.dataPickedDitchPatch[i].productId == args.data.productId && this.dataPickedDitchPatch[i].consumerId == args.data.consumerId) {
            this.dataPickedDitchPatch[i].qtyTamp = args.data.qtyTamp;
            break;
          }
        }
      }
      this.setLocalStore("dispatch",this.dataPickedDitchPatch)
    }
  }
  actionCompleteDisPatch(args) {

  }
  showModal(modal){
    this.modalReference = this.modalService.open(modal, { size: 'lg'});

  }
  byList() {
    if (this.dataPicked.length > 0 ) {
      this.service.transferByList(this.dataPicked).subscribe(res => {
        if(res) {
          this.alertify.success(MessageConstants.CREATED_OK_MSG);
          this.dataPicked = []
          this.loadDataPending();
        }
      })
    } else {
      this.alertify.warning(MessageConstants.SYSTEM_ERROR_MSG);
    }
  }


  rowSelected(args){

    if (args.isHeaderCheckboxClicked) {
      for (const item of args.data) {
        for (const item2 of item.consumers) {
          this.dataAdd = {
            productId: item2.productId,
            consumerId: item2.consumerId,
          };
          this.dataPicked.push(this.dataAdd);
        }
      }
    }else {
      for (const item of args.data.consumers){
        this.dataAdd = {
          productId: item.productId,
          consumerId: item.consumerId,
        };
        this.dataPicked.push(this.dataAdd);
      }
    }
  }
  rowDeselected(args){
    if (args.isHeaderCheckboxClicked) {
      this.dataPicked = []
    }else {
      for (const item of args.data.consumers) {
        for (var i = 0; i < this.dataPicked.length; i++) {
          if (this.dataPicked[i].productId == item.productId && this.dataPicked[i].consumerId == item.consumerId) {
            this.dataPicked.splice(i, 1);
            break;
          }
        }
      }
    }
  }
  rowSelectedBuying(args){
    this.dataFake = this.gridBuying.getSelectedRecords();
    if (args.isHeaderCheckboxClicked) {
      for (const item of args.data) {
        for (const item2 of item.consumers) {
          this.dataBuyingAdd = {
            productId: item2.productId,
            consumerId: item2.consumerId,
            qtyTamp: item2.quantity
          };
          this.dataPickedDitchPatch.push(this.dataBuyingAdd);
        }
      }
    }else {
      for (const item of args.data.consumers){
        this.dataBuyingAdd = {
          productId: item.productId,
          consumerId: item.consumerId,
          qtyTamp: item.quantity
        };
        this.dataPickedDitchPatch.push(this.dataBuyingAdd);
      }
    }
  }
  rowDeselectedBuying(args){
    this.dataFake = this.gridBuying.getSelectedRecords();

    if (args.isHeaderCheckboxClicked) {
      this.dataPickedDitchPatch = []
    }else {
      for (const item of args.data.consumers) {
        for (var i = 0; i < this.dataPickedDitchPatch.length; i++) {
          if (this.dataPickedDitchPatch[i].productId == item.productId && this.dataPickedDitchPatch[i].consumerId == item.consumerId) {
            this.dataPickedDitchPatch.splice(i, 1);
            break;
          }
        }
      }
    }
  }
  search(args) {
    if(this.pendingTab)
      this.grid.search(this.name);
    if(this.buyingTab)
      this.gridBuying.search(this.name);
    if(this.completeTab)
      this.gridComplete.search(this.name);
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
  Tabpending(){
    this.pendingTabClass = "btn btn-success"
    this.buyingTabClass = "btn btn-default"
    this.completeTabClass = "btn btn-default"
    this.pendingTab = true
    this.completeTab = false
    this.buyingTab = false
    this.loadDataPending()
  }
  TabBuying(){
    this.pendingTabClass = "btn btn-default"
    this.buyingTabClass = "btn btn-success"
    this.completeTabClass = "btn btn-default"
    this.pendingTab = false
    this.completeTab = false
    this.buyingTab = true
    this.loadDataBuying()
    this.getbuyingPersion()
  }
  TabComplete(){
    this.pendingTabClass = "btn btn-default"
    this.buyingTabClass = "btn btn-default"
    this.completeTabClass = "btn btn-success"
    this.completeTab = true
    this.buyingTab = false
    this.pendingTab = false
    this.loadDataComplete()
  }
  loadDataPending() {
    this.data = [];
    this.service.getProductsInOrderPendingByAdmin().subscribe(res => {
      this.data = res.data || [];
      this.totalPrice = res.totalPrice || 0;
    });
  }




  // end api
  NO(index) {
    return (this.grid.pageSettings.currentPage - 1) * this.pageSettings.pageSize + Number(index) + 1;
  }
  NOBuying(index) {
    return (this.gridBuying.pageSettings.currentPage - 1) * this.pageSettings.pageSize + Number(index) + 1;
  }
  NOComplete(index) {
    return (this.gridComplete.pageSettings.currentPage - 1) * this.pageSettings.pageSize + Number(index) + 1;
  }

}
// function saveAs(data: Blob, arg1: string) {
//   throw new Error('Function not implemented.');
// }

