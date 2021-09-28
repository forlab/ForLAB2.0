import { Component, Inject, OnInit, Injector } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { takeUntil } from 'rxjs/operators';
import { BaseService } from '../../services/base.service';
import { QueryParamsDto } from '../../models/common/response';
import { saveAs } from 'file-saver';

class ConfirmExportParams {
  url: string;
  fileName: string = 'ExportedData';
  queryParamsDtos?: QueryParamsDto[] = [];
}

@Component({
  selector: 'confirm-export',
  templateUrl: './confirm-export.component.html',
  styleUrls: ['./confirm-export.component.scss']
})
export class ConfirmExportComponent extends BaseService implements OnInit {

  applyPagination: boolean = false;

  constructor(@Inject(MAT_DIALOG_DATA) public data: ConfirmExportParams,
    public dialogRef: MatDialogRef<ConfirmExportComponent>,
    public injector: Injector) {
    super(injector);
  }

  ngOnInit() {

  }

  submit() {

    this.loading = true;
    var params: QueryParamsDto[] = this.data.queryParamsDtos || [];

    if(!this.applyPagination) {
      const pageIndex = params.findIndex(x => x.key == 'pageIndex');
      const pageSize = params.findIndex(x => x.key == 'pageSize');
      if(pageIndex > -1) params.splice(pageIndex, 1);
      if(pageSize > -1) params.splice(pageSize, 1);
    }

    this.httpService.ExportToExcel(this.data.url, params)
      .pipe(takeUntil(this.ngUnsubscribe))
      .toPromise()
      .then(data => {
        this.dialogRef.close();
        this.loading = false;
        saveAs(data.body, `${this.data.fileName}.xlsx`);
      });
  }

}
