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
  deleteEvent(id:string): Observable<ApiResponse<AppEvent>>{
    return this.http.delete<ApiResponse<AppEvent>>(`${API_BASE_URL}/events/${id}`);
  }
  getAllEventImages(): Observable<any>{
    return this.http.get<any>(`${API_BASE_URL}/eventimage/getall`);
  }
  getMyEventImages(): Observable<any>{
    return this.http.get<any>(`${API_BASE_URL}/eventimage/myevent/getall`);
  }
  deleteEventImages(image:any): Observable<any>{
    return this.http.delete<any>(`${API_BASE_URL}/eventimage/delete/${image}`);
  }
  getFilteredEvents(category: number,location : string,searchElement: string, filterDate: string, pageNumber: number, pageSize: number) {
    let url = `${API_BASE_URL}/events/filter?`;
    if(category != -111){
      url +=  `category=${category}`
    }
    else{
      url +=  `category=`
    }
    if(location && location.trim() !== ""){
      url +=  `&cityId=${location}`
    }
    else{
      url +=  `&cityId=`
    }
    if (searchElement && searchElement.trim() !== "") {
      url += `&searchElement=${searchElement}`;
    }
    else{
      url += '&searchElement='
    }
    if (filterDate && filterDate.trim() !== "") {
      url += `&date=${filterDate}`;
    }
    else{
      url += "&date=";
    }
    return this.http.get<ApiResponse<PagedResponse<any>>>(`${url}&pageNumber=${pageNumber}&pageSize=${pageSize}`);
  }
  uploadEventImage(id:string,payload : any){
    return this.http.post<any>(`${API_BASE_URL}/eventimage/upload/${id}`,payload);
  }
  getCities(){
    return this.http.get<any>(`${API_BASE_URL}/events/cities`);
  }
}
