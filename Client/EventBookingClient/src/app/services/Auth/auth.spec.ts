import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { Auth } from './auth';
import { API_BASE_URL } from '../../misc/constants';
import { LoginRequest, RegisterRequest } from '../../models/auth.model';

describe('Auth Service', () => {
  let service: Auth;
  let httpMock: HttpTestingController;

  const mockLoginPayload: LoginRequest = {
    email: 'test@example.com',
    password: 'password123'
  };

  const mockRegisterPayload: RegisterRequest = {
    email: 'test@example.com',
    password: 'password123',
    role: 'manager', 
    username: 'testuser',
    confirmPassword: 'password123',
  }
  const mockUserRegisterPayload: RegisterRequest = {
    email: 'test@example.com',
    password: 'password123',
    role: 'user', 
    username: 'testuser',
    confirmPassword: 'password123',
  }
  const dummyApiResponse = {
    "$id": "1",
    "success": true,
    "message": "Manager succesfully added ",
    "data": {
        "$id": "2",
        "email": "test@example.com",
        "username": "testuser",
        "role": "Manager"
    },
    "errors": null
  } 
  const dummyUserApiResponse = {
    "$id": "1",
    "success": true,
    "message": "User succesfully added ",
    "data": {
        "$id": "2",
        "email": "test@example.com",
        "username": "testuser",
        "role": "User"
    },
    "errors": null
  } 

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [Auth]
    });
    service = TestBed.inject(Auth);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
    localStorage.clear();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should login a user', () => {
    service.login(mockLoginPayload).subscribe(res => {
      expect(res).toEqual(dummyApiResponse);
    });

    const req = httpMock.expectOne(`${API_BASE_URL}/auth/login`);
    expect(req.request.method).toBe('POST');
    req.flush(dummyApiResponse);
  });

  it('should register a user', () => {
    service.register(mockUserRegisterPayload).subscribe(res => {
      expect(res).toEqual(dummyUserApiResponse);
    });

    const req = httpMock.expectOne(`${API_BASE_URL}/users/register`);
    expect(req.request.method).toBe('POST');
    req.flush(dummyUserApiResponse);
  });

  it('should register a manager', () => {
    service.register(mockRegisterPayload).subscribe(res => {
      expect(res).toEqual(dummyApiResponse);
    });

    const req = httpMock.expectOne(`${API_BASE_URL}/users/manager`);
    expect(req.request.method).toBe('POST');
    req.flush(dummyApiResponse);
  });

  it('should add an admin', () => {
    const adminPayload = { name: 'Admin' };

    service.addAdmin(adminPayload).subscribe(res => {
      expect(res).toEqual(dummyApiResponse);
    });

    const req = httpMock.expectOne(`${API_BASE_URL}/users/admin`);
    expect(req.request.method).toBe('POST');
    req.flush(dummyApiResponse);
  });

  it('should set, get and remove token from localStorage', () => {
    const token = 'sample-token';
    service.setToken(token);

    expect(localStorage.getItem('token')).toBe(token);
    expect(service.getToken()).toBe(token);

    service.logout();
    expect(localStorage.getItem('token')).toBeNull();
  });
});
