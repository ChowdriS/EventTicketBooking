export class AppEvent {
  id: string = "";
  title: string = "";
  description: string = "";
  eventDate: Date = new Date();
  eventStatus!: EventStatus;
  eventType!: EventType;

  constructor(init?: Partial<AppEvent>) {
    Object.assign(this, init);
    if (init?.eventDate) {
      this.eventDate = new Date(init.eventDate);
    }
  }
}

