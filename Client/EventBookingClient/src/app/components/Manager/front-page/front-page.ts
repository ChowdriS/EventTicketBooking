import { Component, signal } from '@angular/core';
import { EventService } from '../../../services/Event/event.service';
import { AppEvent } from '../../../models/event.model';
import { EventType, Router, RouterLink } from '@angular/router';
import { ApiResponse, PagedResponse } from '../../../models/api-response.model';
import { CommonModule } from '@angular/common';
import { EventStatus, EventTypeEnum, TicketTypeEnum } from '../../../models/enum';

@Component({
  selector: 'app-front-page',
  imports: [RouterLink,CommonModule],
  templateUrl: './front-page.html',
  styleUrl: './front-page.css'
})
export class FrontPage {
  topEvent = signal<AppEvent | null>(null);
  constructor(private eventsService: EventService,public router : Router) {}
  
  ngOnInit() {
    this.fetchTopEvent();
  }
  
  fetchTopEvent() {
    this.eventsService.getManagerEvents(1, 1).subscribe({
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
