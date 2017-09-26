import {Injectable} from '@angular/core';
import {Router} from '@angular/router';
import 'rxjs/add/operator/toPromise';

import {ApiService} from './api.service';
import {User} from '../models/user';
import {AuthService} from './auth.service';

@Injectable()
export class LoginService {
  redirectUrl: string;

  constructor(private apiService: ApiService,
              private router: Router,
              private authService: AuthService) {
  }

  login(loginUser: User) {
    return this.apiService.post('login', loginUser).toPromise()
      .then(user => this.authService.setLoginUser(user));
  }

  logout(): void {
    this.authService.clearLoginUser()
    this.router.navigate(['/login']);
  }
}
