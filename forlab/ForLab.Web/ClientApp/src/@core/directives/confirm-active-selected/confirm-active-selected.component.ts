import { Component, Inject, OnInit, Injector } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { takeUntil } from 'rxjs/operators';
import { BaseService } from '../../services/base.service';
import { QueryParamsDto } from '../../models/common/response';

class ConfirmActiveParams {
  url: string;
  ids: number[];
  queryParamsDto: QueryParamsDto;
  // IsActive, Shared
  isActive: boolean;
  shared: boolean;
  type: 'isActive' | 'shared' = 'isActive';
}

@Component({
  selector: 'confirm-active-selected',
  templateUrl: './confirm-active-selected.component.html',
  styleUrls: ['./confirm-active-selected.component.scss']
})
export class ConfirmActiveSelectedComponent extends BaseService implements OnInit {

  constructor(@Inject(MAT_DIALOG_DATA) public data: ConfirmActiveParams,
    public dialogRef: MatDialogRef<ConfirmActiveSelectedComponent>,
    public injector: Injector) {
    super(injector);
  }

  ngOnInit() {
    if(!this.data.type) {
      this.data.type = 'isActive';
    }
  }

  submit() {

    this.loading = true;
    var params: QueryParamsDto[] = [{ 
      key: this.data.type, 
      value: this.data.type == 'isActive' ? this.data.isActive : this.data.shared
    }];

    this.httpService.PUT(this.data.url, this.data.ids, params)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            if (this.data?.type == 'isActive') {
              this.alertService.success(`Objects are ${this.data.isActive ? 'activated' : 'deactivated'} successfully`);
            } else if (this.data?.type == 'shared') {
              this.alertService.success(`Objects are ${this.data.shared ? 'shared' : 'stopped'} successfully`);
            }
            this.dialogRef.close('done');
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
