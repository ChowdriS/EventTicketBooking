import { Component, signal } from '@angular/core';
import { EventService } from '../../../services/Event/event';
import { ApiResponse, PagedResponse } from '../../../models/api-response.model';
import { AppEvent } from '../../../models/event.model';
import { RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-events',
  imports: [RouterLink,DatePipe],
  templateUrl: './events.html',
  styleUrl: './events.css'
})
export class Events {
  events = signal<AppEvent[]>([]);
  pageNumber = signal(1);
  totalPages = signal(1);
  pageSize = 4;

  constructor(private eventsService: EventService) {}

  ngOnInit() {
    this.loadEvents();
  }

  loadEvents() {
    this.eventsService.getEvents(this.pageNumber(), this.pageSize).subscribe({
      next: (res: ApiResponse<PagedResponse<AppEvent>>) => {
        const values = (res.data?.items as any)?.$values as AppEvent[];
        this.events.set(values || []);
        this.totalPages.set(res.data?.totalPages || 1);
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
}
