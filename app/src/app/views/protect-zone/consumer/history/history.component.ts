import { Component, OnInit, ViewChild } from '@angular/core';
import { GridComponent } from '@syncfusion/ej2-angular-grids';
import { OrderService } from 'src/app/_core/_service/order.service';
import { UtilitiesService } from 'src/app/_core/_service/utilities.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-history',
  templateUrl: './history.component.html',
  styleUrls: ['./history.component.scss']
})
export class HistoryComponent implements OnInit {
  pageSettings = { pageCount: 20, pageSizes: true, pageSize: 10 };
  wrapSettings= { wrapMode: 'Content' };
  @ViewChild('grid') public grid: GridComponent;
  fullName: any;
  toolbarOptions = ['Search'];
  data: any;
  noImage = '/assets/img/photo1.png';
  base = environment.apiUrl.replace('/api','');
  active = 1;
  showPending: boolean;
  totalPrice = 0;
  constructor(
    private service: OrderService,
    private utilitiesService: UtilitiesService,
  ) { }

  ngOnInit() {
    this.showPending = true;
    this.fullName = JSON.parse(localStorage.getItem("user")).fullName;
    this.loadDataByBuyingAndPenidngStatus();
  }
  onClickComplete() {
    this.active = 3;
    this.showPending = false;
    this.loadDataByCompleteStatus();
  }
  onClickBuyingAndPenidngStatus() {
    this.active = 2;
    this.showPending = false;
    this.loadDataByBuyingAndPenidngStatus();
  }
  onClickPenidng() {
    this.active = 1;
    this.showPending = true;
  }
  loadData() {
    this.service.getProductsForCartStatus().subscribe(data => {
      this.data = data || [];
    });
  }
  loadDataByCompleteStatus() {
    this.service.getProductsForCartStatusByCompleteStatus().subscribe(data => {
      this.data = data || [];

      if ( this.data.length > 0) {
        this.totalPrice = data.map(x=> x.amountValue).reduce((previousValue,currentValue)=>previousValue + currentValue).toLocaleString();
      } else {
        this.totalPrice = 0;
      }
    });
  }
  loadDataByBuyingAndPenidngStatus() {
    this.service.getProductsForCartStatusByBuyingAndPenidngStatus().subscribe(data => {
      this.data = data || [];
      if ( this.data.length > 0) {
        this.totalPrice = data.map(x=> x.amountValue).reduce((previousValue,currentValue)=>previousValue + currentValue).toLocaleString();
      } else {
        this.totalPrice = 0;
      }
    });
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
