import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { API_BASE_URL } from '../../misc/constants';

@Injectable({
  providedIn: 'root'
})
export class AnalyticsService {

  constructor(private http: HttpClient) { }
  getMyEarning(){
    return this.http.get<any>(`${API_BASE_URL}/analytics/my-earnings`);
  }
}
