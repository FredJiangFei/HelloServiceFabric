import {NgModule} from '@angular/core';
import {Routes, RouterModule} from '@angular/router';
import {HomeComponent} from './components/home/home.component';
import {LoginGuard} from './guards/login-guard';
import {LoginComponent} from './components/login/login.component';
import {CompanyComponent} from './components/company/company.component';
import {EmployeeComponent} from "./components/employee/employee.component";

const routes: Routes = [
  {
    path: '',
    component: HomeComponent,
    canActivate: [LoginGuard],
    canActivateChild: [LoginGuard],
    children: [
      {
        path: 'companies', component: CompanyComponent
      },{
        path: 'employees', component: EmployeeComponent
      }
    ]
  },
  {
    path: 'login', component: LoginComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, {useHash: true})],
  exports: [RouterModule]
})
export class AppRoutingModule {
}
