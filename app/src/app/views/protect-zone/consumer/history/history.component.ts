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
  active = true;
  constructor(
    private service: OrderService,
    private utilitiesService: UtilitiesService,
  ) { }

  ngOnInit() {
    this.fullName = JSON.parse(localStorage.getItem("user")).fullName;
    this.loadDataByBuyingAndPenidngStatus();
  }
  onClickComplete() {
    this.active = false;
    this.loadDataByCompleteStatus();
  }
  onClickBuyingAndPenidngStatus() {
    this.active = true;
    this.loadDataByBuyingAndPenidngStatus();
  }
  loadData() {
    this.service.getProductsForCartStatus().subscribe(data => {
      this.data = data || [];
    });
  }
  loadDataByCompleteStatus() {
    this.service.getProductsForCartStatusByCompleteStatus().subscribe(data => {
      this.data = data || [];
    });
  }
  loadDataByBuyingAndPenidngStatus() {
    this.service.getProductsForCartStatusByBuyingAndPenidngStatus().subscribe(data => {
      this.data = data || [];
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
