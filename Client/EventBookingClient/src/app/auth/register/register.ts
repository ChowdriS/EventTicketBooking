import { Component, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';import { Auth } from '../../services/Auth/auth';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiResponse } from '../../models/api-response.model';
import { Getrole } from '../../shared/Token';

@Component({
  selector: 'app-register',
  templateUrl: './register.html',
  imports :[CommonModule,RouterLink,ReactiveFormsModule],
  standalone : true
})
export class Register implements OnInit{
  registerForm! : FormGroup;

  constructor(private fb : FormBuilder, private authService: Auth, private router: Router) {}
  ngOnInit(): void {
    this.registerForm = this.fb.group({
      role: ['user', Validators.required],
      username: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', Validators.required]
    }, { validator: this.passwordMatchValidator });

    if (this.authService.getToken()) {
      let token = this.authService.getToken();
      console.log(token);
      let role = Getrole(token);
      if(role === 'User')
        this.router.navigate(['/user']);
    }
  }

  passwordMatchValidator(group: FormGroup) {
    return group.get('password')?.value === group.get('confirmPassword')?.value
      ? null : { mismatch: true };
  }

  onRegister() {
    this.authService.register(this.registerForm.value).subscribe({
      next: (res: ApiResponse) => {
        if (res.success) {
          alert('Registration successful!');
          this.router.navigate(['/login']);
        } else {
          alert(res.message);
        }
      },
      error: (error) => {
        const errorMessage = error?.error?.errors?.message || 'An unknown error occurred.';      
        alert(errorMessage);
      }
    });

  }
}
