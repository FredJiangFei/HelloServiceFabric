import {Component} from '@angular/core';

import {LoginService} from '../../services/login.service';
import {AuthService} from '../../services/auth.service';

@Component({
  selector: 'asw-navigation',
  templateUrl: './navigation.component.html'
})

export class NavigationComponent {
  isLogin: boolean;

  constructor(private loginService: LoginService,
              private authService: AuthService) {
    this.isLogin = this.authService.isLogin();
  }

  logout() {
    this.loginService.logout();
  }
}
