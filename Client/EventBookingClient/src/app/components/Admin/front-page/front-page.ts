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
import { Slider } from "../../slider/slider";
import { TopEvent } from "../../top-event/top-event";
import { NotificationService } from '../../../services/Notification/notification-service';

@Component({
  selector: 'app-front-page',
  imports: [RouterLink, CommonModule, Slider, TopEvent],
  templateUrl: './front-page.html',
  styleUrl: './front-page.css'
})
export class FrontPage {
  role : string = '';
  users = signal<any[]|null>([]);
  topEvent = signal<AppEvent | null>(null);
  images = signal<any | null>(null);
  currentIndex = signal<number>(0);
  intervalId: any;
  constructor(private eventsService: EventService,public router : Router, private userService : UserService, private notify: NotificationService) {}
  
  ngOnInit() {
    this.fetchTopEvent();
    this.fetchUsers();
    this.GetAllEventImages();
    this.startSlider();
  }
  fetchUsers() {
    this.userService.getAllUsers().subscribe({
      next: (res) => {
        this.users.set(res.data.$values);
      },
      error: () => this.notify.error('Failed to fetch users'),
    });
  }
    getCurrentIndex(){
    return this.currentIndex();
  }
  isCancelled(event: AppEvent): boolean {
    return event.eventStatus.toString() === 'Cancelled';
  }
  ngOnDestroy() {
    clearInterval(this.intervalId);
  }
  GetEventById(event: AppEvent) {
    if (this.isCancelled(event)) {
      this.notify.success('The Event is Cancelled! Try a different Event!');
    } else {
      this.router.navigate([this.router.url,'events', event.id]);
    }
  }
  routeToEvent(img:any){
    let eventId = img.eventId;
    this.router.navigate([this.router.url,'events', eventId]);
  }
  GetAllEventImages() {
    this.eventsService.getAllEventImages().subscribe({
      next:(res:any)=>{
        this.images.set(res.$values);
        console.log(this.images());
      },
      error:(err:any)=>{

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
  deleteUser(user : any) {
    console.log(user);
    if (confirm(`Are you sure you want to delete ${user.email}?`)) {
      this.userService.deleteUser(user.id).subscribe({
        next: () => {
          this.notify.success('User deleted successfully!');
          this.fetchUsers();
        },
        error: () => this.notify.error('Failed to delete user'),
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
