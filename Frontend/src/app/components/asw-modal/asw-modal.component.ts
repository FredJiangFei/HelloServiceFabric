import {Component, EventEmitter, Input, Output, ViewChild} from '@angular/core';
import {ModalDirective} from 'ngx-bootstrap';

@Component({
  selector: 'asw-modal',
  templateUrl: './asw-modal.component.html'
})

export class AswModalComponent {
  @Input() item: any;
  @Output() itemChange = new EventEmitter<any>();

  isSaving: boolean;
  @ViewChild('aswModal') public aswModal: ModalDirective;
  @Output() public saveFunc = new EventEmitter();
  @Input() public buttonDisable: boolean;

  constructor() {
    this.item = {};
  }

  displayModal(item: any): void {
    const i = Object.assign({}, item || {});
    this.itemChange.emit(i);
    this.aswModal.show();
  }

  saveClick() {
    this.isSaving = true;
    this.saveFunc.emit();
  }

  hideLoadingAndModal() {
    this.hideLoading();
    this.hideModal();
  }

  hideLoading() {
    this.isSaving = false;
  }

  hideModal() {
    this.aswModal.hide();
  }
}
