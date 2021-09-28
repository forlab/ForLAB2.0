import { Component, OnInit, Injector, Input, OnChanges } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { TestingAssumptionParametersController } from 'src/@core/APIs/TestingAssumptionParametersController';
import { takeUntil } from 'rxjs/operators';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { GroupTestingAssumptionParameterDto } from 'src/@core/models/program/TestingAssumptionParameter';
import { ProgramDto } from 'src/@core/models/program/Program';
import { ForecastTestingAssumptionValueDto } from 'src/@core/models/forecasting/ForecastTestingAssumptionValue';

@Component({
  selector: 'forecast-testing-assumptions',
  templateUrl: './forecast-testing-assumptions.component.html',
  styleUrls: ['./forecast-testing-assumptions.component.scss']
})
export class ForecastTestingAssumptionsComponent extends BaseService implements OnInit, OnChanges {
  // Inputs
  @Input('programDtos') programDtos: ProgramDto[] = [];
  @Input('data') forecastTestingAssumptionValueDtos: ForecastTestingAssumptionValueDto[] = [];
  // Vars
  groupTestingAssumptionParameterDto: GroupTestingAssumptionParameterDto[] = [];
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
    this.loadTestingAssumptions();
  }

  loadTestingAssumptions() {
    const programIds = this.programDtos.map(x => x.id);
    if (programIds.length == 0) {
      return;
    }

    let params: QueryParamsDto[] = [
      { key: 'programIds', value: programIds.join(',') },
    ];

    this.httpService.GET(TestingAssumptionParametersController.GetAllTestingAssumptionsForForcast, params)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.groupTestingAssumptionParameterDto = res.data;
            // Set values from the forecast
            if (this.forecastTestingAssumptionValueDtos?.length > 0) {
              this.groupTestingAssumptionParameterDto?.forEach(item => {
                item.testingAssumptionParameterDtos.forEach(x => {
                  x.value = this.forecastTestingAssumptionValueDtos.find(y => y.testingAssumptionParameterId == x.id)?.value;
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
