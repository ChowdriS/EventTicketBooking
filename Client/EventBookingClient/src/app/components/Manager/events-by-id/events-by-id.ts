import { Component, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { EventService } from '../../../services/Event/event.service';
import { TicketTypeService } from '../../../services/TicketType/ticket-type.service';
import { ActivatedRoute, Route, Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AppEvent } from '../../../models/event.model';
import { EventStatus, EventTypeEnum } from '../../../models/enum';

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
    public router : Router
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
  deleteImage(image:any){
    // debugger;
    this.eventService.deleteEventImages(image).subscribe({
      next:()=>{
        alert("Image deleted!");
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
        // console.log(this.images());
        this.loading.set(false);
      },
      error: () => {
        alert('Failed to load event details');
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
  }

  saveEvent() {
    if (this.eventForm.invalid) return;
    console.log(this.previousEventData());
    const formValue = this.eventForm.value;
    const payload: any = {};
    payload.title = formValue.title !== this.previousEventData()?.title ? formValue.title : null;
    payload.description = formValue.description !== this.previousEventData()?.description ? formValue.description : null;
    payload.eventDate = formValue.eventDate !== this.previousEventData()?.eventDate ? formValue.eventDate : null;
    payload.eventType = formValue.eventType !== this.previousEventData()?.eventType ? EventTypeEnum[formValue.eventType as keyof typeof EventTypeEnum] : null;
    payload.eventStatus = formValue.eventStatus !== this.previousEventData()?.eventStatus ? EventStatus[formValue.eventStatus as keyof typeof EventStatus] : null;
      this.eventService.updateEvent(this.eventId, payload).subscribe({
        next: () => {
          alert('Event updated successfully');
          this.isEditingEvent.set(false);
          this.loadEventData();
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

  submitImage() {
    if (!this.selectedFile || !this.eventId) {
      alert('Please select a file');
      return;
    }

    const formData = new FormData();
    formData.append('image', this.selectedFile);

    this.eventService.uploadEventImage(this.eventId,formData).subscribe({
      next: (res:any) => {
        alert('Image uploaded successfully!');
        this.selectedFile = null;
        this.isImageAdd.set(false);
        this.loadEventData();
      },
      error: (err:any) => {
        alert('Image upload failed.');
      }
    });
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
