import {Injectable} from '@angular/core';

import {User} from '../models/user';
import {CookieService} from 'angular2-cookie/core';

@Injectable()
export class AuthService {
  private loginUser: User;

  constructor(private cookieService: CookieService) {}

  isLogin(): boolean {
    return this.getLoginUser() != null;
  }

  checkUserRole(url: string): boolean {
    const user = this.getLoginUser();
    if (!user) {
      return false;
    }

    const hasPermission = true;
    return hasPermission;
  }

  setLoginUser(user: User) {
    this.cookieService.putObject('loginUser', user);
  }

  getLoginUser(): User {
    return this.cookieService.getObject('loginUser') as User;
  }

  clearLoginUser() {
    this.cookieService.remove('loginUser');
  }
}
