import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, NgControl, NgForm, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { User } from '../../models/user.model';
import { UserService } from '../../services/User/user-service';
import { TicketService } from '../../services/Ticket/ticket.service';
import { NotificationService } from '../../services/Notification/notification-service';
import { ApiResponse } from '../../models/api-response.model';
@Component({
  selector: 'app-user-details',
  imports: [ ReactiveFormsModule, CommonModule],
  templateUrl: './user-details.html',
  styleUrl: './user-details.css',
  standalone: true
})
export class UserDetails implements OnInit {
  user: User = { email: '', role: '', username: '' }; 
  isEditingUsername = false;
  isChangingPassword = false;
  usernameForm!: FormGroup;
  passwordForm!: FormGroup;
  originalName:string = '';
  constructor(private userService: UserService, private fb: FormBuilder,
     private ticketService: TicketService,private notificationService : NotificationService) { }

  ngOnInit(): void {
    this.usernameForm = this.fb.group({
      username: ['', Validators.required]
    });
    
    this.passwordForm = this.fb.group({
      oldPassword: ['', Validators.required],
      newPassword: ['', [Validators.required, Validators.minLength(6)]]
    });
    this.loadUserDetails();
  }
  CancelEdit(){
    this.usernameForm.patchValue({ username: this.originalName });
    this.isEditingUsername = false;
  }

  loadUserDetails() {
    this.userService.getUserDetails().subscribe((res: ApiResponse) => {
      this.user = res.data;
      console.log(this.user)
      this.usernameForm.get('username')?.setValue(this.user.username);
      this.originalName = this.user.username;
    });
  }

  saveUsername() {
    if (this.usernameForm.invalid) return;

    const payload = { username: this.usernameForm.value.username };

    this.userService.updateUsername(payload).subscribe({
      next: (res: ApiResponse) => {
        this.user = res.data;
        this.isEditingUsername = false;
        this.notificationService.success("Username Changed");
        this.loadUserDetails();
      },
      error: () => alert('Failed to update username.')
    });
  }

  changePassword() {
    if (this.passwordForm.invalid) return;

    this.userService.changePassword(this.passwordForm.value).subscribe({
      next: (res: ApiResponse) => {
        alert('Password changed successfully!');
        this.isChangingPassword = false;
        this.passwordForm.reset();
      },
      error: () => alert('Failed to change password.')
    });
  }

  cancelPasswordChange() {
    this.isChangingPassword = false;
    this.passwordForm.reset();
  }
}