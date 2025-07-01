import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, NgControl, NgForm, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiResponse } from '../../../models/api-response.model';
import { UserService } from '../../../services/User/user-service';
import { User } from '../../../models/user.model';
import { CommonModule } from '@angular/common';
import { TicketService } from '../../../services/Ticket/ticket.service';
import { NotificationService } from '../../../services/Notification/notification-service';
import { UserDetails } from '../../user-details/user-details';

@Component({
  selector: 'app-profile',
  imports: [FormsModule, CommonModule, ReactiveFormsModule, UserDetails],
  templateUrl: './profile.html',
  standalone: true
})
export class Profile implements OnInit {
  user!: User;
  isEditingUsername = false;
  isChangingPassword = false;
  usernameForm!: FormGroup;
  passwordForm!: FormGroup;
  tickets: any[] = [];
  currentPage = 1;
  pageSize = 5;
  totalPages = 1;
  originalName:string = '';
  constructor(private userService: UserService, private fb: FormBuilder, private ticketService: TicketService) { }

  ngOnInit(): void {
    this.loadUserDetails();
    this.loadTickets();
    this.usernameForm = this.fb.group({
      username: ['', Validators.required]
    });

    this.passwordForm = this.fb.group({
      oldPassword: ['', Validators.required],
      newPassword: ['', [Validators.required, Validators.minLength(6)]]
    });
  }
  CancelEdit(){
    this.usernameForm.patchValue({ username: this.originalName });
    this.isEditingUsername = false;
  }
  
  loadTickets() {
    this.ticketService.getMyTickets(this.currentPage, this.pageSize).subscribe({
      next: (res) => {
        const data = res.data;
        this.tickets = data.items.$values;
        this.totalPages = data.totalPages;
        this.currentPage = data.pageNumber;
      },
      error: () => alert('Failed to load tickets')
    });
  }

  changePage(page: number) {
    if (page < 1 || page > this.totalPages) return;
    this.currentPage = page;
    this.loadTickets();
  }

  cancelTicket(id: string) {
    if (!confirm('Are you sure you want to cancel this ticket?')) return;

    this.ticketService.cancelTicket(id).subscribe({
      next: () => {
        alert('Ticket cancelled successfully.');
        this.loadTickets();
      },
      error: () => alert('Cancellation failed. Try again.')
    });
  }

  exportTicket(id: string) {
    this.ticketService.exportTicket(id).subscribe({
      next: (pdfBlob: Blob) => {
        const url = window.URL.createObjectURL(pdfBlob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `Ticket_${id}.pdf`;
        document.body.appendChild(a);
        a.click();
        setTimeout(() => {
          document.body.removeChild(a);
          window.URL.revokeObjectURL(url);
        }, 100);
        // alert('Download started successfully!');
      },
      error: (error) => {
        alert(`Error: ${error.message}`);
      }
    });
  }

  loadUserDetails() {
    this.userService.getUserDetails().subscribe((res: ApiResponse) => {
      this.user = res.data;
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