import { TestBed } from '@angular/core/testing';
import { NotificationService } from './notification-service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CustomSnackbarComponent } from '../../components/custom-snackbar/custom-snackbar';

describe('NotificationService', () => {
  let service: NotificationService;
  let snackBarSpy: jasmine.SpyObj<MatSnackBar>;

  beforeEach(() => {
    const spy = jasmine.createSpyObj('MatSnackBar', ['openFromComponent']);

    TestBed.configureTestingModule({
      providers: [
        NotificationService,
        { provide: MatSnackBar, useValue: spy }
      ]
    });

    service = TestBed.inject(NotificationService);
    snackBarSpy = TestBed.inject(MatSnackBar) as jasmine.SpyObj<MatSnackBar>;
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should show info message', () => {
    const message = 'Info Message';
    service.info(message);
    expect(snackBarSpy.openFromComponent).toHaveBeenCalledWith(CustomSnackbarComponent, jasmine.objectContaining({
      data: { message, type: 'info' }
    }));
  });

  it('should show success message', () => {
    const message = 'Success Message';
    service.success(message);
    expect(snackBarSpy.openFromComponent).toHaveBeenCalledWith(CustomSnackbarComponent, jasmine.objectContaining({
      data: { message, type: 'success' }
    }));
  });

  it('should show error message', () => {
    const message = 'Error Message';
    service.error(message);
    expect(snackBarSpy.openFromComponent).toHaveBeenCalledWith(CustomSnackbarComponent, jasmine.objectContaining({
      data: { message, type: 'error' }
    }));
  });
});
