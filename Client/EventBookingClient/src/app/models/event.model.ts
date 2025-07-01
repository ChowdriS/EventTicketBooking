import { EventType } from "@angular/router";
import { BookedSeatStatus, EventCategory, EventStatus, EventTypeEnum, TicketTypeEnum } from "./enum";

export class AppEvent {
  id: string = '';
  title: string = '';
  description: string = '';
  location: string = '';
  images: any[] = [];
  eventDate: Date = new Date();
  eventStatus!: EventStatus;
  category!: EventCategory;
  eventType!: EventTypeEnum;
  ticketTypes: EventResponseTicketType[] = [];
  bookedSeats: EventResponseBookedSeat[] = [];

  constructor(init?: Partial<AppEvent>) {
    Object.assign(this, init);
    if (init?.eventDate) {
      this.eventDate = new Date(init.eventDate);
    }
    const input = init as any;
    this.ticketTypes = input?.ticketTypes?.$values || [];
    this.images = input?.images?.$values || [];
    this.bookedSeats = input?.bookedSeats?.$values || [];
  }
}

export interface EventResponseBookedSeat {
  seatNumber: number;
  bookedSeatStatus: BookedSeatStatus;
}

export interface EventResponseTicketType {
  id: string;
  typeName: TicketTypeEnum;
  price: number;
  totalQuantity: number;
  bookedQuantity: number;
  description: string;
  isDeleted: boolean;
  imageUrl: string;
}
