import { TestBed } from '@angular/core/testing';
import { SignalRService } from './signalr-service';
import { NotificationService } from './notification-service';
import { Auth } from '../Auth/auth';

describe('SignalRService', () => {
  let service: SignalRService;
  let notificationSpy: jasmine.SpyObj<NotificationService>;

  beforeEach(() => {
    notificationSpy = jasmine.createSpyObj('NotificationService', ['info', 'success', 'error']);

    TestBed.configureTestingModule({
      providers: [
        SignalRService,
        { provide: NotificationService, useValue: notificationSpy },
        { provide: Auth, useValue: {} } // Auth is unused here
      ]
    });

    service = TestBed.inject(SignalRService);
  });

  it('should call notification.info for "info" type', () => {
    (service as any).handleNotification('info', 'Info message');
    expect(notificationSpy.info).toHaveBeenCalledWith('Info message');
  });

  it('should call notification.success for "success" type', () => {
    (service as any).handleNotification('success', 'Success message');
    expect(notificationSpy.success).toHaveBeenCalledWith('Success message');
  });

  it('should call notification.error for "error" type', () => {
    (service as any).handleNotification('error', 'Error message');
    expect(notificationSpy.error).toHaveBeenCalledWith('Error message');
  });

  it('should default to notification.info for unknown type', () => {
    (service as any).handleNotification('unknown', 'Default message');
    expect(notificationSpy.info).toHaveBeenCalledWith('Default message');
  });
});
