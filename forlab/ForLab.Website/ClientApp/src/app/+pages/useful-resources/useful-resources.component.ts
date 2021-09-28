import { Component, OnInit, Injector } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { UsefulResourceDto } from 'src/@core/models/CMS/UsefulResource';
import { UsefulResourcesController } from 'src/@core/APIs/UsefulResourcesController';
import { Observable } from 'rxjs';
import { map, takeUntil } from 'rxjs/operators';
import { QueryParamsDto } from 'src/@core/models/common/response';
import * as FileSaver from 'file-saver';

@Component({
  selector: 'app-useful-resources',
  templateUrl: './useful-resources.component.html',
  styleUrls: ['./useful-resources.component.scss']
})
export class UsefulResourcesComponent extends BaseService implements OnInit {

  usefulResources$: Observable<UsefulResourceDto[]>;

  constructor(public injector: Injector) {
    super(injector);
  }

  ngOnInit(): void {
    this.loadUsefulResources();
  }

  loadUsefulResources() {
    let params: QueryParamsDto[] = [
      { key: 'isActive', value: true },
    ];
    this.usefulResources$ = this.httpService.GET(UsefulResourcesController.GetAll, params).pipe(map(res => res.data.list));
  }

  DownloadResourceFile(usefulResourceDto: UsefulResourceDto): void {
    var name = usefulResourceDto.attachmentUrl.substr(usefulResourceDto.attachmentUrl.lastIndexOf('\\') + 1);
    FileSaver.saveAs(usefulResourceDto.attachmentUrl, name);
    this.incrementDownloadCount(usefulResourceDto.id);
  }

  incrementDownloadCount(usefulResourceId: number) {
    let params: QueryParamsDto[] = [
      { key: 'usefulResourceId', value: usefulResourceId },
    ];

    this.httpService.POST(UsefulResourcesController.IncrementDownloadCount, null, params)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res.isPassed) {
          this.loadUsefulResources();
        } else {
          // this.alertService.error(res.message);
          this.loading = false;
          this._ref.detectChanges();
        }
      });
  }

}
