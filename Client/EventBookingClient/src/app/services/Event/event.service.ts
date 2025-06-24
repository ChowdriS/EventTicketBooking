import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { API_BASE_URL } from '../../misc/constants';
import { ApiResponse, PagedResponse } from '../../models/api-response.model';
import { Observable } from 'rxjs';
import { AppEvent } from '../../models/event.model';

@Injectable({ providedIn: 'root' })
export class EventService {
  constructor(private http: HttpClient) {}

  getEvents(pageNumber = 1, pageSize = 10): Observable<ApiResponse<PagedResponse<any>>> {
    return this.http.get<ApiResponse<PagedResponse<any>>>(`${API_BASE_URL}/events/?pageNumber=${pageNumber}&pageSize=${pageSize}`);
  }
  getEventById(eventId:string) : Observable<ApiResponse<AppEvent>>{
    return this.http.get<ApiResponse<AppEvent>>(`${API_BASE_URL}/events/${eventId}`);
  }
  getManagerEvents(pageNumber = 1, pageSize = 10): Observable<ApiResponse<PagedResponse<any>>> {
    return this.http.get<ApiResponse<PagedResponse<any>>>(`${API_BASE_URL}/events/myevents?pageNumber=${pageNumber}&pageSize=${pageSize}`);
  }
  updateEvent(eventId: string, payload: any): Observable<ApiResponse<AppEvent>>{
    return this.http.put<ApiResponse<AppEvent>>(`${API_BASE_URL}/events/${eventId}`,payload);
  }
  addEvent(payload: any): Observable<ApiResponse<AppEvent>>{
    return this.http.post<ApiResponse<AppEvent>>(`${API_BASE_URL}/events`,payload);
  }
  getFilteredEvents(searchElement: string, filterDate: string, pageNumber: number, pageSize: number) {
    let url = `${API_BASE_URL}/events/filter?`;
    if (searchElement && searchElement.trim() !== "") {
      url += `searchElement=${searchElement}`;
    }
    else{
      url += 'searchElement='
    }
    if (filterDate && filterDate.trim() !== "") {
      url += `&date=${filterDate}`;
    }
    else{
      url += "&date=";
    }
    return this.http.get<ApiResponse<PagedResponse<any>>>(`${url}&pageNumber=${pageNumber}&pageSize=${pageSize}`);
    // return this.http.get<ApiResponse<PagedResponse<any>>>(`${API_BASE_URL}/events/filter?searchElement=&date=&pageNumber=1&pageSize=10`);
  }

}
