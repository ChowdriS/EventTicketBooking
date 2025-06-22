import { Component, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { EventService } from '../../../services/Event/event.service';
import { TicketTypeService } from '../../../services/TicketType/ticket-type.service';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-events-by-id',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './events-by-id.html',
  styleUrl: './events-by-id.css'
})
export class EventsById implements OnInit {
  eventId!: string;
  eventForm!: FormGroup;
  ticketTypeForm!: FormGroup;

  isEditingEvent = signal(false);
  isAddingTicketType = signal(false);
  ticketTypes = signal<any[]>([]);
  loading = signal(true);

  constructor(
    private fb: FormBuilder,
    private eventService: EventService,
    private ticketTypeService: TicketTypeService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.eventId = this.route.snapshot.paramMap.get('id')!;
    this.initForms();
    this.loadEventData();
  }

  initForms(): void {
    this.eventForm = this.fb.group({
      title: ['', Validators.required],
      description: [''],
      eventDate: ['', Validators.required],
      eventType: ['', Validators.required],
      eventStatus: ['', Validators.required],
    });

    this.ticketTypeForm = this.fb.group({
      id: [null],
      typeName: ['', Validators.required],
      price: ['', [Validators.required, Validators.min(1)]],
      totalQuantity: ['', [Validators.required, Validators.min(1)]],
      description: [''],
    });
  }

  loadEventData() {
    this.loading.set(true);
    this.eventService.getEventById(this.eventId).subscribe({
      next: (res: any) => {
        this.eventForm.patchValue(res?.data);
        this.ticketTypes.set(res.data?.ticketTypes.$values || []);
        this.loading.set(false);
      },
      error: () => {
        alert('Failed to load event details');
        this.loading.set(false);
      }
    });
  }

  toggleEditEvent() {
    this.isEditingEvent.set(!this.isEditingEvent());
  }

  saveEvent() {
    if (this.eventForm.invalid) return;
    this.eventService.updateEvent(this.eventId, this.eventForm.value).subscribe({
      next: () => {
        alert('Event updated successfully');
        this.isEditingEvent.set(false);
      },
      error: () => {
        alert('Failed to update event');
      }
    });
  }

  startAddTicketType() {
    this.isAddingTicketType.set(true);
    this.ticketTypeForm.reset();
  }

  submitTicketType() {
    if (this.ticketTypeForm.invalid) return;
    const ticketData = {
      ...this.ticketTypeForm.value,
      eventId: this.eventId,
    };
    ticketData.typeName = Number(ticketData.typeName);
    if (ticketData.id) {
      this.ticketTypeService.updateTicketType(ticketData.id, ticketData).subscribe({
        next: () => {
          alert('Ticket type updated');
          this.ticketTypeForm.reset();
          this.isAddingTicketType.set(false);
          this.loadEventData();
        },
        error: () => {
          alert('Failed to save ticket type');
        }
      });
    } else {
      this.ticketTypeService.addTicketType(ticketData).subscribe({
        next: () => {
          alert('Ticket type added');
          this.ticketTypeForm.reset();
          this.isAddingTicketType.set(false);
          this.loadEventData();
        },
        error: () => {
          alert('Failed to save ticket type');
        }
      });
    }
  }

  editTicketType(type: any) {
    this.ticketTypeForm.patchValue(type);
    this.isAddingTicketType.set(true);
  }

  deleteTicketType(typeId: string) {
    if (!confirm('Are you sure you want to delete this ticket type?')) return;
    this.ticketTypeService.deleteTicketType(typeId).subscribe({
      next: () => {
        alert('Ticket type deleted');
        this.loadEventData();
      },
      error: () => {
        alert('Failed to delete ticket type');
      }
    });
  }

  cancelEditTicketType() {
    this.ticketTypeForm.reset();
    this.isAddingTicketType.set(false);
  }
}
