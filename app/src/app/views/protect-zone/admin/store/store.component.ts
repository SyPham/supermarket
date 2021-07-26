import { Store } from './../../../../_core/_model/store';
import { StoreService } from './../../../../_core/_service/store.service';
import { Component, OnInit, ViewChild } from '@angular/core';
import { BaseComponent } from 'src/app/_core/_component/base.component';
import { AlertifyService } from 'src/app/_core/_service/alertify.service';
import { GridComponent } from '@syncfusion/ej2-angular-grids';
import { MessageConstants } from 'src/app/_core/_constants/system';

@Component({
  selector: 'app-store',
  templateUrl: './store.component.html',
  styleUrls: ['./store.component.scss']
})
export class StoreComponent extends BaseComponent implements OnInit {

  locale = localStorage.getItem('lang');
  pageSettings = { pageCount: 20, pageSizes: true, pageSize: 10 };
  setFocus: any;
  StoreCreate: Store;
  name: any;
  StoreUpdate: Store;
  data: Store[] = [];
  storeFields: object = { text: 'name', value: 'id' };
  storeId: 0;
  @ViewChild('grid') public grid: GridComponent;
  constructor(
    private service: StoreService,
    private alertify: AlertifyService,
  ) { super(); }

  ngOnInit() {
    this.getAll();
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
  onDoubleClick(args: any): void {
    this.setFocus = args.column; // Get the column from Double click event
  }
  search(args) {
    this.grid.search(this.name);
  }
  getAll(){
    this.service.getAll().subscribe((item: any) => {
      this.data = item
    })
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
      this.StoreCreate = {
        id: 0,
        name: args.data.name,
        createdBy: 0,
        modifiedBy: 0,
        createdTime: new Date().toLocaleDateString(),
        modifiedTime: new Date().toLocaleDateString(),
      };

      // if (args.data.vietnameseName || args.data.chineseName || args.data.englishName  === undefined) {
      //   this.alertify.error('Please key in a Kind name! <br> Vui lòng nhập tên thể loại!');
      //   args.cancel = true;
      //   return;
      // }
      this.create();
    }
    if (args.requestType === 'save' && args.action === 'edit') {
      this.StoreUpdate = {
        id: args.data.id,
        name: args.data.name,
        createdBy: args.data.createdBy,
        modifiedBy: args.data.modifiedBy,
        createdTime: args.data.createdTime,
        modifiedTime: args.data.modifiedTime
      };
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
          this.getAll();
        } else {
           this.alertify.warning(MessageConstants.SYSTEM_ERROR_MSG);
        }
      },
      (err) => this.alertify.warning(MessageConstants.SYSTEM_ERROR_MSG)
    );

  }
  create() {
    this.service.add(this.StoreCreate).subscribe(
      (res) => {
        if (res.success === true) {
          this.alertify.success(MessageConstants.CREATED_OK_MSG);
          this.getAll();
          this.StoreCreate = {} as Store;
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
    this.StoreCreate = {
      id: 0,
      name: null,
      createdBy: 0,
      modifiedBy: 0,
      modifiedTime: null,
      createdTime: new Date().toLocaleDateString(),

    };

  }
  updateModel(data) {
    this.storeId = data.store_ID
  }
  update() {
    this.service.update(this.StoreUpdate).subscribe(
      (res) => {
        if (res.success === true) {
          this.alertify.success(MessageConstants.UPDATED_OK_MSG);
          this.getAll();
        } else {
          this.alertify.warning(MessageConstants.SYSTEM_ERROR_MSG);
        }
      },
      (error) => {
        this.alertify.warning(MessageConstants.SYSTEM_ERROR_MSG);
      }
    );
  }
  NO(index) {
    return (this.grid.pageSettings.currentPage - 1) * this.pageSettings.pageSize + Number(index) + 1;
  }
}
