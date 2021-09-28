import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { LaboratoryDto } from 'src/@core/models/lookup/Laboratory';
import { LaboratoriesController } from 'src/@core/APIs/LaboratoriesController';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { takeUntil } from 'rxjs/operators';
import { LaboratoryWorkingDaysComponent } from '../../laboratory-working-day/laboratory-working-days/laboratory-working-days.component';
import { LaboratoryConsumptionsComponent } from '../../laboratory-consumption/laboratory-consumptions/laboratory-consumptions.component';
import { LaboratoryInstrumentsComponent } from '../../laboratory-instrument/laboratory-instruments/laboratory-instruments.component';
import { LaboratoryPatientStatisticsComponent } from '../../laboratory-patient-statistic/laboratory-patient-statistics/laboratory-patient-statistics.component';
import { LaboratoryTestServicesComponent } from '../../laboratory-test-service/laboratory-test-services/laboratory-test-services.component';

@Component({
  selector: 'app-laboratory-details',
  templateUrl: './laboratory-details.component.html',
  styleUrls: ['./laboratory-details.component.scss']
})
export class LaboratoryDetailsComponent extends BaseService implements OnInit {

  // Children
  @ViewChild(LaboratoryConsumptionsComponent, { static: false }) laboratoryConsumptionsComponent: LaboratoryConsumptionsComponent;
  @ViewChild(LaboratoryInstrumentsComponent, { static: false }) laboratoryInstrumentsComponent: LaboratoryInstrumentsComponent;
  @ViewChild(LaboratoryPatientStatisticsComponent, { static: false }) laboratoryPatientStatisticsComponent: LaboratoryPatientStatisticsComponent;
  @ViewChild(LaboratoryWorkingDaysComponent, { static: false }) laboratoryWorkingDaysComponent: LaboratoryWorkingDaysComponent;
  @ViewChild(LaboratoryTestServicesComponent, { static: false }) laboratoryTestServicesComponent: LaboratoryTestServicesComponent;

  laboratoryId: number;
  laboratoryDto: LaboratoryDto = new LaboratoryDto();
  loadingLaboratory = false;

  constructor(public injector: Injector) {
    super(injector);

    if (this.router.url.includes('details')) {
      this.activatedRoute.paramMap.subscribe(params => {
        this.laboratoryId = Number(params.get('laboratoryId'));
        this.loadDataById(this.laboratoryId);
      });
    }

  }

  ngOnInit(): void {
  }

  loadDataById(id: number) {
    this.loadingLaboratory = true;
    const url = LaboratoriesController.GetLaboratoryDetails;
    let params: QueryParamsDto[] = [
      { key: 'laboratoryId', value: id },
    ];

    this.httpService.GET(url, params)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res.isPassed) {
          this.laboratoryDto = res.data;
          this.loadingLaboratory = false;

          if (this.laboratoryDto.createdBy != this.loggedInUser?.id) {
            this.laboratoryConsumptionsComponent.columns = this.laboratoryConsumptionsComponent?.columns.filter(x => x.property != 'select' && x.property != 'actions');
            this.laboratoryInstrumentsComponent.columns = this.laboratoryInstrumentsComponent?.columns.filter(x => x.property != 'select' && x.property != 'actions');
            this.laboratoryPatientStatisticsComponent.columns = this.laboratoryPatientStatisticsComponent?.columns.filter(x => x.property != 'select' && x.property != 'actions');
            this.laboratoryWorkingDaysComponent.columns = this.laboratoryWorkingDaysComponent?.columns.filter(x => x.property != 'select' && x.property != 'actions');
            this.laboratoryTestServicesComponent.columns = this.laboratoryTestServicesComponent?.columns.filter(x => x.property != 'select' && x.property != 'actions');
          }

        } else {
          this.alertService.error(res.message);
          this.loading = false;
          this._ref.detectChanges();
        }
      });
  }

}
