import { CommonModule } from '@angular/common';
import { Component, Inject } from '@angular/core';
import { MAT_SNACK_BAR_DATA, MatSnackBarRef } from '@angular/material/snack-bar';

@Component({
  selector: 'app-custom-snackbar',
  standalone: true,
  imports:[CommonModule],
  templateUrl: './custom-snackbar.html'
})
export class CustomSnackbarComponent {
  constructor(
    private snackBarRef: MatSnackBarRef<CustomSnackbarComponent>,
    @Inject(MAT_SNACK_BAR_DATA) public data: { message: string; type: 'info' | 'success' | 'error' }
  ) {}

  close() {
    this.snackBarRef.dismiss();
  }
}
