import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';

import {User} from '../../models/user';
import {LoginService} from '../../services/login.service';

@Component({
  selector: 'asw-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})

export class LoginComponent implements OnInit {
  loginUser: User;
  logining: boolean;

  constructor(private loginService: LoginService,
              private router: Router) {
      this.loginUser = new User();
  }

  ngOnInit() {}

  login() {
    this.logining = true;
    this.loginService.login(this.loginUser)
      .then(res => {
        this.logining = false;
        const redirect = this.loginService.redirectUrl ? this.loginService.redirectUrl : '/';
        this.router.navigate([redirect]);
      }, err => this.logining = false);
  }
}
