import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CustomSnackbarComponent } from '../../components/custom-snackbar/custom-snackbar';

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  constructor(private snackBar: MatSnackBar) {}

  info(message: string) {
    this.show(message, 'info');
  }

  success(message: string) {
    this.show(message, 'success');
  }

  error(message: string) {
    this.show(message, 'error');
  }

  private show(message: string, type: 'info' | 'success' | 'error') {
    this.snackBar.openFromComponent(CustomSnackbarComponent, {
      data: { message, type },
      horizontalPosition: 'end',
      verticalPosition: 'top',
      panelClass: [],
      duration: 3000,
    });
  }
}
