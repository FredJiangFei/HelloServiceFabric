import {Injectable} from '@angular/core';
import {ApiService} from './api.service';
import {Customer} from '../models/customer';
import {BaseService} from './base.service';

@Injectable()
export class CustomerService extends BaseService<Customer> {

  constructor(public apiService: ApiService) {
    super(apiService, 'customers');
  }

}

