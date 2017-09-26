import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {HttpModule} from '@angular/http';
import {ModalModule} from 'ngx-bootstrap';
import {AlertModule} from 'ngx-bootstrap/alert';
import {SpinnerModule} from 'angular2-spinner/dist';
import {PaginationModule} from 'ngx-bootstrap/pagination';

import {DatepickerModule} from 'ngx-bootstrap/datepicker';
import {AswModalComponent} from '../../components/asw-modal/asw-modal.component';
import {ImageUrlPipe} from '../../pipes/image-url.pipe';
import {JsonToArrayPipe} from '../../pipes/json-to-array.pipe';
import {EnumToArrayPipe} from '../../pipes/enum-to-array.pipe';
import {AswMessageComponent} from '../../components/asw-message/asw-message.component';
import {AswChartComponent} from '../../components/asw-chart/asw-chart.component';
import {AswLoadingComponent} from '../../components/asw-loading/asw-loading.component';
import {AswDatepickerComponent} from '../../components/asw-datepicker/asw-datepicker.component';
import {FormsModule} from '@angular/forms';

@NgModule({
  imports: [
    CommonModule,
    SpinnerModule,
    FormsModule,
    ModalModule.forRoot(),
    AlertModule.forRoot(),
    DatepickerModule.forRoot(),
    PaginationModule.forRoot()
  ],
  declarations: [
    AswModalComponent,
    AswChartComponent,
    AswDatepickerComponent,
    AswMessageComponent,
    AswLoadingComponent,
    ImageUrlPipe,
    JsonToArrayPipe,
    EnumToArrayPipe
  ],

  exports: [
    AswModalComponent,
    AswMessageComponent,
    AswDatepickerComponent,
    AswChartComponent,
    AswLoadingComponent,
    HttpModule,
    ModalModule,
    AlertModule,
    DatepickerModule,
    PaginationModule,
    SpinnerModule,
    JsonToArrayPipe,
    EnumToArrayPipe,
    ImageUrlPipe
  ],
  providers: [
    JsonToArrayPipe
  ]
})
export class ShareModule {
}
