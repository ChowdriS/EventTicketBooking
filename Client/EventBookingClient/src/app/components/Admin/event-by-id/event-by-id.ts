import { Component, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { EventService } from '../../../services/Event/event.service';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AppEvent } from '../../../models/event.model';

@Component({
  selector: 'app-event-by-id',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './event-by-id.html',
  styleUrl: './event-by-id.css'
})
export class EventById implements OnInit {
  eventId!: string;
  eventForm!: FormGroup;
  ticketTypeForm!: FormGroup;

  isEditingEvent = signal(false);
  isAddingTicketType = signal(false);
  ticketTypes = signal<any[]>([]);
  loading = signal(true);
  previousEventData = signal<AppEvent | null>(null);

  constructor(
    private fb: FormBuilder,
    private eventService: EventService,
    private route: ActivatedRoute,
    public router : Router
  ) { }

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
        this.previousEventData.set(res.data);
        this.ticketTypes.set(res.data?.ticketTypes.$values || []);
        this.loading.set(false);
      },
      error: () => {
        alert('Failed to load event details');
        this.loading.set(false);
      }
    });
  }
}
