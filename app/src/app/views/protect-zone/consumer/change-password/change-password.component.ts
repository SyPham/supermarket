import { Account2Service } from 'src/app/_core/_service/account2.service';
import { Component, OnInit } from '@angular/core';
import { MessageConstants } from 'src/app/_core/_constants/system';
import { AlertifyService } from 'src/app/_core/_service/alertify.service';

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
    private service: Account2Service
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
        this.alertify.success(MessageConstants.CREATED_OK_MSG);
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
