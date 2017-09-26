import {Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, CanActivate, CanActivateChild, Router, RouterStateSnapshot} from '@angular/router';

import {LoginService} from '../services/login.service';
import {AuthService} from '../services/auth.service';

@Injectable()
export class LoginGuard implements CanActivate, CanActivateChild {

  constructor(private loginService: LoginService,
              private authService: AuthService,
              private router: Router) {
  }

  canActivate(route: ActivatedRouteSnapshot,
              state: RouterStateSnapshot): boolean {
    return this.validateIsLogin(state.url);
  }

  canActivateChild(childRoute: ActivatedRouteSnapshot,
                   state: RouterStateSnapshot): boolean {
    return this.validateIsLogin(state.url);
  }

  validateIsLogin(url: string) {
    return true;

    // const isLogin = this.authService.isLogin();
    // if (isLogin) {
    //   return true;
    // }
    //
    // this.loginService.redirectUrl = url;
    // this.router.navigate(['/login']);
  }
}

