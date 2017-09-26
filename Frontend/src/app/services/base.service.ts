import {Injectable} from '@angular/core';
import {ApiService} from './api.service';
import {Observable} from 'rxjs/Observable';

@Injectable()
export class BaseService<T> {

  constructor(public apiService: ApiService, private url: string) {}

  getAll(): Observable<T[]> {
    return this.apiService.getAll(this.url);
  }

  getById(id: string): Observable<T> {
    return this.apiService.get(`${this.url}/${id}`);
  }

  create(item: T): Observable<T> {
    return this.apiService.post(this.url, item);
  }

  update(item: T): Observable<T> {
    return this.apiService.put(this.url, item);
  }

  delete(id: string): Observable<T> {
    return this.apiService.delete(`${this.url}/${id}`);
  }
}
