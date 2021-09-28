import { Component, OnInit, Injector, Input, OnChanges } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { ProductAssumptionParametersController } from 'src/@core/APIs/ProductAssumptionParametersController';
import { takeUntil } from 'rxjs/operators';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { GroupProductAssumptionParameterDto } from 'src/@core/models/program/ProductAssumptionParameter';
import { ProgramDto } from 'src/@core/models/program/Program';
import { ForecastProductAssumptionValueDto } from 'src/@core/models/forecasting/ForecastProductAssumptionValue';

@Component({
  selector: 'forecast-product-assumptions',
  templateUrl: './forecast-product-assumptions.component.html',
  styleUrls: ['./forecast-product-assumptions.component.scss']
})
export class ForecastProductAssumptionsComponent extends BaseService implements OnInit, OnChanges {
  // Inputs
  @Input('programDtos') programDtos: ProgramDto[] = [];
  @Input('data') forecastProductAssumptionValueDtos: ForecastProductAssumptionValueDto[] = [];
  // Vars
  groupProductAssumptionParameterDto: GroupProductAssumptionParameterDto[] = [];
  // Accordion
  panelOpenState = false;
  step = 0;
  setStep(index: number) {
    this.step = index;
  }
  nextStep() {
    this.step++;
  }
  prevStep() {
    this.step--;
  }

  constructor(public injector: Injector) {
    super(injector);
  }

  ngOnInit(): void {
  }

  ngOnChanges() {
    this.loadProductAssumptions();
  }

  loadProductAssumptions() {
    const programIds = this.programDtos.map(x => x.id);
    if (programIds.length == 0) {
      return;
    }

    let params: QueryParamsDto[] = [
      { key: 'programIds', value: programIds.join(',') },
    ];

    this.httpService.GET(ProductAssumptionParametersController.GetAllProductAssumptionsForForcast, params)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.groupProductAssumptionParameterDto = res.data;
            // Set values from the forecast
            if (this.forecastProductAssumptionValueDtos?.length > 0) {
              this.groupProductAssumptionParameterDto?.forEach(item => {
                item.productAssumptionParameterDtos.forEach(x => {
                  x.value = this.forecastProductAssumptionValueDtos.find(y => y.productAssumptionParameterId == x.id)?.value;
                });
              });
            }
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
