import {Injectable} from '@angular/core';
import {ApiService} from './api.service';
import {BaseService} from './base.service';
import {Company} from '../models/company';

@Injectable()
export class CompanyService extends BaseService<Company> {

  constructor(public apiService: ApiService) {
    super(apiService, 'company');
  }

}

