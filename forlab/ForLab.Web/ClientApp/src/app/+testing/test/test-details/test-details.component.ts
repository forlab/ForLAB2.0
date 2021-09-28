import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { TestDto } from 'src/@core/models/testing/Test';
import { TestsController } from 'src/@core/APIs/TestsController';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { takeUntil } from 'rxjs/operators';
import { ProductUsagesComponent } from 'src/app/+product/product-usage/product-usages/product-usages.component';

@Component({
  selector: 'app-test-details',
  templateUrl: './test-details.component.html',
  styleUrls: ['./test-details.component.scss']
})
export class TestDetailsComponent extends BaseService implements OnInit {

  // Children
  @ViewChild(ProductUsagesComponent, { static: false }) productUsagesComponent: ProductUsagesComponent;

  testId: number;
  testDto: TestDto = new TestDto();
  loadingTest = false;

  constructor(public injector: Injector) {
    super(injector);

    if (this.router.url.includes('details')) {
      this.activatedRoute.paramMap.subscribe(params => {
        this.testId = Number(params.get('testId'));
        this.loadDataById(this.testId);
      });
    }

  }

  ngOnInit(): void {
  }

  loadDataById(id: number) {
    this.loadingTest = true;
    const url = TestsController.GetTestDetails;
    let params: QueryParamsDto[] = [
      { key: 'testId', value: id },
    ];

    this.httpService.GET(url, params)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res.isPassed) {
          this.testDto = res.data;
          this.loadingTest = false;

          if (this.testDto.createdBy != this.loggedInUser?.id) {
            if(this.productUsagesComponent) {
              this.productUsagesComponent.columns = this.productUsagesComponent?.columns.filter(x => x.property != 'select' && x.property != 'actions');
            }
          }

        } else {
          this.alertService.error(res.message);
          this.loading = false;
          this._ref.detectChanges();
        }
      });
  }

}
