import { Component, signal } from '@angular/core';
import { EventService } from '../../../services/Event/event.service';
import { ApiResponse, PagedResponse } from '../../../models/api-response.model';
import { AppEvent } from '../../../models/event.model';
import { Router, RouterLink } from '@angular/router';
import { CommonModule, DatePipe } from '@angular/common';
import {EventStatus, EventTypeEnum, TicketTypeEnum} from '../../../models/enum';
import { FormsModule } from '@angular/forms';
import { NotificationService } from '../../../services/Notification/notification-service';

@Component({
  selector: 'app-events',
  standalone: true,
  imports: [DatePipe, CommonModule,RouterLink,FormsModule],
  templateUrl: './events.html',
  styleUrl: './events.css'
})
export class Events {
  events = signal<AppEvent[]>([]);
  pageNumber = signal(1);
  totalPages = signal(1);
  pageSize = 4;

  constructor(private eventsService: EventService,public router: Router, private notify: NotificationService) {}

  ngOnInit() {
    this.loadEvents();
  }
  isCancelled(event: AppEvent): boolean {
    return event.eventStatus.toString() == "Cancelled";
  }
  getTotalBooked(event  : AppEvent){
    return event.ticketTypes.reduce((sum, ticket) => sum + (ticket.bookedQuantity), 0);
  }
  getTotalAvailable(event  : AppEvent){
    return event.ticketTypes.reduce((sum, ticket) => sum + (ticket.totalQuantity), 0);
  }
  loadEvents() {
    this.eventsService.getManagerEvents(this.pageNumber(), this.pageSize).subscribe({
      next: (res: ApiResponse<PagedResponse<any>>) => {
        const rawItems = res.data?.items?.$values || [];

        const parsedEvents = rawItems.map((e: any) => new AppEvent(e));
        this.events.set(parsedEvents);
        console.log(this.events());
        this.totalPages.set(res.data?.totalPages || 1);
      },
      error: () => this.notify.error("Failed to load events.")
    });
  }
  GetEventById(event:AppEvent){
    if(this.isCancelled(event)){
      this.notify.error("The Event is Cancelled! Try different Event!");
    }
    else{
      this.router.navigate([this.router.url,event.id]);
    }
  }
  goToPage(page: number) {
    if (page >= 1 && page <= this.totalPages()) {
      this.pageNumber.set(page);
      this.loadEvents();
    }
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
