import {Injectable} from '@angular/core';
import {ApiService} from './api.service';
import {BaseService} from './base.service';

@Injectable()
export class EmployeeService extends BaseService<string> {

  constructor(public apiService: ApiService) {
    super(apiService, 'employee');
  }
}

