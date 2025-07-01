import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Auth } from '../../../services/Auth/auth';
import { Router, RouterLink } from '@angular/router';
import { Getrole } from '../../../misc/Token';
import { ApiResponse } from '../../../models/api-response.model';
import { CommonModule } from '@angular/common';
import { NotificationService } from '../../../services/Notification/notification-service';

@Component({
  selector: 'app-add-admin',
  imports: [CommonModule,ReactiveFormsModule],
  templateUrl: './add-admin.html',
  styleUrl: './add-admin.css'
})
export class AddAdmin implements OnInit{
  registerForm! : FormGroup;

  constructor(private fb : FormBuilder, private authService: Auth, private router: Router, private notify: NotificationService) {}
  ngOnInit(): void {
    this.registerForm = this.fb.group({
      username: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', Validators.required]
    }, { validator: this.passwordMatchValidator });
  }

  passwordMatchValidator(group: FormGroup) {
    return group.get('password')?.value === group.get('confirmPassword')?.value
      ? null : { mismatch: true };
  }

  onRegister() {
    this.authService.addAdmin(this.registerForm.value).subscribe({
      next: (res: ApiResponse) => {
        if (res.success) {
          this.notify.success('Admin added successfully!');
        } else {
          this.notify.error(res.message);
        }
      },
      error: (error:any) => {
        const errorMessage = error?.error?.errors?.message || 'An unknown error occurred.';      
        this.notify.error(errorMessage);
      }
    });

  }
}
