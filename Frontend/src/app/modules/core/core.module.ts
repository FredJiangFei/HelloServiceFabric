import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {CookieService} from 'angular2-cookie/core';

import {CustomerService} from '../../services/customer.service';
import {LoginService} from '../../services/login.service';
import {AuthService} from '../../services/auth.service';
import {LoginGuard} from '../../guards/login-guard';
import {AlertMessageService} from '../../services/alert-message.service';
import {CompanyService} from '../../services/company.service';

@NgModule({
  imports: [
    CommonModule
  ],
  providers: [
    CustomerService,
    AuthService,
    LoginService,
    CompanyService,
    AlertMessageService,
    CookieService,
    LoginGuard
  ]
})
export class CoreModule {
}
