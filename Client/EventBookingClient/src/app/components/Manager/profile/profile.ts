import { HttpClient } from '@angular/common/http';
import { Component, signal, computed, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { AnalyticsService } from '../../../services/Analytics/analytics.service';
import { CommonModule } from '@angular/common';
import Chart from 'chart.js/auto';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { User } from '../../../models/user.model';
import { ApiResponse } from '../../../models/api-response.model';
import { UserService } from '../../../services/User/user-service';
import { TicketService } from '../../../services/Ticket/ticket.service';
import { NotificationService } from '../../../services/Notification/notification-service';
import { UserDetails } from "../../user-details/user-details";

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, UserDetails],
  templateUrl: './profile.html',
  styleUrls: ['./profile.css']
})
export class Profile implements AfterViewInit {
  @ViewChild('earningsChart') chartRef!: ElementRef<HTMLCanvasElement>;
  private chartInstance: any;
  user!: User;
  isEditingUsername = false;
  isChangingPassword = false;
  usernameForm!: FormGroup;
  passwordForm!: FormGroup;
  earningsTableSignal = signal<{ event: string; amount: number }[]>([]);
  earningsTable = computed(() => this.earningsTableSignal());
  earningsTableTotal = computed(() => this.earningsTableSignal().reduce((sum, item) => sum + item.amount, 0));
  originalName:string = '';
  
  CancelEdit(){
    this.usernameForm.patchValue({ username: this.originalName });
    this.isEditingUsername = false;
  }

  constructor(private http: HttpClient,
    private analyticsService: AnalyticsService,private userService: UserService, 
    private fb: FormBuilder, private ticketService: TicketService, private notificationService : NotificationService) { }


  ngOnInit(): void {
    this.loadUserDetails();
    this.loadEarnings();
    this.usernameForm = this.fb.group({
      username: ['', Validators.required]
    });

    this.passwordForm = this.fb.group({
      oldPassword: ['', Validators.required],
      newPassword: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  ngAfterViewInit() {
    this.renderChart();
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

  loadEarnings() {
    this.analyticsService.getMyEarning().subscribe({
      next: (res: any) => {
        const data = Object.fromEntries(Object.entries(res.data).slice(1));
        const table = Object.entries(data).map(([event, amount]) => ({
          event,
          amount: Number(amount)
        }));
        this.earningsTableSignal.set(table);
        this.renderChart();
      },
      error: () => alert('Failed to load earnings.')
    });
  }

  renderChart() {
    if (!this.chartRef || !this.earningsTable().length) return;
    
    // Destroy previous chart if exists
    if (this.chartInstance) {
      this.chartInstance.destroy();
    }

    const ctx = this.chartRef.nativeElement.getContext('2d');
    if (!ctx) return;

    const labels = this.earningsTable().map(e => e.event);
    const data = this.earningsTable().map(e => e.amount);

    this.chartInstance = new Chart(ctx, {
      type: 'pie',
      data: {
        labels: labels,
        datasets: [{
          data: data,
          backgroundColor: [
            '#FF6384', '#36A2EB', '#FFCE56', '#4BC0C0', '#9966FF',
            '#FF9F40', '#8AC926', '#1982C4', '#6A4C93', '#F15BB5'
          ]}
        ]
      }
    });
  }
}