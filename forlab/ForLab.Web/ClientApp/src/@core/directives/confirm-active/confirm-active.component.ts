import { Component, Inject, OnInit, Injector } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { takeUntil } from 'rxjs/operators';
import { BaseService } from '../../services/base.service';
import { QueryParamsDto } from '../../models/common/response';

class ConfirmActiveParams {
  url: string;
  isActive: boolean;
  objectInfo: QueryParamsDto[];
  queryParamsDto: QueryParamsDto;
  btnTitle: string;
}

@Component({
  selector: 'confirm-active',
  templateUrl: './confirm-active.component.html',
  styleUrls: ['./confirm-active.component.scss']
})
export class ConfirmActiveComponent extends BaseService implements OnInit {


  constructor(@Inject(MAT_DIALOG_DATA) public data: ConfirmActiveParams,
    public dialogRef: MatDialogRef<ConfirmActiveComponent>,
    public injector: Injector) {
    super(injector);
  }

  ngOnInit() {

  }

  submit() {

    this.loading = true;
    var params: QueryParamsDto[] = [this.data.queryParamsDto];
    params.push({ key: 'isActive', value: this.data.isActive });

    this.httpService.PUT(this.data.url, null, params)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.alertService.success(`Object is ${this.data.isActive ? 'activated' : 'deactivated'} successfully`);
            this.dialogRef.close('deleted');
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
