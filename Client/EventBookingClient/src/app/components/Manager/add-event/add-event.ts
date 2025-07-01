import { Component, OnInit, signal } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { EventService } from '../../../services/Event/event.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ApiResponse } from '../../../models/api-response.model';
import { EventCategory } from '../../../models/enum';
import { NotificationService } from '../../../services/Notification/notification-service';

@Component({
  selector: 'app-add-event',
  imports: [ReactiveFormsModule,CommonModule,FormsModule],
  templateUrl: './add-event.html',
  styleUrl: './add-event.css'
})
export class AddEvent implements OnInit {
  eventForm!: FormGroup;
  location : string = '';
  ticketTypeForm!: FormGroup;
  categoryOptions: { name: string; value: number }[] = [];
  selectedCategory: number = -111;
  cityOptions: { id: string; label: string }[] = [];
  selectedFile: File | null = null;
  ticketTypes = signal<any[]>([]);
  isAddingTicketType = signal(false);

  constructor(private fb: FormBuilder, private eventService: EventService, public router: Router, private notify: NotificationService) {}

  ngOnInit(): void {
    this.initForms();
    this.loadCities();
    this.categoryOptions = Object.keys(EventCategory)
      .filter((key) => isNaN(Number(key)))
      .map((key) => ({
        name: key,
        value: EventCategory[key as keyof typeof EventCategory]
      }));
  }
  handleFileChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0];
      this.eventForm.get('image')?.setValue(this.selectedFile);
      this.eventForm.get('image')?.markAsTouched(); 
    }
  }
  loadCities(){
    this.eventService.getCities().subscribe({
      next:(res:ApiResponse)=>{
        const cities = res.data.$values;
        
        this.cityOptions = cities.map((city:any) => ({
          id: city.id,
          label: `${city.cityName}, ${city.stateName}`
        }));
      }
    });
  }
  initForms(): void {
    this.eventForm = this.fb.group({
      title: ['', Validators.required],
      description: ['', Validators.required],
      eventDate: ['', [Validators.required,this.futureDateValidator]],
      eventType: ['', Validators.required],
      location: ['', Validators.required],
      category: [-111, Validators.required],
      image: [null, Validators.required]
    });

    this.ticketTypeForm = this.fb.group({
      id: [null],
      typeName: [null, Validators.required],
      price: [0, [Validators.required, Validators.min(1)]],
      totalQuantity: [0, [Validators.required, Validators.min(1)]],
      description: ['', Validators.required],
    });
  }
  futureDateValidator(control: AbstractControl): ValidationErrors | null {
    const selectedDate = new Date(control.value);
    const now = new Date();
    return selectedDate > now ? null : { pastDate: true };
  }
  startAddTicketType() {
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
      this.notify.info('Please complete the form and add at least one ticket type.');
      return;
    }
    const payload = {
      Title: this.eventForm.value.title,
      Description: this.eventForm.value.description,
      EventDate: new Date(this.eventForm.value.eventDate).toISOString(),
      EventType: this.eventForm.value.eventType === 'Seatable' ? 0 : 1,
      CityId: this.eventForm.value.location,
      Category: this.eventForm.value.category,
      TicketTypes: this.ticketTypes().map((t) => ({
        typeName: Number(t.typeName),
        price: Number(t.price),
        totalQuantity: Number(t.totalQuantity),
        description: t.description,
      })),
    };

    const formData = new FormData();

    formData.append('Title', payload.Title);
    formData.append('Description', payload.Description);
    formData.append('EventDate', payload.EventDate);
    formData.append('EventType', payload.EventType.toString());
    formData.append('CityId', payload.CityId);
    formData.append('Category', payload.Category.toString());

    payload.TicketTypes.forEach((ticket, index) => {
      formData.append(`TicketTypes[${index}].typeName`, ticket.typeName.toString());
      formData.append(`TicketTypes[${index}].price`, ticket.price.toString());
      formData.append(`TicketTypes[${index}].totalQuantity`, ticket.totalQuantity.toString());
      formData.append(`TicketTypes[${index}].description`, ticket.description);
    });

    if (this.selectedFile) {
      formData.append('Image', this.eventForm.get('image')?.value);
    }
    console.log(formData)
    this.eventService.addEvent(formData).subscribe({
      next: (res: any) => {
        this.notify.success('Event created successfully!');
        this.router.navigate([`/manager/events/${res.data.id}`]);
        // this.router.navigate([this.router.url, res.data.id]);
      },
      error: () => this.notify.error('Failed to create event'),
    });
  }
}
