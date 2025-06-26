import { Component, signal } from '@angular/core';
import { EventService } from '../../../services/Event/event.service';
import { ApiResponse, PagedResponse } from '../../../models/api-response.model';
import { AppEvent } from '../../../models/event.model';
import { Router } from '@angular/router';
import { CommonModule, DatePipe } from '@angular/common';
import { EventStatus, EventTypeEnum, TicketTypeEnum } from '../../../models/enum';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-events',
  standalone: true,
  imports: [CommonModule, FormsModule, DatePipe],
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

  constructor(private eventsService: EventService, private router: Router) {}

  ngOnInit() {
    this.loadEvents();
  }

  isCancelled(event: AppEvent): boolean {
    return event.eventStatus.toString() === 'Cancelled';
  }

  loadEvents() {
    if (this.filterDate) {
      this.filterDate = new Date(this.filterDate).toISOString();
    }

    this.eventsService
      .getFilteredEvents(this.searchElement, this.filterDate, this.pageNumber(), this.pageSize)
      .subscribe({
        next: (res: ApiResponse<PagedResponse<any>>) => {
          const rawItems = res.data?.items?.$values || [];
          console.log(rawItems)
          this.events.set(rawItems.map((e: any) => new AppEvent(e)));
          this.totalPages.set(res.data?.totalPages || 1);
          this.filterDate = '';
          console.log(this.events());
        },
        error: () => alert('Failed to load events.')
      });
  }

  GetEventById(event: AppEvent) {
    if (this.isCancelled(event)) {
      alert('The Event is Cancelled! Try a different Event!');
    } else {
      this.router.navigate([this.router.url, event.id]);
    }
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
