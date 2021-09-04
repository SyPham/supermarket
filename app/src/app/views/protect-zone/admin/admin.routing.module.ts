import { TeamComponent } from './team/team.component';
import { GroupComponent } from './Group/Group.component';
import { DeliveryComponent } from './delivery/delivery.component';
import { OrderComponent } from './order/order.component';
import { ProductComponent } from './product/product.component';
import { KindComponent } from './kind/kind.component';
import { StoreComponent } from './store/store.component';
import { NgModule } from '@angular/core'
import { RouterModule, Routes } from '@angular/router'
import { AuthGuard } from 'src/app/_core/_guards/auth.guard'
import { AccountComponent } from './account/account.component';


// import { PeriodComponent } from './period/period.component';
const routes: Routes = [
  {
    path: '',
    data: {
      title: 'Admin',
      breadcrumb: 'Admin'
    },
    children: [
      {
        path: 'account',
        component: AccountComponent,
        data: {
          title: 'Account',
          breadcrumb: 'Account',
          functionCode: 'account'
        },
        // canActivate: [AuthGuard]
      },
      {
        path: 'group',
        component: GroupComponent,
        data: {
          title: 'Account',
          breadcrumb: 'Account',
          functionCode: 'account'
        },
        // canActivate: [AuthGuard]
      },
      {
        path: 'team',
        component: TeamComponent,
        data: {
          title: 'Account',
          breadcrumb: 'Account',
          functionCode: 'account'
        },
        // canActivate: [AuthGuard]
      },
      {
        path: 'store',
        component: StoreComponent,
        data: {
          title: 'Store',
          breadcrumb: 'Store',
          functionCode: 'Store'
        },
        // canActivate: [AuthGuard]
      },
      {
        path: 'kind',
        component: KindComponent,
        data: {
          title: 'Kind',
          breadcrumb: 'Kind',
          functionCode: 'Kind'
        },
        // canActivate: [AuthGuard]
      },
      {
        path: 'product',
        component: ProductComponent,
        data: {
          title: 'Product',
          breadcrumb: 'Product',
          functionCode: 'Product'
        },
        // canActivate: [AuthGuard]
      },
      {
        path: 'order',
        component: OrderComponent,
        data: {
          title: 'Buy List',
          breadcrumb: 'Buy List',
          functionCode: 'Buy List'
        },
      },
      {
        path: 'delivery',
        component: DeliveryComponent,
        data: {
          title: 'Delivery',
          breadcrumb: 'Delivery',
          functionCode: 'Delivery'
        },
      }
    ]
  },
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AdminRoutingModule { }
