import { Component, Signal, signal } from '@angular/core';
import { EventService } from '../../../services/Event/event.service';
import { AppEvent } from '../../../models/event.model';
import { EventType, Router, RouterLink } from '@angular/router';
import { ApiResponse, PagedResponse } from '../../../models/api-response.model';
import { CommonModule } from '@angular/common';
import { EventStatus, EventTypeEnum, TicketTypeEnum } from '../../../models/enum';
import { UserService } from '../../../services/User/user-service';
import { Getrole } from '../../../misc/Token';
import { Auth } from '../../../services/Auth/auth';

@Component({
  selector: 'app-front-page',
  imports: [RouterLink,CommonModule],
  templateUrl: './front-page.html',
  styleUrl: './front-page.css'
})
export class FrontPage {
  role : string = '';
  users = signal<any[]|null>([]);
  topEvent = signal<AppEvent | null>(null);
  constructor(private eventsService: EventService,public router : Router, private userService : UserService) {}
  
  ngOnInit() {
    this.fetchTopEvent();
    this.fetchUsers();
  }
  fetchUsers() {
    this.userService.getAllUsers().subscribe({
      next: (res) => {
        this.users.set(res.data.$values);
      },
      error: () => alert('Failed to fetch users'),
    });
  }

  deleteUser(user : any) {
    console.log(user);
    if (confirm(`Are you sure you want to delete ${user.email}?`)) {
      this.userService.deleteUser(user.id).subscribe({
        next: () => {
          alert('User deleted successfully!');
          this.fetchUsers();
        },
        error: () => alert('Failed to delete user'),
      });
    }
  }
  fetchTopEvent() {
    this.eventsService.getEvents(1, 1).subscribe({
      next: (res: ApiResponse<PagedResponse<any>>) => {
        const rawItem = res.data?.items?.$values || [];
        const parsedEvents = rawItem.map((e: any) => new AppEvent(e));
        this.topEvent.set(parsedEvents[0]);
      },
      error: () => alert("Failed to load events.")
    });
  }
  eventStatusToString(status: number): string {
    return EventStatus[status];
  }

  eventTypeToString(type: number): string {
    return EventTypeEnum[type];
  }

  ticketTypeToString(type: number): string {
    return TicketTypeEnum[type];
  }
}
