import {Injectable} from '@angular/core';
import {Http, Headers, RequestOptions} from '@angular/http';
import {environment} from '../../environments/environment';
import {Observable} from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';
import 'rxjs/add/operator/map';

import {AlertMessageService} from './alert-message.service';

@Injectable()
export class ApiService {

  constructor(private http: Http,
              private alertMessageService: AlertMessageService) {
    this.http = http;
  }

  getAll(url): Observable<any> {
    return this.http.get(`${environment.API_URL}${url}`, this.getAuthorizationHeader())
      .map(res => res.json())
      .catch(err => this.handleError(err));
  }

  get(url): Observable<any> {
    return this.http.get(`${environment.API_URL}${url}`, this.getAuthorizationHeader())
      .map(res => res.json())
      .catch(err => this.handleError(err));
  }

  post(url, data): Observable<any> {
    return this.http.post(`${environment.API_URL}${url}`, data, this.getAuthorizationHeader())
      .map(res => res)
      .catch(err => this.handleError(err));
  }

  put(url, data): Observable<any> {
    return this.http.put(`${environment.API_URL}${url}`, data, this.getAuthorizationHeader())
      .map(res => res)
      .catch(err => this.handleError(err));
  }

  delete(url): Observable<any> {
    return this.http.delete(`${environment.API_URL}${url}`, this.getAuthorizationHeader())
      .map(res => res)
      .catch(err => this.handleError(err));
  }

  public handleError(err: Response) {
    this.alertMessageService.alertErrorMessage(err['_body']);
    return Observable.throw(err);
  }

  private getAuthorizationHeader() {
    const headers = new Headers();
    const params = new URLSearchParams();
    params.set('format', 'jsonp');
    params.set('callback', 'JSONP_CALLBACK');

    return new RequestOptions({headers: headers, search: params});
  }
}
