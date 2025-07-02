import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Home } from './home';
import { ActivatedRoute, Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { UserService } from '../../../services/User/user-service';
import { SignalRService } from '../../../services/Notification/signalr-service';
import { NotificationService } from '../../../services/Notification/notification-service';
import { User } from '../../../models/user.model';
import { ApiResponse } from '../../../models/api-response.model';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('Home Component', () => {
  let component: Home;
  let fixture: ComponentFixture<Home>;

  let routerSpy: jasmine.SpyObj<Router>;
  let userServiceSpy: jasmine.SpyObj<UserService>;
  let signalrServiceSpy: jasmine.SpyObj<SignalRService>;
  let notificationServiceSpy: jasmine.SpyObj<NotificationService>;

  beforeEach(async () => {
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    userServiceSpy = jasmine.createSpyObj('UserService', ['getUserDetails']);
    signalrServiceSpy = jasmine.createSpyObj('SignalRService', ['stopConnection']);
    notificationServiceSpy = jasmine.createSpyObj('NotificationService', ['error']);

    await TestBed.configureTestingModule({
      imports: [Home, HttpClientTestingModule], 
    })
    .overrideComponent(Home, {
      set: {
        providers: [
          { provide: Router, useValue: routerSpy },
          { provide: UserService, useValue: userServiceSpy },
          { provide: SignalRService, useValue: signalrServiceSpy },
          { provide: NotificationService, useValue: notificationServiceSpy },
          { provide: ActivatedRoute, useValue: {} } 
        ]
      }
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(Home);
    component = fixture.componentInstance;
  });

  it('should create the Home component', () => {
    userServiceSpy.getUserDetails.and.returnValue(of({ success: true, data: null,$id:"1",errors:null,message:"" }));
    fixture.detectChanges();
    expect(component).toBeTruthy();
  });

  // it('should fetch user details on init', () => {
  //   const mockUser: User = { username: 'John', email: 'john@example.com', role: 'User' };
  //   const mockResponse: ApiResponse = {
  //     $id: '1',
  //     success: true,
  //     message: 'Success',
  //     errors: null,
  //     data: mockUser
  //   };

  //   userServiceSpy.getUserDetails.and.returnValue(of(mockResponse));

  //   fixture.detectChanges(); 

  //   expect(userServiceSpy.getUserDetails).toHaveBeenCalled();
  //   expect(component.user()).toEqual(mockUser);
  // });

  it('should call notification service on user fetch error', () => {
    userServiceSpy.getUserDetails.and.returnValue(throwError(() => new Error('Error')));

    fixture.detectChanges();

    expect(notificationServiceSpy.error).toHaveBeenCalledWith('Failed to fetch your Data');
  });

  it('should log out and navigate to login', () => {
    localStorage.setItem('token', 'abc123');

    userServiceSpy.getUserDetails.and.returnValue(of({ success: true, data: null,$id:"1",errors:null,message:""}));

    fixture.detectChanges();

    component.logout();

    expect(localStorage.getItem('token')).toBeNull();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/login']);
    expect(signalrServiceSpy.stopConnection).toHaveBeenCalled();
  });
});
