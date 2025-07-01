import { Component, signal } from '@angular/core';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { UserService } from '../../../services/User/user-service';
import { User } from '../../../models/user.model';
import { ApiResponse } from '../../../models/api-response.model';
import { SignalRService } from '../../../services/Notification/signalr-service';
import { Navbar } from "../../navbar/navbar";
import { NotificationService } from '../../../services/Notification/notification-service';

@Component({
  selector: 'app-home',
  imports: [RouterOutlet, Navbar],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class Home {
  user = signal<User | null>(null);
  constructor(public router : Router,private userService:UserService,private signalrService : SignalRService,  private notify: NotificationService) {}

  logout() {
    localStorage.removeItem('token');
    this.router.navigate(['/login']);
    this.signalrService.stopConnection();
  }
  ngOnInit(): void {
    this.getMyDetail();
  }

  getMyDetail() {
    this.userService.getUserDetails().subscribe({
      next: (res: ApiResponse) => {
        this.user.set(res.data);
      },
      error: (err: any) => {
        this.notify.error("Failed to fetch your Data");
      }
    });
  }
}
