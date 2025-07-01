import { Component, signal } from '@angular/core';
import { EventService } from '../../../services/Event/event.service';
import { AppEvent } from '../../../models/event.model';
import { EventType, Router, RouterLink } from '@angular/router';
import { ApiResponse, PagedResponse } from '../../../models/api-response.model';
import { CommonModule } from '@angular/common';
import { EventStatus, EventTypeEnum, TicketTypeEnum } from '../../../models/enum';
import { Slider } from "../../slider/slider";
import { TopEvent } from "../../top-event/top-event";
import { NotificationService } from '../../../services/Notification/notification-service';

@Component({
  selector: 'app-front-page',
  imports: [CommonModule, Slider, TopEvent],
  templateUrl: './front-page.html',
  styleUrl: './front-page.css'
})
export class FrontPage {
  topEvent = signal<AppEvent | null>(null);
  images = signal<any | null>(null);
  currentIndex = signal<number>(0);
  intervalId: any;
  constructor(private eventsService: EventService,public router : Router ,private notify: NotificationService) {}
  
  ngOnInit() {
    this.fetchTopEvent();
    this.GetAllEventImages();
    this.startSlider();
  }
    routeToEvent(img:any){
    let eventId = img.eventId;
    this.router.navigate([this.router.url,'events', eventId]);
  }
  GetAllEventImages() {
    this.eventsService.getMyEventImages().subscribe({
      next:(res:any)=>{
        this.images.set(res.$values);
        // console.log(this.images());
      },
      error:(err:any)=>{
        console.log(err.message)
      }
    })
  }
  startSlider() {
    this.intervalId = setInterval(() => {
      const count = this.images()?.length || 0;
      if (count > 0) {
        const next = ((this.currentIndex()) + 1) % count;
        this.currentIndex.set(next);
      }
    }, 3500); 
  }

  goToSlide(index: number) {
    this.currentIndex.set(index);
  }
  getCurrentIndex(){
    return this.currentIndex();
  }
  isCancelled(event: AppEvent): boolean {
    return event.eventStatus.toString() === 'Cancelled';
  }
  fetchTopEvent() {
    this.eventsService.getManagerEvents(1, 1).subscribe({
      next: (res: ApiResponse<PagedResponse<any>>) => {
        const rawItem = res.data?.items?.$values || [];
        const parsedEvents = rawItem.map((e: any) => new AppEvent(e));
        this.topEvent.set(parsedEvents[0]);
      },
      error: () => this.notify.error("Failed to load events.")
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
