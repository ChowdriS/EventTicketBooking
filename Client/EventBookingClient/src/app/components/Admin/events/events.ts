import { Component, signal } from '@angular/core';
import { EventService } from '../../../services/Event/event.service';
import { ApiResponse, PagedResponse } from '../../../models/api-response.model';
import { AppEvent } from '../../../models/event.model';
import { Router, RouterLink } from '@angular/router';
import { CommonModule, DatePipe } from '@angular/common';
import {EventCategory, EventStatus, EventTypeEnum, TicketTypeEnum} from '../../../models/enum';
import { FormsModule } from '@angular/forms';
import { EventManager } from '@angular/platform-browser';
import { NotificationService } from '../../../services/Notification/notification-service';

@Component({
  selector: 'app-events',
  standalone: true,
  imports: [ DatePipe, CommonModule,FormsModule],
  templateUrl: './events.html',
  styleUrl: './events.css'
})
export class Events {
  events = signal<AppEvent[]>([]);
  pageNumber = signal(1);
  totalPages = signal(1);
  pageSize = 4;
  location: string = "";
  searchElement: string = '';
  filterDate: string = '';
  categoryOptions: { name: string; value: number }[] = [];
  selectedCategory: number = -111;
  cityOptions: { id: string; label: string }[] = [];

  constructor(private eventsService: EventService, private router: Router, private notify: NotificationService) {}

  ngOnInit() {
    this.loadEvents();
    this.loadCities();
        this.categoryOptions = Object.keys(EventCategory)
          .filter((key) => isNaN(Number(key)))
          .map((key) => ({
            name: key,
            value: EventCategory[key as keyof typeof EventCategory]
          }));
  }
  loadCities(){
    this.eventsService.getCities().subscribe({
      next:(res:ApiResponse)=>{
        const cities = res.data.$values;
        
        this.cityOptions = cities.map((city:any) => ({
          id: city.id,
          label: `${city.cityName}, ${city.stateName}`
        }));
      }
    });
  }

  isCancelled(event: AppEvent): boolean {
    return event.eventStatus.toString() == "Cancelled";
  }

  loadEvents() {
    console.log(this.location)
    if (this.filterDate) {
      this.filterDate = new Date(this.filterDate).toISOString();
    }
    if(this.location == 'Select a City'){
      this.location = '';
    }
    this.eventsService
      .getFilteredEvents(this.selectedCategory,this.location,this.searchElement, this.filterDate, this.pageNumber(), this.pageSize)
      .subscribe({
        next: (res: ApiResponse<PagedResponse<any>>) => {
          const rawItems = res.data?.items?.$values || [];
          // console.log(rawItems)
          this.events.set(rawItems.map((e: any) => new AppEvent(e)));
          this.totalPages.set(res.data?.totalPages || 1);
          this.filterDate = '';
          // console.log(this.events());
        },
        error: () => this.notify.error('Failed to load events.')
      });
  }
  getTotalBooked(event  : AppEvent){
    return event.ticketTypes.reduce((sum, ticket) => sum + (ticket.bookedQuantity), 0);
  }
  getTotalAvailable(event  : AppEvent){
    return event.ticketTypes.reduce((sum, ticket) => sum + (ticket.totalQuantity), 0);
  }
  GetEventById(event: AppEvent) {
    if (this.isCancelled(event)) {
      this.notify.success('The Event already deleted!');
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
