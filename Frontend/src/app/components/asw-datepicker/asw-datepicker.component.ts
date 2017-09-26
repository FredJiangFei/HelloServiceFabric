import {Component, EventEmitter, Input, Output} from '@angular/core';

@Component({
  selector: 'asw-datepicker',
  templateUrl: './asw-datepicker.component.html'
})
export class AswDatepickerComponent {
  @Input() date: any;
  @Output() dateChange: EventEmitter<any> = new EventEmitter<any>();
  datepickerOpen: boolean;

  selectionDone(dateSelected) {
    this.datepickerOpen = false;
    this.dateChange.emit(dateSelected);
  }
}
