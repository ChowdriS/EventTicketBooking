import { Component, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { Auth } from '../../../services/Auth/auth';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiResponse } from '../../../models/api-response.model';
import { Getrole } from '../../../misc/Token';
import { NotificationService } from '../../../services/Notification/notification-service';

@Component({
  selector: 'app-login',
  templateUrl: './login.html',
  imports :[CommonModule,RouterLink,ReactiveFormsModule],
  standalone : true
})
export class Login implements OnInit {
  loginForm!: FormGroup;

  ngOnInit() {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
    this.roleBasedRoute();
  }

  roleBasedRoute(){
    if (this.authService.getToken()) {
      let token = this.authService.getToken();
      // console.log(token);
      let role = Getrole(token);
      if(role === 'User')
        this.router.navigate(['/user']);
      if(role === 'Manager')
        this.router.navigate(['/manager']);
      if(role === 'Admin')
        this.router.navigate(['/admin']);
    }
  }

  constructor(private fb: FormBuilder,private authService: Auth, private router: Router,private notify:NotificationService) {}

  onLogin() {
    this.authService.login(this.loginForm.value)
      .subscribe({
        next: (res: ApiResponse) => {
          if (res.success && res.data?.token) {
            this.authService.setToken(res.data.token);
            this.roleBasedRoute();
            this.notify.success("Login Success");
          } else {
            this.notify.error(res.message || 'Login failed.');
          }
        },
        error: (error) => {
          const errorMessage = error?.error?.errors?.message || 'An unknown error occurred.';      
          this.notify.error(errorMessage);
        }
      });
}

}
