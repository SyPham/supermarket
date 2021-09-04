import { Account2Service } from 'src/app/_core/_service/account2.service';
import { Component, OnInit } from '@angular/core';
import { MessageConstants } from 'src/app/_core/_constants/system';
import { AlertifyService } from 'src/app/_core/_service/alertify.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.scss']
})
export class ChangePasswordComponent implements OnInit {
  confirmPassword: any;
  newPassword: any;
  constructor(
    private alertify: AlertifyService,
    private service: Account2Service,
    private router: Router
  ) { }

  ngOnInit() {
  }
  submit() {
    if (!this.newPassword || !this.confirmPassword) {
      this.alertify.warning("The new password and confirm password are empty! <br>Please try again!", true);
      return;
    }
    if (this.newPassword !== this.confirmPassword) {
      this.alertify.warning("The new password and confirm password are not the same! <br> Please try again!", true);
      return;
    }
    const request = {
      id: +JSON.parse(localStorage.getItem("user")).id,
      newPassword: this.newPassword,
      confirmPassword: this.confirmPassword
    }
    this.service.changePassword(request).subscribe( res => {
      if (res.success === true) {
        const lang = localStorage.getItem('lang')  ;
        const message = lang == 'vi' ? 'Chỉnh sửa thành công!' : lang === 'en' ? 'Revised Successfully' : '修改成功';
        const close = lang == 'vi' ? 'Đóng' : lang === 'en' ? 'Close' : '關閉';
        const viewProductList = lang == 'vi' ? 'Về trang sản phẩm' : lang === 'en' ? 'View Product List' : '去逛逛';
        this.alertify.confirm3('',message, close, viewProductList, () => {

        }, () => {
          this.router.navigate(['/consumer/product-list']);
          return;
        })
        this.newPassword = '';
        this.confirmPassword = '';
      } else {

         this.alertify.warning(MessageConstants.SYSTEM_ERROR_MSG);
      }
    }, err => {
      this.alertify.warning(MessageConstants.SYSTEM_ERROR_MSG)
    })
  }
}
