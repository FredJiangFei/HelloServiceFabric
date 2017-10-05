import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {CookieService} from 'angular2-cookie/core';

import {LoginService} from '../../services/login.service';
import {AuthService} from '../../services/auth.service';
import {LoginGuard} from '../../guards/login-guard';
import {AlertMessageService} from '../../services/alert-message.service';
import {CompanyService} from '../../services/company.service';
import {EmployeeService} from "../../services/employee.service";

@NgModule({
  imports: [
    CommonModule
  ],
  providers: [
    AuthService,
    LoginService,
    CompanyService,
    EmployeeService,
    AlertMessageService,
    CookieService,
    LoginGuard
  ]
})
export class CoreModule {
}
