import { Component, Input, input, OnInit, signal } from '@angular/core';
import { EventService } from '../../services/Event/event.service';
import { Router, RouterLink } from '@angular/router';
import { EventStatus, EventTypeEnum, TicketTypeEnum } from '../../models/enum';
import { AppEvent } from '../../models/event.model';
import { DatePipe } from '@angular/common';
import { Auth } from '../../services/Auth/auth';
import { Getrole } from '../../misc/Token';

@Component({
  selector: 'app-top-event',
  imports: [RouterLink, DatePipe],
  templateUrl: './top-event.html',
  styleUrl: './top-event.css',
  standalone : true
})
export class TopEvent implements OnInit {
  @Input() topEvent = signal<AppEvent | null>(null);
  role : string = '';
  constructor(private auth: Auth,public router : Router) {}
  
  ngOnInit() {
    this.role = Getrole(this.auth.getToken());
    // console.log("inside top event");
  }
  routeToEvent(img:any){
    let eventId = img.eventId;
    this.router.navigate([this.router.url,'events', eventId]);
  }

  isCancelled(event: AppEvent): boolean {
    return event.eventStatus.toString() === 'Cancelled';
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
