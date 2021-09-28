import { Component, OnInit, Injector } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { ForecastInfoDto } from 'src/@core/models/forecasting/ForecastInfo';
import { ForecastInfosController } from 'src/@core/APIs/ForecastInfosController';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { takeUntil } from 'rxjs/operators';
import { ForecastMethodologyEnum } from 'src/@core/models/enum/Enums';

@Component({
  selector: 'app-forecast-details',
  templateUrl: './forecast-details.component.html',
  styleUrls: ['./forecast-details.component.scss']
})
export class ForecastDetailsComponent extends BaseService implements OnInit {

  forecastInfoId: number;
  forecastInfoDto: ForecastInfoDto = new ForecastInfoDto();
  loadingForecastInfo = false;
  // Drp
  forecastMethodologyEnum = ForecastMethodologyEnum;
  // Menue
  liks = [
    { title: 'Instruments',  objectName: 'ForecastInstrument', visible: false },
    { title: 'Laboratories',  objectName: 'ForecastLaboratory', visible: false },
    { title: 'Laboratory Consumptions',  objectName: 'ForecastLaboratoryConsumption', visible: false },
    { title: 'Laboratory Test Services',  objectName: 'ForecastLaboratoryTestService', visible: false },
    { title: 'Morbidity Programs',  objectName: 'ForecastMorbidityProgram', visible: false },
    { title: 'Target Bases',  objectName: 'ForecastMorbidityTargetBase', visible: false },
    { title: 'Protocol Months',  objectName: 'ForecastMorbidityTestingProtocolMonth', visible: false },
    { title: 'WHO Bases',  objectName: 'ForecastMorbidityWhoBase', visible: false },
    { title: 'Patient Assumptions',  objectName: 'ForecastPatientAssumptionValue', visible: false },
    { title: 'Product Assumptions',  objectName: 'ForecastProductAssumptionValue', visible: false },
    { title: 'Testing Assumptions',  objectName: 'ForecastTestingAssumptionValue', visible: false },
    { title: 'Forecast Tests',  objectName: 'ForecastTest', visible: false },
    { title: 'Patient Groups',  objectName: 'ForecastPatientGroup', visible: false },
    { title: 'Forecast Results',  objectName: 'ForecastResult', visible: false },
  ]

  constructor(public injector: Injector) {
    super(injector);

    if (this.router.url.includes('details')) {
      this.activatedRoute.paramMap.subscribe(params => {
        this.forecastInfoId = Number(params.get('forecastInfoId'));
        this.loadDataById(this.forecastInfoId);
      });
    }

  }

  ngOnInit(): void {
  }

  loadDataById(id: number) {
    this.loadingForecastInfo = true;
    const url = ForecastInfosController.GetForecastInfoDetails;
    let params: QueryParamsDto[] = [
      { key: 'forecastInfoId', value: id },
    ];

    this.httpService.GET(url, params)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res.isPassed) {
          this.forecastInfoDto = res.data;
          this.loadingForecastInfo = false;
          this.updateVisible();
        } else {
          this.alertService.error(res.message);
          this.loading = false;
          this._ref.detectChanges();
        }
      });
  }

  updateVisible() { 

    if(this.forecastInfoDto.forecastMethodologyId == this.forecastMethodologyEnum.Service) {

      this.liks.find(x => x.objectName == 'ForecastTest').visible = true;
      this.liks.find(x => x.objectName == 'ForecastInstrument').visible = true;
      this.liks.find(x => x.objectName == 'ForecastLaboratory').visible = true;
      this.liks.find(x => x.objectName == 'ForecastLaboratoryTestService').visible = true;
      this.liks.find(x => x.objectName == 'ForecastResult').visible = true;

    } else if(this.forecastInfoDto.forecastMethodologyId == this.forecastMethodologyEnum.Consumption) {

      this.liks.find(x => x.objectName == 'ForecastLaboratory').visible = true;
      this.liks.find(x => x.objectName == 'ForecastLaboratoryConsumption').visible = true;
      this.liks.find(x => x.objectName == 'ForecastResult').visible = true;

    } else if (this.forecastInfoDto.forecastMethodologyId == this.forecastMethodologyEnum.DempgraphicMorbidity && this.forecastInfoDto.isTargetBased) {

      this.liks.find(x => x.objectName == 'ForecastInstrument').visible = true;
      this.liks.find(x => x.objectName == 'ForecastLaboratory').visible = true;
      this.liks.find(x => x.objectName == 'ForecastMorbidityProgram').visible = true;
      this.liks.find(x => x.objectName == 'ForecastMorbidityTargetBase').visible = true;
      this.liks.find(x => x.objectName == 'ForecastPatientGroup').visible = true;
      this.liks.find(x => x.objectName == 'ForecastMorbidityTestingProtocolMonth').visible = true;
      this.liks.find(x => x.objectName == 'ForecastPatientAssumptionValue').visible = true;
      this.liks.find(x => x.objectName == 'ForecastProductAssumptionValue').visible = true;
      this.liks.find(x => x.objectName == 'ForecastTestingAssumptionValue').visible = true;
      this.liks.find(x => x.objectName == 'ForecastResult').visible = true;

    } else if(this.forecastInfoDto.forecastMethodologyId == this.forecastMethodologyEnum.DempgraphicMorbidity && this.forecastInfoDto.isWorldHealthOrganization) {

      this.liks.find(x => x.objectName == 'ForecastInstrument').visible = true;
      this.liks.find(x => x.objectName == 'ForecastMorbidityProgram').visible = true;
      this.liks.find(x => x.objectName == 'ForecastMorbidityWhoBase').visible = true;
      this.liks.find(x => x.objectName == 'ForecastPatientGroup').visible = true;
      this.liks.find(x => x.objectName == 'ForecastMorbidityTestingProtocolMonth').visible = true;
      this.liks.find(x => x.objectName == 'ForecastPatientAssumptionValue').visible = true;
      this.liks.find(x => x.objectName == 'ForecastProductAssumptionValue').visible = true;
      this.liks.find(x => x.objectName == 'ForecastTestingAssumptionValue').visible = true;

    }

  }

}
