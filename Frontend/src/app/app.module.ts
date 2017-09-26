import {BrowserModule} from '@angular/platform-browser';
import {NgModule} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {FileSelectDirective} from 'ng2-file-upload';

import {AppRoutingModule} from './app-routing.module';
import {AppComponent} from './app.component';
import {HomeComponent} from './components/home/home.component';
import {CustomerComponent} from './components/customer/customer.component';
import {NavigationComponent} from './components/navigation/navigation.component';
import {LoginComponent} from './components/login/login.component';
import {ShareModule} from './modules/share/share.module';
import {CoreModule} from './modules/core/core.module';
import {ApiService} from './services/api.service';
import {CompanyComponent} from './components/company/company.component';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    CustomerComponent,
    CompanyComponent,
    NavigationComponent,
    LoginComponent,
    FileSelectDirective
  ],
  imports: [
    BrowserModule,
    FormsModule,
    ShareModule,
    CoreModule,
    AppRoutingModule
  ],
  providers: [ApiService],
  bootstrap: [AppComponent]
})
export class AppModule {
}
