import {Injectable} from '@angular/core';
import {ApiService} from './api.service';
import {BaseService} from './base.service';
import {Employee} from "../models/employee";

@Injectable()
export class EmployeeService extends BaseService<Employee> {

  constructor(public apiService: ApiService) {
    super(apiService, 'employee');
  }
}

