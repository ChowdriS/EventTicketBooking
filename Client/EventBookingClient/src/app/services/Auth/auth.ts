import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { API_BASE_URL } from '../../misc/constants';
import { Observable } from 'rxjs';
import { LoginRequest, RegisterRequest } from '../../models/auth.model';
import { ApiResponse } from '../../models/api-response.model';

@Injectable({ providedIn: 'root' })
export class Auth {
  constructor(private http: HttpClient) {}

  login(payload: LoginRequest): Observable<ApiResponse> {
    return this.http.post<ApiResponse>(`${API_BASE_URL}/auth/login`, payload);
  }

  register(payload: RegisterRequest): Observable<ApiResponse> {
    const url = payload.role === 'manager' ? '/users/manager' : '/users/register';
    return this.http.post<ApiResponse>(`${API_BASE_URL}${url}`, payload);
  }

  setToken(token: string) {
    localStorage.setItem('token', token);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  logout(): void {
    localStorage.removeItem('token');
  }
  
  addAdmin(payload: any) {
    return this.http.post<ApiResponse>(`${API_BASE_URL}/users/admin`, payload);
  }
}