import { Component, OnInit, ViewChild } from '@angular/core';
import { GridComponent } from '@syncfusion/ej2-angular-grids';

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

  constructor() { }

  ngOnInit() {
    this.fullName = JSON.parse(localStorage.getItem("user")).fullName;
  }
  NO(index) {
    return (this.grid.pageSettings.currentPage - 1) * this.pageSettings.pageSize + Number(index) + 1;
  }
}
