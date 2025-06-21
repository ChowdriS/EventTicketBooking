import { Component, signal } from '@angular/core';
import { EventService } from '../../services/Event/event';
import { AppEvent } from '../../models/event.model';
import { Router, RouterLink } from '@angular/router';
import { ApiResponse } from '../../models/api-response.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-front-page',
  imports: [RouterLink,CommonModule],
  templateUrl: './front-page.html',
  styleUrl: './front-page.css'
})
export class FrontPage {
  topEvent = signal<AppEvent | null>(null);
  constructor(private eventsService: EventService,public router : Router) {}

  ngOnInit() {
    this.fetchTopEvent();
  }

  fetchTopEvent() {
    this.eventsService.getEvents(1, 1).subscribe({
      next: (res:ApiResponse)=>{
        console.log(res.data)
      this.topEvent.set(res.data?.items.$values[0] ?? null);
      },
      error: (error:any) => {
          const errorMessage = error?.error?.errors?.message || 'An unknown error occurred.';      
          alert(errorMessage);
        }
    });
  }
}
