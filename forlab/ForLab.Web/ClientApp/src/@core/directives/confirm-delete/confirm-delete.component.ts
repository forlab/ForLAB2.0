import { Component, Inject, OnInit, Injector } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { takeUntil } from 'rxjs/operators';
import { BaseService } from '../../services/base.service';
import { QueryParamsDto } from '../../models/common/response';

class ConfirmDeleteParams {
  url: string;
  objectInfo: QueryParamsDto[];
  queryParamsDto: QueryParamsDto;
}

@Component({
  selector: 'confirm-delete',
  templateUrl: './confirm-delete.component.html',
  styleUrls: ['./confirm-delete.component.scss']
})
export class ConfirmDeleteComponent extends BaseService implements OnInit {


  constructor(@Inject(MAT_DIALOG_DATA) public data: ConfirmDeleteParams,
    public dialogRef: MatDialogRef<ConfirmDeleteComponent>,
    public injector: Injector) {
    super(injector);
  }

  ngOnInit() {

  }

  submit() {

    this.loading = true;
    var params: QueryParamsDto[] = [this.data.queryParamsDto];

    this.httpService.DELETE(this.data.url, params)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.alertService.success(`Object is deleted successfully`);
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
