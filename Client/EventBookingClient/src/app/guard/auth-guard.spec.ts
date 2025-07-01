import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { AuthGuard, UserGuard, ManagerGuard, AdminGuard } from './auth-guard';
import { Auth } from '../services/Auth/auth';
import { SignalRService } from '../services/Notification/signalr-service';

class MockAuthService {
  getToken(): string | null {
    return null;
  }
}

class MockSignalRService {
  startConnection() {}
}

class MockRouter {
  navigate(path: any[]) {}
}

describe('Auth Guards', () => {
  let router: Router;
  
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        { provide: Router, useClass: MockRouter },
        { provide: Auth, useClass: MockAuthService },
        { provide: SignalRService, useClass: MockSignalRService }
      ]
    });
    
    router = TestBed.inject(Router);
    spyOn(router, 'navigate');
  });

  describe('AuthGuard', () => {
    let authGuard: AuthGuard;
    let authService: Auth;
    let signalRService: SignalRService;

    beforeEach(() => {
      authService = TestBed.inject(Auth);
      signalRService = TestBed.inject(SignalRService);
      authGuard = TestBed.inject(AuthGuard);
      
      spyOn(signalRService, 'startConnection');
    });

    it('should allow activation when token exists', () => {
      spyOn(authService, 'getToken').and.returnValue('valid-token');
      expect(authGuard.canActivate()).toBeTrue();
      expect(signalRService.startConnection).toHaveBeenCalled();
    });

    it('should block activation and redirect when token is missing', () => {
      spyOn(authService, 'getToken').and.returnValue(null);
      expect(authGuard.canActivate()).toBeFalse();
      expect(router.navigate).toHaveBeenCalledWith(['/default']);
    });
  });
});