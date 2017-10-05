import {Injectable} from '@angular/core';
import {ApiService} from './api.service';
import {BaseService} from './base.service';
import {Employee} from "../models/employee";
import {Observable} from "_rxjs@5.4.3@rxjs/Observable";

@Injectable()
export class EmployeeService extends BaseService<Employee> {

  constructor(public apiService: ApiService) {
    super(apiService, 'employee');
  }

  createEmployee(employee: Employee): Observable<Employee> {
    return this.apiService.put(`employee`, employee.name);
  }

  deleteEmployee(name: string): Observable<Employee> {
    return this.apiService.delete(`employee/${name}`);
  }
}

