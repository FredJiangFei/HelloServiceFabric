import {Component, OnInit, ViewChild} from '@angular/core';
import 'rxjs/add/operator/finally';

import {Company} from '../../models/company';
import {CompanyService} from '../../services/company.service';

@Component({
  selector: 'asw-company',
  templateUrl: './company.component.html',
  styleUrls: ['./company.component.css']
})

export class CompanyComponent implements OnInit {
  companies: Company[];
  currentCompany: Company;
  isLoading: boolean;
  @ViewChild('saveModal') saveModal;
  @ViewChild('deleteModal') deleteModal;

  constructor(private companyService: CompanyService) {
    this.currentCompany = new Company();
  }

  ngOnInit() {
    this.getCompanies();
  }

  getCompanies() {
    this.isLoading = true;
    this.companyService.getAll()
      .finally(() => this.isLoading = false)
      .subscribe(companies => this.companies = companies);
  }

  save() {
    const _observable = this.currentCompany.id
      ? this.companyService.update(this.currentCompany)
      : this.companyService.create(this.currentCompany);

    _observable
      .finally(() => this.saveModal.hideLoadingAndModal())
      .subscribe(res => this.getCompanies());
  }

  delete() {
    this.companyService.delete(this.currentCompany.id)
      .finally(() => this.deleteModal.hideLoadingAndModal())
      .subscribe(res => this.getCompanies());
  }
}
