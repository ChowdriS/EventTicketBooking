import { Injectable } from '@angular/core';
import { API_BASE_URL } from '../../misc/constants';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse, PagedResponse } from '../../models/api-response.model';
import { AppEvent } from '../../models/event.model';

@Injectable({
  providedIn: 'root'
})
export class TicketTypeService {

  constructor(private http: HttpClient) {}

  getByEventId(eventId: string): Observable<any[]> {
    return this.http.get<any[]>(`${API_BASE_URL}/tickettype/event/${eventId}`);
  }

  addTicketType(data: any): Observable<ApiResponse> {
    return this.http.post<ApiResponse>(`${API_BASE_URL}/tickettype`, data);
  }

  updateTicketType(id: string, data: any): Observable<ApiResponse> {
    return this.http.put<ApiResponse>(`${API_BASE_URL}/tickettype/${id}`, data);
  }

  deleteTicketType(id: string): Observable<ApiResponse> {
    return this.http.delete<ApiResponse>(`${API_BASE_URL}/tickettype/${id}`);
  }
}
