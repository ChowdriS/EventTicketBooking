import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { UserService } from '../../services/User/user-service';
import { NotificationService } from '../../services/Notification/notification-service';
import { ApiResponse } from '../../models/api-response.model';
import { User } from '../../models/user.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-user-details',
  templateUrl: './user-details.html',
  styleUrls: ['./user-details.css'],
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule]
})
export class UserDetails implements OnInit {
  user: User = { email: '', role: '', username: '' };
  isEditingUsername = false;
  isChangingPassword = false;
  usernameForm!: FormGroup;
  passwordForm!: FormGroup;
  originalName = '';

  constructor(
    private userService: UserService,
    private fb: FormBuilder,
    private notify: NotificationService
  ) { }

  ngOnInit(): void {
    this.initializeForms();
    this.loadUserDetails();
  }

  private initializeForms(): void {
    this.usernameForm = this.fb.group({
      username: ['', Validators.required]
    });
    
    this.passwordForm = this.fb.group({
      oldPassword: ['', Validators.required],
      newPassword: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  loadUserDetails(): void {
    this.userService.getUserDetails().subscribe({
      next: (res: ApiResponse<User>) => {
        this.user = res.data || { email: '', role: '', username: '' };
        this.usernameForm.patchValue({ username: this.user.username });
        this.originalName = this.user.username;
      },
      error: () => console.error('Failed to load user details')
    });
  }

  saveUsername(): void {
    if (this.usernameForm.invalid) return;
    
    const newUsername = this.usernameForm.value.username;
    if (this.user.username === newUsername) {
      this.isEditingUsername = false;
      return;
    }

    this.userService.updateUsername({ username: newUsername }).subscribe({
      next: (res: ApiResponse<User>) => {
        this.user = res.data || this.user;
        this.notify.success('Username Changed');
        this.isEditingUsername = false;
        this.originalName = newUsername;
      },
      error: () => this.notify.error('Failed to update username.')
    });
  }

  changePassword(): void {
    if (this.passwordForm.invalid) return;
    
    const { oldPassword, newPassword } = this.passwordForm.value;
    if (oldPassword === newPassword) {
      this.notify.error('The old and new passwords are same. Try stronger passwords!');
      return;
    }

    this.userService.changePassword({ oldPassword, newPassword }).subscribe({
      next: () => {
        this.notify.success('Password changed successfully!');
        this.cancelPasswordChange();
      },
      error: () => this.notify.error('Failed to change password.')
    });
  }

  CancelEdit(): void {
    this.usernameForm.patchValue({ username: this.originalName });
    this.isEditingUsername = false;
  }

  cancelPasswordChange(): void {
    this.isChangingPassword = false;
    this.passwordForm.reset();
  }
}