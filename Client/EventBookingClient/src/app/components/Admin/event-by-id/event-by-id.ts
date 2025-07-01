import { Component, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { EventService } from '../../../services/Event/event.service';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AppEvent } from '../../../models/event.model';
import { ApiResponse } from '../../../models/api-response.model';
import { NotificationService } from '../../../services/Notification/notification-service';

@Component({
  selector: 'app-event-by-id',
  imports: [CommonModule, ReactiveFormsModule,RouterLink],
  templateUrl: './event-by-id.html',
  styleUrl: './event-by-id.css'
})
export class EventById implements OnInit {
  eventId!: string;
  currentImageIndex = 0;
  eventForm!: FormGroup;
  ticketTypeForm!: FormGroup;
  ticketTypes = signal<any[]>([]);
  loading = signal(true);
  status = signal("");
  imageIntervalId: any;
  images = signal<any[]|null>([]);


  previousEventData = signal<AppEvent | null>(null);

  constructor(
    private fb: FormBuilder,
    private eventService: EventService,
    private route: ActivatedRoute,
    public router : Router,
    private notify: NotificationService
  ) { }

  ngOnInit(): void {
    this.eventId = this.route.snapshot.paramMap.get('id')!;
    this.initForms();
    this.loadEventData();
    this.startImageSlider();
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
  startImageSlider() {
    this.imageIntervalId = setInterval(() => {
      const images = this.images();
      if (images && images.length > 1) {
        this.currentImageIndex = (this.currentImageIndex + 1) % images.length;
      }
    }, 1500); 
  }
  isCancelled(event: AppEvent): boolean {
    return event.eventStatus.toString() == "Cancelled";
  }
  loadEventData() {
    this.loading.set(true);
    this.eventService.getEventById(this.eventId).subscribe({
      next: (res: any) => {
        console.log(res.data);
        this.eventForm.patchValue(res?.data);
        this.status.set(res?.data.eventStatus);
        this.images.set(res.data?.images.$values);
        this.previousEventData.set(res.data);
        this.ticketTypes.set(res.data?.ticketTypes.$values || []);
        this.loading.set(false);
      },
      error: () => {
        this.notify.error('Failed to load event details');
        this.loading.set(false);
      }
    });
  }

  deleteEvent(id:string) {
    this.eventService.deleteEvent(id).subscribe({
      next:(res:ApiResponse)=>{
        this.notify.success("Successfully Deleted!");
        this.router.navigate(["admin/events"]);
      },
      error:(err:ApiResponse)=>{
        this.notify.success(err.message);
      }
    })
  }
}
