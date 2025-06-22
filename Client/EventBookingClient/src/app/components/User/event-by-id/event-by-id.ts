import { Component, NgModule, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, NgForm, NgModel, ReactiveFormsModule, Validators } from '@angular/forms';
import { AppEvent, EventResponseTicketType } from '../../../models/event.model';
import { EventService } from '../../../services/Event/event.service';
import { v4 as uuidv4 } from 'uuid';
import { EventStatus, EventTypeEnum, PaymentTypeEnum, TicketTypeEnum } from '../../../models/enum';
import { TicketService } from '../../../services/Ticket/ticket.service';
import { ApiResponse } from '../../../models/api-response.model';
import { CommonModule, DatePipe, KeyValuePipe } from '@angular/common';
import { signal, effect } from '@angular/core';

@Component({
  selector: 'app-event-by-id',
  templateUrl: './event-by-id.html',
  styleUrl: './event-by-id.css',
  standalone : true,
  imports : [ReactiveFormsModule,DatePipe,CommonModule]
})
export class EventById implements OnInit {
  event = signal<AppEvent | null>(null);
  eventId!: string;
  form!: FormGroup;
  selectedTicketType = signal<EventResponseTicketType | undefined>(undefined);
  availableSeats = signal<number[]>([]);
  bookedSeatNumbers = signal<number[]>([]);

  PaymentTypeEnum = PaymentTypeEnum;
  paymentTypes = Object.entries(PaymentTypeEnum).filter(([k, v]) => !isNaN(Number(v)));

  constructor(
    private route: ActivatedRoute,
    private eventService: EventService,
    private ticketService: TicketService,
    private fb: FormBuilder,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.eventId = this.route.snapshot.paramMap.get('id')!;
    this.loadEvent();
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

  loadEvent() {
    this.eventService.getEventById(this.eventId).subscribe((res: ApiResponse) => {
      const evt = new AppEvent(res.data);
      this.event.set(evt);
      this.bookedSeatNumbers.set(
        evt.bookedSeats.filter((s) => s.bookedSeatStatus === 0).map((s) => s.seatNumber)
      );

      this.form = this.fb.group({
        ticketTypeId: [null, Validators.required],
        quantity: [1, [Validators.required, Validators.min(1)]],
        paymentType: [null, Validators.required],
        seatNumbers: [[]]
      });

      // Initialize selected ticket type based on form changes
      this.form.get('ticketTypeId')?.valueChanges.subscribe(id => {
        const ticketType = evt.ticketTypes.find(t => t.id === id);
        this.selectedTicketType.set(ticketType);
        this.form.get('seatNumbers')?.setValue([]);
        this.form.get('quantity')?.setValue(1);
      });

      // Set initial value for ticketTypeId to trigger first selection
      if (evt.ticketTypes.length > 0) {
        this.form.get('ticketTypeId')?.setValue(evt.ticketTypes[0].id);
      }
    });
  }

  toggleSeat(seat: number) {
    const currentSeats = this.form.value.seatNumbers as number[];
    const index = currentSeats.indexOf(seat);

    if (index > -1) {
      currentSeats.splice(index, 1);
    } else if (currentSeats.length < this.form.value.quantity) {
      currentSeats.push(seat);
    }

    this.form.get('seatNumbers')?.setValue([...currentSeats]);
  }

  isSeatBooked(seat: number): boolean {
    return this.bookedSeatNumbers().includes(seat);
  }

  isSeatSelected(seat: number): boolean {
    return this.form.value.seatNumbers?.includes(seat);
  }

  generateSeats(total: number): number[] {
    return Array.from({ length: total }, (_, i) => i + 1);
  }

  submit() {
  if (this.form.invalid) return;

  const evt = this.event();
  if (!evt) return;

  const isSeatable = EventTypeEnum[evt.eventType] === 'Seatable';

  const payload = {
    EventId: evt.id,
    TicketTypeId: this.form.value.ticketTypeId,
    Quantity: this.form.value.quantity,
    SeatNumbers: isSeatable ? this.form.value.seatNumbers : null,
    Payment: {
      PaymentType: this.form.value.paymentType,
      TransactionId: uuidv4(),
    }
  };

  // console.log(payload)
  this.ticketService.bookTicket(payload).subscribe({
    next: () => this.router.navigate(['/user']),
    error: () => alert('Booking failed. Try again.'),
  });
}

}

