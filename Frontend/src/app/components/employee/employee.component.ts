import {Component, OnInit} from '@angular/core';
import {Employee} from "../../models/employee";
import {EmployeeService} from "../../services/employee.service";

@Component({
  selector: 'asw-employee',
  templateUrl: './employee.component.html'
})
export class EmployeeComponent implements OnInit {
  public employees: string[];
  public employee: string;

  constructor(private employeeService: EmployeeService) {
  }

  ngOnInit() {
    this.getEmployees();
  }

  public getEmployees() {
    this.employeeService.getAll()
      .subscribe(employees => this.employees = employees);
  }

  public add(employee: string) {
    this.employeeService.updateBy(employee)
      .subscribe(e => this.getEmployees());
  }

  public delete(employee: string) {
    this.employeeService.delete(employee)
      .subscribe(e => this.getEmployees());
  }
}
