import { BuyListComponent } from './buy-list/buy-list.component';
import { ProductListComponent } from './product-list/product-list.component';
import { NgModule } from '@angular/core'
import { RouterModule, Routes } from '@angular/router'
import { AuthGuard } from 'src/app/_core/_guards/auth.guard'
import { CartComponent } from './cart/cart.component';

const routes: Routes = [
  {
    path: '',
    data: {
      title: 'Consumer',
      breadcrumb: 'Consumer'
    },
    children: [
      {
        path: 'product-list',
        component: ProductListComponent,
        data: {
          title: 'Product List',
          breadcrumb: 'Product List',
          functionCode: 'Product List'
        },
        children: [
          {
            path: '',
            component: CartComponent,
          },
          {
            path: ':storeId/:kindId/cart',
            component: CartComponent,
            data: {
              title: 'Cart',
              breadcrumb: 'Cart',
              functionCode: 'Cart'
            },
          }
        ]
      },
      {
        path: ':storeId/:kindId/cart',
        component: CartComponent,
        data: {
          title: 'Cart',
          breadcrumb: 'Cart',
          functionCode: 'Cart'
        },
      },
      {
        path: 'product-list/:storeId/:kindId',
        component: ProductListComponent,
        data: {
          title: 'Product List',
          breadcrumb: 'Product List',
          functionCode: 'Product List'
        }
      },
      {
        path: 'buy-list',
        component: BuyListComponent,
        data: {
          title: 'Buy List',
          breadcrumb: 'Buy List',
          functionCode: 'Buy List'
        }
      },
    ]
  },
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ConsumerRoutingModule { }
