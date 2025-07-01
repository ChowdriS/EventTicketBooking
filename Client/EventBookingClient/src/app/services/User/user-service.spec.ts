import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { UserService } from './user-service';
import { API_BASE_URL } from '../../misc/constants';
import { ApiResponse } from '../../models/api-response.model';

describe('UserService', () => {
  let service: UserService;
  let httpMock: HttpTestingController;

  const mockResponse: ApiResponse = {
    $id: '1',
    success: true,
    message: 'OK',
    data: {},
    errors: null
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [UserService]
    });

    service = TestBed.inject(UserService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should get user details', () => {
    service.getUserDetails().subscribe(res => {
      expect(res.success).toBeTrue();
    });

    const req = httpMock.expectOne(`${API_BASE_URL}/users/me`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should update username', () => {
    const payload = { username: 'newUser' };

    service.updateUsername(payload).subscribe(res => {
      expect(res.success).toBeTrue();
    });

    const req = httpMock.expectOne(`${API_BASE_URL}/users/update`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(payload);
    req.flush(mockResponse);
  });

  it('should change password', () => {
    const payload = { oldPassword: 'old123', newPassword: 'new123' };

    service.changePassword(payload).subscribe(res => {
      expect(res.success).toBeTrue();
    });

    const req = httpMock.expectOne(`${API_BASE_URL}/users/changepassword`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(payload);
    req.flush(mockResponse);
  });

  it('should get all users', () => {
    service.getAllUsers().subscribe(res => {
      expect(res.success).toBeTrue();
    });

    const req = httpMock.expectOne(`${API_BASE_URL}/users/all`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should delete a user by ID', () => {
    const userId = '123abc';

    service.deleteUser(userId).subscribe(res => {
      expect(res.success).toBeTrue();
    });

    const req = httpMock.expectOne(`${API_BASE_URL}/users/delete/${userId}`);
    expect(req.request.method).toBe('DELETE');
    req.flush(mockResponse);
  });
});
