import { Store } from './../../../../_core/_model/store';
import { Kind } from './../../../../_core/_model/kind';
import { KindService } from './../../../../_core/_service/kind.service';
import { Component, OnInit, ViewChild } from '@angular/core';
import { BaseComponent } from 'src/app/_core/_component/base.component';
import { Account2Service } from 'src/app/_core/_service/account2.service';
import { GridComponent } from '@syncfusion/ej2-angular-grids';
import { AlertifyService } from 'src/app/_core/_service/alertify.service';
import { MessageConstants } from 'src/app/_core/_constants/system';

@Component({
  selector: 'app-kind',
  templateUrl: './kind.component.html',
  styleUrls: ['./kind.component.scss']
})
export class KindComponent  extends BaseComponent implements OnInit {
  locale = localStorage.getItem('lang');
  pageSettings = { pageCount: 20, pageSizes: true, pageSize: 10 };
  setFocus: any;
  KindCreate: Kind;
  KindUpdate: Kind;
  data: Kind[] = [];
  storeFields: object = { text: 'name', value: 'id' };
  storeId: 0;
  dataStore: Store[] = [];
  @ViewChild('grid') public grid: GridComponent;
  constructor(
    private service: KindService,
    private alertify: AlertifyService,
  ) {
    super();
  }

  ngOnInit() {
    this.getAllStore();
    setTimeout(() => {

      this.getAll();
    }, 300);
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
  getAll(){
    this.service.getAll().subscribe((item: any) => {
      this.data = item.map((item: any) => {
        return {
          id: item.id,
          chineseName: item.chineseName,
          vietnameseName: item.vietnameseName,
          englishName: item.englishName,
          store_ID: item.store_ID,
          store_name: this.dataStore.filter((a) => a.id === item.store_ID)[0]?.name,
        }
      });
    })
  }
  getAllStore() {
    this.service.getAllStore().subscribe((item) => {
      this.dataStore = item
    })
  }

  actionComplete(args) {
    if (args.requestType === 'add') {
      // args.form.elements.namedItem('username').focus(); // Set focus to the Target element
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
      this.KindCreate = {
        id: 0,
        vietnameseName: args.data.vietnameseName,
        store_ID: this.storeId,
        englishName: args.data.englishName,
        chineseName: args.data.chineseName,
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
      this.KindUpdate = {
        id: args.data.id,
        store_ID: this.storeId,
        vietnameseName: args.data.vietnameseName,
        englishName: args.data.englishName,
        chineseName: args.data.chineseName,
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
    this.service.add(this.KindCreate).subscribe(
      (res) => {
        if (res.success === true) {
          this.alertify.success(MessageConstants.CREATED_OK_MSG);
          this.getAll();
          this.KindCreate = {} as Kind;
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
    this.KindCreate = {
      id: 0,
      vietnameseName: null,
      store_ID: this.storeId,
      englishName: null,
      chineseName: null,
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
    this.service.update(this.KindUpdate).subscribe(
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
