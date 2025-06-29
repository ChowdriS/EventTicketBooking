import { Component, Input, OnInit } from '@angular/core';
import { AppEvent } from '../../models/event.model';
import { CommonModule, DatePipe } from '@angular/common';
import { Route, Router } from '@angular/router';
import { TicketTypeEnum } from '../../models/enum';

@Component({
  selector: 'app-similar-events',
  imports: [DatePipe,CommonModule],
  templateUrl: './similar-events.html',
  styleUrl: './similar-events.css'
})
export class SimilarEvents{
  @Input() data! : AppEvent[];
  constructor(private router : Router){}
  GetEventById(event: AppEvent) {
    if (this.isCancelled(event)) {
      alert('The Event is Cancelled! Try a different Event!');
    } else {
      const role = this.router.url.split('/')[1]; 
      const url = `/${role}/events/${event.id}`;
      this.router.navigateByUrl(url).then(() => {
        window.scrollTo({ top: 0, behavior: 'smooth' });
      });
    }
  }
  getTotalBooked(event  : AppEvent){
    return event.ticketTypes.reduce((sum, ticket) => sum + (ticket.bookedQuantity), 0);
  }
  getTotalAvailable(event  : AppEvent){
    return event.ticketTypes.reduce((sum, ticket) => sum + (ticket.totalQuantity), 0);
  }
  isCancelled(event: AppEvent): boolean {
    return event.eventStatus.toString() == "Cancelled";
  }
  ticketTypeToString(type: number): string {
    return TicketTypeEnum[type];
  }
}
