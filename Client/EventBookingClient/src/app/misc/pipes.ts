import { Pipe, PipeTransform } from '@angular/core';
import { EventStatus, TicketTypeEnum } from '../models/enum';
import { EventType } from '@angular/router';

@Pipe({ name: 'eventStatus' })
export class EventStatusPipe implements PipeTransform {
  transform(value: number): string {
    return EventStatus[value] ?? 'Unknown';
  }
}

@Pipe({ name: 'eventType' })
export class EventTypePipe implements PipeTransform {
  transform(value: number): string {
    return EventType[value] ?? 'Unknown';
  }
}

@Pipe({ name: 'ticketType' })
export class TicketTypePipe implements PipeTransform {
  transform(value: number): string {
    return TicketTypeEnum[value] ?? 'Unknown';
  }
}
