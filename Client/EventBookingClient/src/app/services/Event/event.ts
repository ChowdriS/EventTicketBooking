import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { API_BASE_URL } from '../../shared/constants';
import { ApiResponse } from '../../models/api-response.model';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class EventService {

  constructor(private http: HttpClient) {}

  getEvents(pageNumber = 1, pageSize = 10) : Observable<ApiResponse> {
    return this.http.get<ApiResponse>(`${API_BASE_URL}/events/?pageNumber=${pageNumber}&pageSize=${pageSize}`);
  }
}
