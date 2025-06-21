import { Component, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { Auth } from '../../../services/Auth/auth';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiResponse } from '../../../models/api-response.model';
import { Getrole } from '../../../misc/Token';

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
    if (this.authService.getToken()) {
      let token = this.authService.getToken();
      // console.log(token);
      let role = Getrole(token);
      if(role === 'User')
        this.router.navigate(['/user']);
    }
  }
  constructor(private fb: FormBuilder,private authService: Auth, private router: Router) {}

  onLogin() {
    this.authService.login(this.loginForm.value)
      .subscribe({
        next: (res: ApiResponse) => {
          if (res.success && res.data?.token) {
            alert('Login Successful!');
            this.authService.setToken(res.data.token);
            this.router.navigate(['/user']);
          } else {
            alert(res.message || 'Login failed.');
          }
        },
        error: (error) => {
          const errorMessage = error?.error?.errors?.message || 'An unknown error occurred.';      
          alert(errorMessage);
        }
      });
}

}
