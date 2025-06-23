import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiResponse } from '../../models/api-response.model';
import { catchError, map, Observable, throwError } from 'rxjs';
import { API_BASE_URL } from '../../misc/constants';

@Injectable({
  providedIn: 'root'
})
export class TicketService {
  constructor(private http: HttpClient) { }

  bookTicket(payload: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${API_BASE_URL}/tickets/book`, payload);
  }
  getMyTickets(page: number, size: number): Observable<ApiResponse> {
    return this.http.get<ApiResponse>(`${API_BASE_URL}/tickets/mine?pageNumber=${page}&pageSize=${size}`);
  }
  
  getTicketsByEventId(eventId:string, pageNumber:Number, pageSize:Number){
    return this.http.get<ApiResponse>(`${API_BASE_URL}/tickets/event/${eventId}?pageNumber=${pageNumber}&pageSize=${pageSize}`);
  }

  cancelTicket(ticketId: string): Observable<ApiResponse> {
    return this.http.delete<ApiResponse>(`${API_BASE_URL}/tickets/${ticketId}/cancel`);
  }
  exportTicket(ticketId: string): Observable<Blob> {
    return this.http.get(`${API_BASE_URL}/tickets/${ticketId}/export`, {
      responseType: 'blob'
    }).pipe(
      catchError((error: HttpErrorResponse) => {
        return throwError(() => new Error(
          error.error?.message || 'Ticket export failed'
        ));
      })
    );
  }
}
