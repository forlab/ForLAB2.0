import { Component, Inject, OnInit, Injector } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { takeUntil } from 'rxjs/operators';
import { BaseService } from 'src/@core/services/base.service';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { UsersController } from 'src/@core/APIs/UsersController';
class Params {
  objectInfo: QueryParamsDto[];
  userId: number;
  status: string;
}

@Component({
  selector: 'change-user-status',
  templateUrl: './change-user-status.component.html',
  styleUrls: ['./change-user-status.component.scss']
})
export class ChangeUserStatusComponent extends BaseService implements OnInit {


  constructor(@Inject(MAT_DIALOG_DATA) public data: Params,
    public dialogRef: MatDialogRef<ChangeUserStatusComponent>,
    public injector: Injector) {
    super(injector);
  }

  ngOnInit() {

  }


  async submit() {

    this.loading = true;

    const location = await this.httpService.GetLocationInfo();
    let body = {
      userId: this.data.userId,
      status: this.data.status,
      locationDto: location
    }

    this.httpService.PUT(UsersController.UpdateUserStatus, body)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res.isPassed) {
          this.loading = false;
          this.alertService.success(`User status is updated successfully`);
          this.dialogRef.close('updated');
        } else {
          this.alertService.error(res.message);
          this.loading = false;
          this._ref.detectChanges();
        }
      }, err => {
        this.alertService.exception();
        this.loading = false;
        this._ref.detectChanges();
      });
  }

}
