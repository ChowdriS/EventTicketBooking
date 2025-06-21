import { Component, signal } from '@angular/core';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { EventService } from '../../../services/Event/event';
import { ApiResponse } from '../../../models/api-response.model';
import { AppEvent } from '../../../models/event.model';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-home',
  imports: [RouterOutlet],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class Home {
  constructor(public router : Router) {}

  logout() {
    localStorage.removeItem('token');
    this.router.navigate(['/login']);
  }
}
