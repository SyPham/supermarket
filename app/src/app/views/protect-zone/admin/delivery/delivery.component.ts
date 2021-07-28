import { Component, OnInit } from '@angular/core';
import { QueryCellInfoEventArgs } from '@syncfusion/ej2-angular-grids';
import { EmitType } from '@syncfusion/ej2-base';
import { NgxSpinnerService } from 'ngx-spinner';
import { BaseComponent } from 'src/app/_core/_component/base.component';
import { OrderService } from 'src/app/_core/_service/order.service';

@Component({
  selector: 'app-delivery',
  templateUrl: './delivery.component.html',
  styleUrls: ['./delivery.component.scss']
})
export class DeliveryComponent extends BaseComponent implements OnInit {
  data: any = []
  dataTamp: any = []
  total: number;
  startDate = new Date();
  endDate = new Date();
  constructor(
    private service: OrderService,
    private spinner: NgxSpinnerService
  ) { super();}
  public queryCellInfoEvent: EmitType<QueryCellInfoEventArgs> = (args: QueryCellInfoEventArgs) => {
    const data = args.data as any;
    const fields = ['consumerId', 'fullName', 'totalPrice'];
    if (fields.includes(args.column.field)) {
      args.rowSpan = this.data.filter(
        item => item.consumerId === data.consumerId &&
          item.fullName === data.fullName &&
          item.totalPrice === data.totalPrice &&
          item.date === data.date
      ).length;
    }
  }
  ngOnInit() {
    this.getAll()
  }
  onClickDefault() {
    this.startDate = new Date();
    this.endDate = new Date();
    this.getAll()
  }

  startDateOnchange(args) {
    this.startDate = (args.value as Date);
    this.getAll()
  }

  endDateOnchange(args) {
    this.endDate = (args.value as Date);
    this.getAll()
  }
  getAll() {
    this.spinner.show();
    this.service.GetUserDelevery(this.startDate.toDateString() , this.endDate.toDateString()).subscribe(res => {
      this.dataTamp = res.data || [];
      this.data = res.data || [];
      this.spinner.hide();
    })
  }
  convertTotal(item) {
    this.total = 0
    for(const items of this.dataTamp) {
      if(items.consumerId === item)
        this.total = this.total + parseFloat(items.subtotalPrice)
    }
    return this.total
  }
}
