// user-details.component.spec.ts
import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { UserDetails } from './user-details';
import { UserService } from '../../services/User/user-service';
import { NotificationService } from '../../services/Notification/notification-service';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { of, throwError } from 'rxjs';
import { ApiResponse } from '../../models/api-response.model';
import { User } from '../../models/user.model';

describe('UserDetails Component', () => {
  let component: UserDetails;
  let fixture: ComponentFixture<UserDetails>;
  let mockUserService: jasmine.SpyObj<UserService>;
  let mockNotificationService: jasmine.SpyObj<NotificationService>;
  let formBuilder: FormBuilder;

  const mockUser: User = {
    email: 'john@example.com',
    username: 'johnDoe',
    role: 'user'
  };

  beforeEach(async () => {
    mockUserService = jasmine.createSpyObj('UserService', [
      'getUserDetails',
      'updateUsername',
      'changePassword'
    ]);
    mockNotificationService = jasmine.createSpyObj('NotificationService', ['success']);

    await TestBed.configureTestingModule({
      // Correctly import standalone component
      imports: [UserDetails, ReactiveFormsModule, CommonModule],
      providers: [
        FormBuilder,
        { provide: UserService, useValue: mockUserService },
        { provide: NotificationService, useValue: mockNotificationService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(UserDetails);
    component = fixture.componentInstance;
    formBuilder = TestBed.inject(FormBuilder);
    
    // Initialize forms
    component.usernameForm = formBuilder.group({
      username: ['', Validators.required]
    });
    
    component.passwordForm = formBuilder.group({
      oldPassword: ['', Validators.required],
      newPassword: ['', [Validators.required, Validators.minLength(6)]]
    });
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize and load user details', fakeAsync(() => {
    const response: ApiResponse<User> = {
      $id: '1',
      success: true,
      message: '',
      data: mockUser,
      errors: null
    };
    mockUserService.getUserDetails.and.returnValue(of(response));

    component.ngOnInit();
    tick();

    expect(mockUserService.getUserDetails).toHaveBeenCalled();
    expect(component.user).toEqual(mockUser);
    expect(component.usernameForm.value.username).toEqual(mockUser.username);
    expect(component.originalName).toEqual(mockUser.username);
  }));

  it('should save a new username successfully', fakeAsync(() => {
    const updatedUser: User = { ...mockUser, username: 'newUser' };
    const response: ApiResponse<User> = {
      $id: '2',
      success: true,
      message: '',
      data: updatedUser,
      errors: null
    };

    component.user = mockUser;
    component.originalName = mockUser.username;
    component.usernameForm.setValue({ username: 'newUser' });
    component.isEditingUsername = true;

    mockUserService.updateUsername.and.returnValue(of(response));
    mockUserService.getUserDetails.and.returnValue(of({
      $id: '3',
      success: true,
      message: '',
      data: updatedUser,
      errors: null
    }));

    component.saveUsername();
    tick();

    expect(mockUserService.updateUsername).toHaveBeenCalledWith({ username: 'newUser' });
    expect(mockNotificationService.success).toHaveBeenCalledWith('Username Changed');
    expect(component.isEditingUsername).toBeFalse();
  }));

  it('should not save username if unchanged', () => {
    component.user = mockUser;
    component.originalName = mockUser.username;
    component.usernameForm.setValue({ username: mockUser.username });
    component.isEditingUsername = true;

    component.saveUsername();

    expect(mockUserService.updateUsername).not.toHaveBeenCalled();
    expect(component.isEditingUsername).toBeFalse();
  });

  it('should cancel username editing', () => {
    component.originalName = 'originalUser';
    component.usernameForm.setValue({ username: 'changedName' });
    component.isEditingUsername = true;

    component.CancelEdit();

    expect(component.usernameForm.value.username).toEqual('originalUser');
    expect(component.isEditingUsername).toBeFalse();
  });

    it('should change password successfully', fakeAsync(() => {
    component.passwordForm.setValue({
      oldPassword: 'oldpass',
      newPassword: 'newpass123'
    });
    component.isChangingPassword = true;

    const response: ApiResponse = {
      $id: '4',
      success: true,
      message: 'Password changed',
      data: null,
      errors: null
    };

    mockUserService.changePassword.and.returnValue(of(response));

    component.changePassword();
    tick();

    expect(mockUserService.changePassword).toHaveBeenCalledWith({
      oldPassword: 'oldpass',
      newPassword: 'newpass123'
    });
    expect(mockNotificationService.success).toHaveBeenCalledWith('Password changed successfully!');
    expect(component.isChangingPassword).toBeFalse();
    expect(component.passwordForm.value).toEqual({ oldPassword: null, newPassword: null });
  }));

  it('should alert when old and new passwords match', () => {
    component.passwordForm.setValue({
      oldPassword: 'samepass',
      newPassword: 'samepass'
    });

    component.changePassword();

    expect(mockNotificationService.error).toHaveBeenCalledWith('The old and new passwords are same. Try stronger passwords!');
    expect(mockUserService.changePassword).not.toHaveBeenCalled();
  });

  it('should handle password change error', fakeAsync(() => {
    component.passwordForm.setValue({
      oldPassword: 'oldpass',
      newPassword: 'newpass123'
    });

    mockUserService.changePassword.and.returnValue(throwError(() => new Error('Server error')));
    
    component.changePassword();
    tick();

    expect(mockNotificationService.error).toHaveBeenCalledWith('Failed to change password.');
  }));

  it('should handle username update error', fakeAsync(() => {
    component.user = mockUser;
    component.usernameForm.setValue({ username: 'newName' });
    component.isEditingUsername = true;

    mockUserService.updateUsername.and.returnValue(throwError(() => new Error('update error')));
    
    component.saveUsername();
    tick();

    expect(mockNotificationService.error).toHaveBeenCalledWith('Failed to update username.');
  }));


  it('should cancel password change', () => {
    component.passwordForm.setValue({
      oldPassword: 'oldpass',
      newPassword: 'newpass'
    });
    component.isChangingPassword = true;

    component.cancelPasswordChange();

    expect(component.isChangingPassword).toBeFalse();
    expect(component.passwordForm.value).toEqual({ oldPassword: null, newPassword: null });
  });

});