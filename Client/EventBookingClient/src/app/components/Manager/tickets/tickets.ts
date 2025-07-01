import { Component } from '@angular/core';
import { TicketService } from '../../../services/Ticket/ticket.service';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { NotificationService } from '../../../services/Notification/notification-service';

@Component({
  selector: 'app-tickets',
  imports: [CommonModule,],
  templateUrl: './tickets.html',
  styleUrl: './tickets.css'
})
export class Tickets {
  eventId!:string;
  tickets: any[] = [];
  currentPage = 1;
  pageSize = 5;
  totalPages = 1;

  constructor(private ticketService: TicketService,private route:ActivatedRoute,  private notify: NotificationService) { }

  ngOnInit(): void {
    this.eventId = this.route.snapshot.paramMap.get('id')!;
    this.loadTickets();
    console.log(this.eventId);
  }
  loadTickets(){
    this.ticketService.getTicketsByEventId(this.eventId,this.currentPage, this.pageSize).subscribe({
      next: (res) => {
        this.tickets = res.data.items.$values;
        console.log(this.tickets);
        this.totalPages = res.data.totalPages;
        this.currentPage = res.data.pageNumber;
      },
      error: () => this.notify.error('Failed to load tickets')
    });
  }
  changePage(page: number) {
    if (page < 1 || page > this.totalPages) return;
    this.currentPage = page;
    this.loadTickets();
  }
}
