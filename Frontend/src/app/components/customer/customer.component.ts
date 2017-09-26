import {Component, OnInit, ViewChild} from '@angular/core';
import {FileUploader} from 'ng2-file-upload';
import 'rxjs/add/operator/finally';

import {CustomerService} from '../../services/customer.service';
import {Customer} from '../../models/customer';
import {environment} from '../../../environments/environment';

@Component({
  selector: 'asw-customer',
  templateUrl: './customer.component.html',
  styleUrls: ['./customer.component.css']
})

export class CustomerComponent implements OnInit {
  customers: Customer[];
  currentCustomer: Customer;
  isLoading: boolean;
  @ViewChild('saveModal') saveModal;
  @ViewChild('deleteModal') deleteModal;
  public uploader: FileUploader;

  constructor(private customerService: CustomerService) {
    this.currentCustomer = new Customer();
    this.uploader = new FileUploader({url: environment.API_URL + 'upload'});
  }

  ngOnInit() {
    this.getCustomers();
  }

  getCustomers() {
    this.isLoading = true;
    this.customerService.getAll()
      .finally(() => this.isLoading = false)
      .subscribe(customers => this.customers = customers);
  }

  saveCustomer() {
    if (this.uploader.queue.length === 0) {
      this.save();
      return;
    }

    this.uploader.uploadAll();
    this.uploader.onSuccessItem = (item, response, status, headers) => this.uploadImageSuccess(response);
  }

  uploadImageSuccess(response: string): any {
    const res = JSON.parse(response);
    this.currentCustomer.portrait = res.filename;
    this.save();
  }

  save() {
    const _observable = this.currentCustomer._id
      ? this.customerService.update(this.currentCustomer)
      : this.customerService.create(this.currentCustomer);

    _observable
      .finally(() => this.saveModal.hideLoadingAndModal())
      .subscribe(res => this.getCustomers());
  }

  deleteCustomer() {
    this.customerService.delete(this.currentCustomer._id)
      .finally(() => this.deleteModal.hideLoadingAndModal())
      .subscribe(res => this.getCustomers());
  }
}
