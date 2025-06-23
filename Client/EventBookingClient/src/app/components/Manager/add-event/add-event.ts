import { Component, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { EventService } from '../../../services/Event/event.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-add-event',
  imports: [ReactiveFormsModule,CommonModule],
  templateUrl: './add-event.html',
  styleUrl: './add-event.css'
})
export class AddEvent implements OnInit {
  eventForm!: FormGroup;
  ticketTypeForm!: FormGroup;

  ticketTypes = signal<any[]>([]);
  isAddingTicketType = signal(false);

  constructor(private fb: FormBuilder, private eventService: EventService, public router: Router) {}

  ngOnInit(): void {
    this.initForms();
  }

  initForms(): void {
    this.eventForm = this.fb.group({
      title: ['', Validators.required],
      description: [''],
      eventDate: ['', Validators.required],
      eventType: ['NonSeatable', Validators.required],
    });

    this.ticketTypeForm = this.fb.group({
      id: [null],
      typeName: [0, Validators.required],
      price: [0, [Validators.required, Validators.min(1)]],
      totalQuantity: [0, [Validators.required, Validators.min(1)]],
      description: [''],
    });
  }

  startAddTicketType() {
    this.ticketTypeForm.reset({ typeName: 0 });
    this.isAddingTicketType.set(true);
  }

  addTicketType() {
    if (this.ticketTypeForm.invalid) return;
    const ticket = { ...this.ticketTypeForm.value, id: crypto.randomUUID() };
    const current = this.ticketTypes();
    this.ticketTypes.set([...current, ticket]);
    this.ticketTypeForm.reset();
    this.isAddingTicketType.set(false);
  }

  editTicketType(t: any) {
    this.ticketTypeForm.patchValue(t);
    this.isAddingTicketType.set(true);
  }

  deleteTicketType(id: string) {
    this.ticketTypes.set(this.ticketTypes().filter((t) => t.id !== id));
  }

  cancelEditTicketType() {
    this.ticketTypeForm.reset();
    this.isAddingTicketType.set(false);
  }

  submitEvent() {
    if (this.eventForm.invalid || this.ticketTypes().length === 0) {
      alert('Please complete the form and add at least one ticket type.');
      return;
    }

    const payload = {
      ...this.eventForm.value,
      eventType: this.eventForm.value.eventType === 'Seatable' ? 0 : 1,
      ticketTypes: this.ticketTypes().map((t) => ({
        typeName: Number(t.typeName),
        price: Number(t.price),
        totalQuantity: Number(t.totalQuantity),
        description: t.description,
      })),
    };
    console.log(payload);
    payload.eventDate = new Date(payload.eventDate).toISOString();
    console.log(payload);
    this.eventService.addEvent(payload).subscribe({
      next: (res: any) => {
        alert('Event created successfully!');
        this.router.navigate([this.router.url, res.data.id]);
      },
      error: () => alert('Failed to create event'),
    });
  }
}
