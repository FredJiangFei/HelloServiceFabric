import {NgModule} from '@angular/core';
import {Routes, RouterModule} from '@angular/router';
import {CustomerComponent} from './components/customer/customer.component';
import {HomeComponent} from './components/home/home.component';
import {LoginGuard} from './guards/login-guard';
import {LoginComponent} from './components/login/login.component';
import {CompanyComponent} from './components/company/company.component';

const routes: Routes = [
  {
    path: '',
    component: HomeComponent,
    canActivate: [LoginGuard],
    canActivateChild: [LoginGuard],
    children: [
      {
        path: 'customers', component: CustomerComponent
      },
      {
        path: 'companies', component: CompanyComponent
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
