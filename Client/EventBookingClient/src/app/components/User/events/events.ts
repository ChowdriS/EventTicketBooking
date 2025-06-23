import { Component, signal } from '@angular/core';
import { EventService } from '../../../services/Event/event.service';
import { ApiResponse, PagedResponse } from '../../../models/api-response.model';
import { AppEvent } from '../../../models/event.model';
import { RouterLink } from '@angular/router';
import { CommonModule, DatePipe } from '@angular/common';
import {EventStatus, EventTypeEnum, TicketTypeEnum} from '../../../models/enum';
import { EventStatusPipe, EventTypePipe, TicketTypePipe } from '../../../misc/pipes';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-events',
  standalone: true,
  imports: [RouterLink, DatePipe, CommonModule,FormsModule],
  templateUrl: './events.html',
  styleUrl: './events.css'
})
export class Events {
  events = signal<AppEvent[]>([]);
  pageNumber = signal(1);
  totalPages = signal(1);
  pageSize = 4;
  searchElement: string = '';
  filterDate: string = '';

  constructor(private eventsService: EventService) {}

  ngOnInit() {
    this.loadEvents();
  }

  loadEvents() {
    if(this.filterDate != ''){
      let dates = new Date(this.filterDate).toISOString();
      this.filterDate = dates;
    }
    this.eventsService
      .getFilteredEvents(this.searchElement, this.filterDate, this.pageNumber(), this.pageSize)
      .subscribe({
        next: (res: ApiResponse<PagedResponse<any>>) => {
          const rawItems = res.data?.items?.$values || [];
          this.events.set(rawItems.map((e: any) => new AppEvent(e)));
          this.totalPages.set(res.data?.totalPages || 1);
          this.filterDate = '';
        },
        error: () => alert("Failed to load events.")
      });
  }

  goToPage(page: number) {
    if (page >= 1 && page <= this.totalPages()) {
      this.pageNumber.set(page);
      this.loadEvents();
    }
  }
  onFilterChange() {
    this.pageNumber.set(1);
    this.loadEvents();
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
