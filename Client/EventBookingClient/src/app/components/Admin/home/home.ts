import { Component, OnInit, signal } from '@angular/core';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { UserService } from '../../../services/User/user-service';
import { ApiResponse } from '../../../models/api-response.model';
import { User } from '../../../models/user.model';
import { Navbar } from "../../navbar/navbar";
import { NotificationService } from '../../../services/Notification/notification-service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RouterOutlet, Navbar],
  templateUrl: './home.html',
  styleUrls: ['./home.css']
})
export class Home implements OnInit {
  user = signal<User | null>(null);

  constructor(public router: Router, private userService: UserService, private notify: NotificationService) {}

  logout() {
    localStorage.removeItem('token');
    this.router.navigate(['/login']);
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
