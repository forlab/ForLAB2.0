import { Component, OnInit, Injector, Input, OnChanges } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { PatientAssumptionParametersController } from 'src/@core/APIs/PatientAssumptionParametersController';
import { takeUntil } from 'rxjs/operators';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { GroupPatientAssumptionParameterDto } from 'src/@core/models/program/PatientAssumptionParameter';
import { ProgramDto } from 'src/@core/models/program/Program';
import { ForecastPatientAssumptionValueDto } from 'src/@core/models/forecasting/ForecastPatientAssumptionValue';

@Component({
  selector: 'forecast-patient-assumptions',
  templateUrl: './forecast-patient-assumptions.component.html',
  styleUrls: ['./forecast-patient-assumptions.component.scss']
})
export class ForecastPatientAssumptionsComponent extends BaseService implements OnInit, OnChanges {
  // Inputs
  @Input('programDtos') programDtos: ProgramDto[] = [];
  @Input('data') forecastPatientAssumptionValueDtos: ForecastPatientAssumptionValueDto[] = [];
  // Vars
  groupPatientAssumptionParameterDto: GroupPatientAssumptionParameterDto[] = [];
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
    this.loadPatientAssumptions();
  }

  loadPatientAssumptions() {
    const programIds = this.programDtos.map(x => x.id);
    if (programIds.length == 0) {
      return;
    }

    let params: QueryParamsDto[] = [
      { key: 'programIds', value: programIds.join(',') },
    ];

    this.httpService.GET(PatientAssumptionParametersController.GetAllPatientAssumptionsForForcast, params)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.groupPatientAssumptionParameterDto = res.data;
            // Set values from the forecast
            if (this.forecastPatientAssumptionValueDtos?.length > 0) {
              this.groupPatientAssumptionParameterDto?.forEach(item => {
                item.patientAssumptionParameterDtos.forEach(x => {
                  x.value = this.forecastPatientAssumptionValueDtos.find(y => y.patientAssumptionParameterId == x.id)?.value;
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
