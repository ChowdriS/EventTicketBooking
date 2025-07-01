import { Component, OnInit, signal } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { EventService } from '../../../services/Event/event.service';
import { TicketTypeService } from '../../../services/TicketType/ticket-type.service';
import { ActivatedRoute, Route, Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AppEvent } from '../../../models/event.model';
import { EventStatus, EventTypeEnum } from '../../../models/enum';
import { ApiResponse } from '../../../models/api-response.model';
import { NotificationService } from '../../../services/Notification/notification-service';

@Component({
  selector: 'app-events-by-id',
  imports: [CommonModule, ReactiveFormsModule,RouterLink],
  templateUrl: './events-by-id.html',
  styleUrl: './events-by-id.css'
})
export class EventsById implements OnInit {
  eventId!: string;
  eventForm!: FormGroup;
  ticketTypeForm!: FormGroup;
  currentImageIndex = 0;
  prevTicketvalues = signal<any | null>(null);
  isEditingEvent = signal(false);
  isAddingTicketType = signal(false);
  ticketTypes = signal<any[]>([]);
  images = signal<any[]|null>([]);
  loading = signal(true);
  isImageEdit = signal(false);
  isImageAdd = signal(false);
  previousEventData = signal<AppEvent | null>(null);
  imageIntervalId: any;
  selectedFile: File | null = null;
  constructor(
    private fb: FormBuilder,
    private eventService: EventService,
    private ticketTypeService: TicketTypeService,
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
  toggleEventImage(){
    this.isImageEdit.set(!this.isImageEdit());
    this.isImageAdd.set(false);
  }
  toggleEventImageAdd(){
    this.isImageAdd.set(!this.isImageAdd());
    this.selectedFile = null;
    this.isImageEdit.set(false);
  }
  initForms(): void {
    this.eventForm = this.fb.group({
      title: ['', Validators.required],
      description: ['', Validators.required],
      eventDate: ['', Validators.required],
      eventType: ['', Validators.required],
      eventStatus: ['', Validators.required],
      location: ['', Validators.required],
      category: [-111, Validators.required],
    });

    this.ticketTypeForm = this.fb.group({
      id: [null],
      typeName: [null, Validators.required],
      price: ['', [Validators.required, Validators.min(1)]],
      totalQuantity: ['', [Validators.required, Validators.min(1)]],
      description: ['',Validators.required],
    });
  }
  deleteImage(image:any){
    // debugger;
    this.eventService.deleteEventImages(image).subscribe({
      next:()=>{
        this.notify.info("Image deleted!");
        this.loadEventData();
        this.isImageEdit.set(!this.isImageEdit());
      },
      error:(err:any)=>{

      }
    })
  }
  loadEventData() {
    this.loading.set(true);
    this.eventService.getEventById(this.eventId).subscribe({
      next: (res: any) => {

        this.eventForm.patchValue(res?.data);
        this.previousEventData.set(res.data);
        this.images.set(res.data?.images.$values);
        this.ticketTypes.set(res.data?.ticketTypes.$values || []);
        console.log(this.ticketTypes());
        console.log(this.previousEventData());
        console.log(this.eventForm.value);
        this.loading.set(false);
      },
      error: () => {
        this.notify.error('Failed to load event details');
        this.loading.set(false);
      }
    });
  }
  handleFileChange(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0];
    }
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
  toggleEditEvent() {
    this.isEditingEvent.set(!this.isEditingEvent());
    this.isImageEdit.set(false);
  }

  saveEvent() {
    if (this.eventForm.invalid) return;
    // console.log(this.previousEventData());
    const formValue = this.eventForm.value;
    const payload: any = {};
    payload.title = formValue.title !== this.previousEventData()?.title ? formValue.title : null;
    payload.description = formValue.description !== this.previousEventData()?.description ? formValue.description : null;
    payload.eventDate = formValue.eventDate !== this.previousEventData()?.eventDate ? formValue.eventDate : null;
    // payload.eventType = formValue.eventType !== this.previousEventData()?.eventType ? EventTypeEnum[formValue.eventType as keyof typeof EventTypeEnum] : null;
    payload.eventStatus = formValue.eventStatus !== this.previousEventData()?.eventStatus ? EventStatus[formValue.eventStatus as keyof typeof EventStatus] : null;
    console.log(payload);
    this.eventService.updateEvent(this.eventId, payload).subscribe({
        next: (res:ApiResponse) => {
          console.log(res);
          this.notify.success('Event updated successfully');
          this.isEditingEvent.set(false);
          this.loadEventData();
        },
        error: () => {
          this.notify.error('Failed to update event');
        }
      });
  }

  startAddTicketType() {
    this.isAddingTicketType.set(true);
    this.ticketTypeForm.reset();
    console.log(this.ticketTypeForm.value)
  }

  submitImage() {
    if (!this.selectedFile || !this.eventId) {
      this.notify.info('Please select a file');
      return;
    }

    const formData = new FormData();
    formData.append('image', this.selectedFile);

    this.eventService.uploadEventImage(this.eventId,formData).subscribe({
      next: (res:any) => {
        this.notify.success('Image uploaded successfully!');
        this.selectedFile = null;
        this.isImageAdd.set(false);
        this.loadEventData();
      },
      error: (err:any) => {
        this.notify.error('Image upload failed.');
      }
    });
  }

  submitTicketType() {
    if (this.ticketTypeForm.invalid) return;
    console.log(this.ticketTypeForm.value.totalQuantity)
    console.log(this.prevTicketvalues())
    if(this.ticketTypeForm.value.totalQuantity < this.prevTicketvalues().totalQuantity && 
      this.prevTicketvalues().bookedQuantity > this.ticketTypeForm.value.totalQuantity){
        this.notify.info("Caution! The updated quantity is lesser than the booked seats!");
        return;
    }
    // debugger;
    const ticketData = {
      ...this.ticketTypeForm.value,
      eventId: this.eventId,
    };
    ticketData.typeName = Number(ticketData.typeName);
    if (ticketData.id) {
      this.ticketTypeService.updateTicketType(ticketData.id, ticketData).subscribe({
        next: () => {
          this.notify.success('Ticket type updated');
          this.ticketTypeForm.reset();
          this.isAddingTicketType.set(false);
          this.loadEventData();
        },
        error: () => {
          this.notify.error('Failed to save ticket type');
        }
      });
    } else {
      this.ticketTypeService.addTicketType(ticketData).subscribe({
        next: () => {
          this.notify.success('Ticket type added');
          this.ticketTypeForm.reset();
          this.isAddingTicketType.set(false);
          this.loadEventData();
        },
        error: () => {
          this.notify.error('Failed to save ticket type');
        }
      });
    }
  }
  futureDateValidator(control: AbstractControl): ValidationErrors | null {
    const selectedDate = new Date(control.value);
    const now = new Date();
    return selectedDate > now ? null : { pastDate: true };
  }
  editTicketType(type: any) {
    this.ticketTypeForm.patchValue(type);
    this.prevTicketvalues.set(type);
    this.isAddingTicketType.set(true);
    console.log(this.ticketTypeForm.value)
  }

  deleteTicketType(typeId: string) {
    if (!confirm('Are you sure you want to delete this ticket type?')) return;
    this.ticketTypeService.deleteTicketType(typeId).subscribe({
      next: () => {
        this.notify.success('Ticket type deleted');
        this.loadEventData();
      },
      error: () => {
        this.notify.error('Failed to delete ticket type');
      }
    });
  }

  cancelEditTicketType() {
    this.ticketTypeForm.reset();
    this.isAddingTicketType.set(false);
    this.prevTicketvalues.set(null);
  }
}
