import {Component, OnInit} from '@angular/core';
import {Employee} from "../../models/employee";
import {EmployeeService} from "../../services/employee.service";

@Component({
  selector: 'asw-employee',
  templateUrl: './employee.component.html',
  styleUrls: ['./employee.component.css']
})
export class EmployeeComponent implements OnInit {
  public employees: Employee[];
  public employee: Employee;

  constructor(private employeeService: EmployeeService) {
  }

  ngOnInit() {
    this.getEmployees();
  }

  public getEmployees() {
    this.employeeService.getAll()
      .subscribe(employees => this.employees = employees);
  }

  public addEmployee(employee: Employee) {
    this.employeeService.createEmployee(employee)
      .subscribe(e => this.getEmployees());
  }

  public deleteEmployee(employee: Employee) {
    this.employeeService.deleteEmployee(employee.name)
      .subscribe(e => this.getEmployees());
  }
}
