import { ProductListComponent } from './product-list/product-list.component';
import { NgModule } from '@angular/core'
import { RouterModule, Routes } from '@angular/router'
import { AuthGuard } from 'src/app/_core/_guards/auth.guard'

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
        // canActivate: [AuthGuard]
      }
    ]
  },
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ConsumerRoutingModule { }
