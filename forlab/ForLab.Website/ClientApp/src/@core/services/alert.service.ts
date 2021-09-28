import { Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';

@Injectable()
export class AlertService {


  constructor(private toastr: ToastrService) {
  }

  success(message: string) {
    this.toastr.success(message);
  }

  error(message: string) {
    this.toastr.error(message);
  }

  exception(message: string = null) {
    const defaultMsg = 'There is an unexpected error, please contact your administrator.';
    this.toastr.error(message ? message : defaultMsg);
  }

  warning(message: string) {
    this.toastr.warning(message);
  }

  message(message: string) {
    this.toastr.info(message);
  }


}
