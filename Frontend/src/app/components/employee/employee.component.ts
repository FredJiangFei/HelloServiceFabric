import {Component, OnInit} from '@angular/core';
import {EmployeeService} from "../../services/employee.service";
import {Employee} from "../../models/employee";

@Component({
  selector: 'asw-employee',
  templateUrl: './employee.component.html'
})
export class EmployeeComponent implements OnInit {
  public employees: Employee[];
  public employee: Employee;

  constructor(private employeeService: EmployeeService) {
    this.employee = new Employee();
  }

  ngOnInit() {
    this.getEmployees();
  }

  public getEmployees() {
    this.employeeService.getAll()
      .subscribe(employees => this.employees = employees);
  }

  public add(employee: Employee) {
    this.employeeService.create(employee)
      .subscribe(e => this.getEmployees());
  }

  public delete(employee: Employee) {
    this.employeeService.delete(employee.name)
      .subscribe(e => this.getEmployees());
  }
}
