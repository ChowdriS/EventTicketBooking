import { Component, OnInit, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { UserService } from '../../services/User/user-service';
import { User } from '../../models/user.model';
import { ApiResponse } from '../../models/api-response.model';
import { Getrole } from '../../misc/Token';
import { Auth } from '../../services/Auth/auth';
import { NotificationService } from '../../services/Notification/notification-service';

@Component({
  selector: 'app-navbar',
  imports: [RouterLink],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css',
  standalone : true
})
export class Navbar implements OnInit {
  user = signal<User | null>(null);
  // role :string = "";
  constructor(public router: Router, private userService: UserService, private auth : Auth,  private notify: NotificationService) {}

  logout() {
    localStorage.removeItem('token');
    this.router.navigate(['/login']);
  }

  ngOnInit(): void {
    this.getMyDetail();
    // this.role = Getrole(this.auth.getToken());
  }
  routeToProfile(){
    this.router.navigate(['profile']);
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
