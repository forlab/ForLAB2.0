import { Component, OnInit, Injector, ViewChild, Input } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { ForecastInfoDto } from 'src/@core/models/forecasting/ForecastInfo';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { ForecastInfosController } from 'src/@core/APIs/ForecastInfosController';
import { takeUntil, map } from 'rxjs/operators';
import { ForecastInfoLevelEnum, ForecastMethodologyEnum, ScopeOfTheForecastEnum, ForecastInfoStatusEnum, CountryPeriodEnum } from 'src/@core/models/enum/Enums';
import { merge } from 'rxjs';
import { UserSubscriptionsController } from 'src/@core/APIs/UserSubscriptionsController';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { CountryDto } from 'src/@core/models/lookup/Country';
import { ForecastPatientAssumptionsComponent } from '../shared/forecast-patient-assumptions/forecast-patient-assumptions.component';
// Shared Components
import { ForecastMorbidityProgramsComponent } from '../shared/forecast-morbidity-programs/forecast-morbidity-programs.component';
import { ForecastInstrumentsComponent } from '../shared/forecast-instruments/forecast-instruments.component';
import { ForecastPatientGroupsComponent } from '../shared/forecast-patient-groups/forecast-patient-groups.component';
import { ForecastProductAssumptionsComponent } from '../shared/forecast-product-assumptions/forecast-product-assumptions.component';
import { ForecastTestingAssumptionsComponent } from '../shared/forecast-testing-assumptions/forecast-testing-assumptions.component';
import { ForecastTargetBasesComponent } from '../shared/forecast-target-bases/forecast-target-bases.component';
import { ForecastTestingProtocolMonthsComponent } from '../shared/forecast-testing-protocol-months/forecast-testing-protocol-months.component';
import { ForecastTestsComponent } from '../shared/forecast-tests/forecast-tests.component';
import { HistoicalConsumptionsComponent } from '../shared/histoical-consumptions/histoical-consumptions.component';
import { HistoicalServiceDataComponent } from '../shared/histoical-service-data/histoical-service-data.component';
// Date
import { MomentDateAdapter, MAT_MOMENT_DATE_ADAPTER_OPTIONS } from '@angular/material-moment-adapter';
import { DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material/core';
import { MatDatepicker } from '@angular/material/datepicker';
import * as _moment from 'moment';
import { Moment } from 'moment';
import { ProgramDto } from 'src/@core/models/program/Program';
import { HistoicalServiceDataDto, HistoicalConsumptionDto, HistoicalTargetBaseDto, HistoicalWhoBaseDto } from 'src/@core/models/forecasting/ImportedFileTemplate';
import { ForecastMorbidityProgramDto } from 'src/@core/models/forecasting/ForecastMorbidityProgram';
import { ForecastPatientAssumptionValueDto } from 'src/@core/models/forecasting/ForecastPatientAssumptionValue';
import { PatientAssumptionParameterDto } from 'src/@core/models/program/PatientAssumptionParameter';
import { ProductAssumptionParameterDto } from 'src/@core/models/program/ProductAssumptionParameter';
import { ForecastProductAssumptionValueDto } from 'src/@core/models/forecasting/ForecastProductAssumptionValue';
import { TestingAssumptionParameterDto } from 'src/@core/models/program/TestingAssumptionParameter';
import { ForecastTestingAssumptionValueDto } from 'src/@core/models/forecasting/ForecastTestingAssumptionValue';
import { ForecastWhoBasesComponent } from '../shared/forecast-who-bases/forecast-who-bases.component';
const moment = _moment;
export const MY_FORMATS = {
  parse: {
    dateInput: 'MM/YYYY',
  },
  display: {
    dateInput: 'MM/YYYY',
    monthYearLabel: 'MMM YYYY',
    dateA11yLabel: 'LL',
    monthYearA11yLabel: 'MMMM YYYY',
  },
};

@Component({
  selector: 'app-add-forecast',
  templateUrl: './add-forecast.component.html',
  styleUrls: ['./add-forecast.component.scss'],
  providers: [
    // `MomentDateAdapter` can be automatically provided by importing `MomentDateModule` in your
    // application's root module. We provide it at the component level here, due to limitations of
    // our example generation script.
    {
      provide: DateAdapter,
      useClass: MomentDateAdapter,
      deps: [MAT_DATE_LOCALE, MAT_MOMENT_DATE_ADAPTER_OPTIONS]
    },

    { provide: MAT_DATE_FORMATS, useValue: MY_FORMATS },
  ],
})
export class AddForecastComponent extends BaseService implements OnInit {

  // Children
  @ViewChild(ForecastProductAssumptionsComponent, { static: false }) forecastProductAssumptionsComponent: ForecastProductAssumptionsComponent;
  @ViewChild(ForecastPatientAssumptionsComponent, { static: false }) forecastPatientAssumptionsComponent: ForecastPatientAssumptionsComponent;
  @ViewChild(ForecastTestingAssumptionsComponent, { static: false }) forecastTestingAssumptionsComponent: ForecastTestingAssumptionsComponent;
  @ViewChild(ForecastInstrumentsComponent, { static: false }) forecastInstrumentsComponent: ForecastInstrumentsComponent;
  @ViewChild(ForecastTestingProtocolMonthsComponent, { static: false }) forecastTestingProtocolMonthsComponent: ForecastTestingProtocolMonthsComponent;
  @ViewChild(ForecastMorbidityProgramsComponent, { static: false }) forecastMorbidityProgramsComponent: ForecastMorbidityProgramsComponent;
  @ViewChild(ForecastPatientGroupsComponent, { static: false }) forecastPatientGroupsComponent: ForecastPatientGroupsComponent;
  @ViewChild(ForecastTargetBasesComponent, { static: false }) forecastTargetBasesComponent: ForecastTargetBasesComponent;
  @ViewChild(ForecastWhoBasesComponent, { static: false }) forecastWhoBasesComponent: ForecastWhoBasesComponent;
  @ViewChild(ForecastTestsComponent, { static: false }) forecastTestsComponent: ForecastTestsComponent;
  @ViewChild(HistoicalConsumptionsComponent, { static: false }) histoicalConsumptionsComponent: HistoicalConsumptionsComponent;
  @ViewChild(HistoicalServiceDataComponent, { static: false }) histoicalServiceDataComponent: HistoicalServiceDataComponent;
  // Vars
  forecastInfoDto: ForecastInfoDto = new ForecastInfoDto();
  form: FormGroup;
  methodologyForm: FormGroup;
  isLinear = true;
  isMorbidity = false;
  wastageRate: number = null;
  // Drp
  countryPeriodEnum = CountryPeriodEnum;
  forecastInfoLevelEnum = ForecastInfoLevelEnum;
  forecastMethodologyEnum = ForecastMethodologyEnum;
  scopeOfTheForecastEnum = ScopeOfTheForecastEnum;
  forecastInfoStatusEnum = ForecastInfoStatusEnum;
  userCountries: CountryDto[];
  // Helper
  getAddedPrograms: ProgramDto[] = [];

  constructor(private fb: FormBuilder, public injector: Injector) {
    super(injector);
  }

  ngOnInit() {
    // Load Drp
    this.loadUserCountries();

    this.form = this.fb.group({
      forecastInfoLevelId: new FormControl({ disabled: true, value: this.loggedInUser.userSubscriptionLevelId }, [Validators.required]),
      countryId: new FormControl(null, [Validators.required]),
      scopeOfTheForecastId: new FormControl(null, [Validators.required]),
      name: new FormControl(null, [Validators.required]),
      startDate: new FormControl(moment(), [Validators.required]),
      endDate: new FormControl({ disabled: true, value: null }, [Validators.required]),
      duration: new FormControl(null, [Validators.required]),
    });

    this.methodologyForm = this.fb.group({
      forecastMethodologyId: new FormControl(null, [Validators.required]),
      isWorldHealthOrganization: new FormControl(false, [Validators.required]),
    });

    this.methodologyForm.controls['forecastMethodologyId'].valueChanges
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(forecastMethodologyId => {
        if (forecastMethodologyId == this.forecastMethodologyEnum.DempgraphicMorbidity) {
          this.isMorbidity = true;
        }
        else {
          this.isMorbidity = false;
        }
      });

    // track any changes to calc end date
    merge(
      this.form.controls['countryId'].valueChanges,
      this.form.controls['startDate'].valueChanges,
      this.form.controls['duration'].valueChanges)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        let endDate = this.calcEndDate(this.form.getRawValue().startDate, this.form.getRawValue().duration, this.form.getRawValue().countryId);
        this.form.controls['endDate'].patchValue(endDate);
      });


  }

  loadUserCountries() {
    let params: QueryParamsDto[] = [
      { key: 'applicationUserId', value: this.loggedInUser.id },
    ];
    this.httpService.GET(UserSubscriptionsController.GetUserCountriesAsDrp, params)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.userCountries = res.data;
            if (this.userCountries && this.userCountries.length == 1) {
              this.form.controls['countryId'].patchValue(this.userCountries[0].id);
            }
            this._ref.detectChanges();
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

  submitForm() {

    const controls = this.form.controls;
    /** check form */
    if (this.form.invalid) {
      Object.keys(controls).forEach(controlName =>
        controls[controlName].markAsTouched()
      );
      return;
    }

    // Set the data
    this.forecastInfoDto = new ForecastInfoDto();
    this.forecastInfoDto.wastageRate = this.wastageRate;
    // Step 1 (Define Forecast)
    this.forecastInfoDto.forecastInfoLevelId = this.form.getRawValue().forecastInfoLevelId;
    this.forecastInfoDto.countryId = this.form.getRawValue().countryId;
    this.forecastInfoDto.scopeOfTheForecastId = this.form.getRawValue().scopeOfTheForecastId;
    this.forecastInfoDto.name = this.form.getRawValue().name;
    this.forecastInfoDto.startDate = moment(this.form.getRawValue().startDate).add(1, 'd').toISOString();
    this.forecastInfoDto.endDate = moment(this.form.getRawValue().endDate).add(1, 'd').toISOString();
    this.forecastInfoDto.duration = this.form.getRawValue().duration;
    // Step 2 (Methodology)
    this.forecastInfoDto.forecastMethodologyId = this.methodologyForm.getRawValue().forecastMethodologyId;
    this.forecastInfoDto.isWorldHealthOrganization = this.methodologyForm.getRawValue().isWorldHealthOrganization;
    this.forecastInfoDto.isTargetBased = !this.methodologyForm.getRawValue().isWorldHealthOrganization;
    // Service
    if (this.forecastInfoDto.forecastMethodologyId == this.forecastMethodologyEnum.Service) {
      // Step (Tests)
      this.forecastInfoDto.forecastTestDtos = this.forecastTestsComponent.forecastTestDtos;
      // Step (Instruments)
      this.forecastInfoDto.forecastInstrumentDtos = this.forecastInstrumentsComponent.forecastInstrumentDtos;
      // Step (Histoical Service Data)
      this.forecastInfoDto.isAggregate = this.histoicalServiceDataComponent.isAggregate;
      this.forecastInfoDto.isSiteBySite = !this.histoicalServiceDataComponent.isAggregate;
      this.forecastInfoDto.histoicalServiceDataDtos = this.histoicalServiceDataComponent.formArry?.map(element => {
        let val = new HistoicalServiceDataDto();
        val.regionId = Number(element.value.regionId);
        val.laboratoryId = Number(element.value.laboratoryId);
        val.testId = Number(element.value.testId);
        val.forecastCategoryName = String(element.value.forecastCategoryName);
        return val;
      });
    }
    // Consumption
    else if (this.forecastInfoDto.forecastMethodologyId == this.forecastMethodologyEnum.Consumption) {
      // Step (Histoical Consumption)
      this.forecastInfoDto.isAggregate = this.histoicalConsumptionsComponent.isAggregate;
      this.forecastInfoDto.isSiteBySite = !this.histoicalConsumptionsComponent.isAggregate;
      this.forecastInfoDto.histoicalConsumptionDtos = this.histoicalConsumptionsComponent.formArry?.map(element => {
        let val = new HistoicalConsumptionDto();
        val.regionId = Number(element.value.regionId);
        val.laboratoryId = Number(element.value.laboratoryId);
        val.productId = Number(element.value.productId);
        val.forecastCategoryName = String(element.value.forecastCategoryName);
        return val;
      });
    }
    // DempgraphicMorbidity
    else if (this.forecastInfoDto.forecastMethodologyId == this.forecastMethodologyEnum.DempgraphicMorbidity) {
      // Step (Instruments)
      this.forecastInfoDto.forecastInstrumentDtos = this.forecastInstrumentsComponent.forecastInstrumentDtos;
      // Step (Programs)
      this.forecastInfoDto.forecastMorbidityProgramDtos = this.forecastMorbidityProgramsComponent.getObjects().controls.map(element => {
        let val = new ForecastMorbidityProgramDto();
        val.programId = Number(element.value.programId);
        return val;
      });
      // Step (Patient Groups)
      this.forecastInfoDto.forecastPatientGroupDtos = this.forecastPatientGroupsComponent.getFinalForecastPatientGroup;
      // Step (Assumptions)
      // Patient
      let patientAssumptions: PatientAssumptionParameterDto[] = [];
      this.forecastPatientAssumptionsComponent.groupPatientAssumptionParameterDto.forEach(x => patientAssumptions = patientAssumptions.concat(x.patientAssumptionParameterDtos));
      this.forecastInfoDto.forecastPatientAssumptionValueDtos = patientAssumptions.map(element => {
        let val = new ForecastPatientAssumptionValueDto();
        val.patientAssumptionParameterId = Number(element.id);
        val.value = Number(element.value);
        return val;
      });
      // Product
      let productAssumptions: ProductAssumptionParameterDto[] = [];
      this.forecastProductAssumptionsComponent.groupProductAssumptionParameterDto.forEach(x => productAssumptions = productAssumptions.concat(x.productAssumptionParameterDtos));
      this.forecastInfoDto.forecastProductAssumptionValueDtos = productAssumptions.map(element => {
        let val = new ForecastProductAssumptionValueDto();
        val.productAssumptionParameterId = Number(element.id);
        val.value = Number(element.value);
        return val;
      });
      // Testing
      let testingAssumptions: TestingAssumptionParameterDto[] = [];
      this.forecastTestingAssumptionsComponent.groupTestingAssumptionParameterDto.forEach(x => testingAssumptions = testingAssumptions.concat(x.testingAssumptionParameterDtos));
      this.forecastInfoDto.forecastTestingAssumptionValueDtos = testingAssumptions.map(element => {
        let val = new ForecastTestingAssumptionValueDto();
        val.testingAssumptionParameterId = Number(element.id);
        val.value = Number(element.value);
        return val;
      });
      // Step (Months)
      this.forecastInfoDto.forecastMorbidityTestingProtocolMonthDtos = this.forecastTestingProtocolMonthsComponent.getFinalForecastMonths;

      if (this.forecastInfoDto.isTargetBased) {
        // Step (Target Bases)
        this.forecastInfoDto.isAggregate = this.forecastTargetBasesComponent.isAggregate;
        this.forecastInfoDto.isSiteBySite = !this.forecastTargetBasesComponent.isAggregate;
        this.forecastInfoDto.histoicalTargetBaseDtos = this.forecastTargetBasesComponent.formArry.map(element => {
          let val = new HistoicalTargetBaseDto();
          val.regionId = Number(element.value.regionId);
          val.laboratoryId = Number(element.value.laboratoryId);
          val.forecastCategoryName = String(element.value.forecastCategoryName);
          val.programId = Number(element.value.programId);
          val.currentPatient = Number(element.value.currentPatient);
          val.targetPatient = Number(element.value.targetPatient);
          return val;
        });
      } else {
        // Step (WHO)
        this.forecastInfoDto.histoicalWhoBaseDtos = this.forecastWhoBasesComponent.getObjects().controls.map(element => {
          let val = new HistoicalWhoBaseDto();
          val.countryId = Number(element.value.countryId);
          val.diseaseId = Number(element.value.diseaseId);
          return val;
        });
      }
    }
    console.log(this.forecastInfoDto);
    this.loading = true;

    this.httpService.POST(ForecastInfosController.CreateForecastInfo, this.forecastInfoDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.alertService.success('Forecast is created successfully');
            this.router.navigate(['/forecasting/forecasts']);
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

  // Helper method
  calcEndDate(startDate, duration: number, countryId: number): string {
    let country = this.userCountries.find(x => x.id == countryId);
    if (!country) return null;
    if (!startDate) return null;
    if (!duration) return null;

    // startDate = startDate.toISOString();
    if (country.countryPeriodId == this.countryPeriodEnum.Weekly) {
      return moment(startDate).add(duration, 'weeks').toISOString();
    } else if (country.countryPeriodId == this.countryPeriodEnum.Monthly) {
      return moment(startDate).add(duration, 'months').toISOString();
    } else if (country.countryPeriodId == this.countryPeriodEnum.Annualy) {
      return moment(startDate).add(duration, 'years').toISOString();
    } else if (country.countryPeriodId == this.countryPeriodEnum.Quarterly) {
      return moment(startDate).add(duration, 'quarters').toISOString();
    }

  }

  chosenYearHandler(normalizedYear: Moment) {
    const ctrlValue = this.form.getRawValue().startDate;
    ctrlValue.year(normalizedYear.year());
    this.form.controls['startDate'].setValue(ctrlValue);
  }

  chosenMonthHandler(normalizedMonth: Moment, datepicker: MatDatepicker<Moment>) {
    const ctrlValue = this.form.getRawValue().startDate;
    ctrlValue.month(normalizedMonth.month());
    this.form.controls['startDate'].setValue(ctrlValue);
    datepicker.close();
  }

  // Program
  refreshAddedPrograms(programDtos: ProgramDto[]) {
    if (!programDtos) {
      this.getAddedPrograms = [];
    }
    this.getAddedPrograms = programDtos;
  }
}
