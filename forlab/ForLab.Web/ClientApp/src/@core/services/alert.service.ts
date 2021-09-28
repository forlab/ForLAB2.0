import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root'
})
export class AlertService {


  constructor(private snackBar: MatSnackBar) {
  }

  snackBadddr(message: string, verticalPosition, horizontalPosition, colorName: string, duration: number = 4000) {
    this.snackBar.open(message, '', {
      duration: duration,
      verticalPosition: verticalPosition,
      horizontalPosition: horizontalPosition,
      panelClass: colorName,
    });
  }

  success(message: string) {
    this.snackBadddr(message, 'bottom', 'center', 'snackbar-success');
  }

  error(message: string) {
    this.snackBadddr(message, 'bottom', 'center', 'snackbar-danger');
  }

  exception(message: string = null) {
    const defaultMsg = 'There is an unexpected error, please contact your administrator.';
    this.snackBadddr(message ? message : defaultMsg, 'bottom', 'center', 'snackbar-danger');
  }

  warning(message: string) {
    this.snackBadddr(message, 'bottom', 'center', 'snackbar-warning');
  }

  message(message: string) {
    this.snackBadddr(message, 'bottom', 'center', 'snackbar-black');
  }


}
