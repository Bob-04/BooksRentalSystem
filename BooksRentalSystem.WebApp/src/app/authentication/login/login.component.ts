import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { Validators } from '@angular/forms';
import { FormGroup, FormBuilder } from 'ngx-strongly-typed-forms';
import { AuthenticationService } from '../authentication.service';
import { LoginFormModel } from './login.model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup<LoginFormModel>;
  returnUrl: string;
  @Output() emitter: EventEmitter<string> = new EventEmitter<string>();

  constructor(private fb: FormBuilder, private authenticationService: AuthenticationService, private router: Router) {
    if (localStorage.getItem('token')) {
      this.router.navigate(['books'])
    }
  }

  ngOnInit(): void {
    localStorage.removeItem('token');
    this.loginForm = this.fb.group<LoginFormModel>({
      email: ['', Validators.required],
      password: ['', Validators.required],
    })
  }

  login() {
    this.authenticationService.login(this.loginForm.value).subscribe(res => {
      this.authenticationService.setToken(res['token']);

      this.authenticationService.getPublisherId().subscribe(res => {
        this.authenticationService.setId(res);

        this.router.navigate(['']).then(() => {
          window.location.reload();
        });
      })
    })
  }
}
