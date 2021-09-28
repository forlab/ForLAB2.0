import { Component, OnInit, Injector, Input, OnChanges } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { ForecastMorbidityTestingProtocolMonthDto } from 'src/@core/models/forecasting/ForecastMorbidityTestingProtocolMonth';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { TestDto } from 'src/@core/models/testing/Test';
import { TestingProtocolDto } from 'src/@core/models/testing/TestingProtocol';
import { PatientGroupDto } from 'src/@core/models/lookup/PatientGroup';
import { takeUntil } from 'rxjs/operators';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { ProgramDto } from 'src/@core/models/program/Program';
import { ProgramTestsController } from 'src/@core/APIs/ProgramTestsController';
import { ProgramTestDto } from 'src/@core/models/program/ProgramTest';

@Component({
  selector: 'forecast-testing-protocol-months',
  templateUrl: './forecast-testing-protocol-months.component.html',
  styleUrls: ['./forecast-testing-protocol-months.component.scss']
})
export class ForecastTestingProtocolMonthsComponent extends BaseService implements OnInit, OnChanges {

  // Inputs
  @Input('programDtos') programDtos: ProgramDto[] = [];
  //Vars
  @Input('data') forecastMorbidityTestingProtocolMonthDtos: ForecastMorbidityTestingProtocolMonthDto[] = [];
  form: FormGroup;
  selectedProgram: ProgramDto;
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
  // Drp
  tests: TestDto[] = [];
  testingProtocols: TestingProtocolDto[] = [];
  patientGroups: PatientGroupDto[] = [];

  constructor(public injector: Injector, private fb: FormBuilder) {
    super(injector);
  }

  ngOnInit(): void {
    this.form = this.fb.group({
      testId: new FormControl(null, [Validators.required]),
      patientGroupId: new FormControl(null, [Validators.required]),
      testingProtocolId: new FormControl(null, [Validators.required]),
    });
  }

  ngOnChanges() {
    const programIds = this.programDtos.map(x => x.id);
    if (!programIds || programIds.length == 0) return;
    this.forecastMorbidityTestingProtocolMonthDtos = this.forecastMorbidityTestingProtocolMonthDtos?.filter(x => programIds?.includes(x.programId));
  }

  loadProgramDetails(programId: number) {

    this.form.reset();

    let params: QueryParamsDto[] = [
      { key: 'programId', value: programId },
    ];

    this.httpService.GET(ProgramTestsController.GetAllAsDrp, params)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res.isPassed) {
          let resData = res.data as ProgramTestDto[];
          // Patient Groups
          this.patientGroups = resData.map(x => {
            let val = new PatientGroupDto();
            val.id = x.testingProtocolDto.patientGroupId;
            val.name = x.testingProtocolDto.patientGroupName;
            return val;
          });
          // Tests
          this.tests = resData.map(x => {
            let val = new TestDto();
            val.id = x.testId;
            val.name = x.testName;
            return val;
          });
          // Testing Protocols
          this.testingProtocols = resData.map(x => {
            let val = new TestingProtocolDto();
            val.id = x.testingProtocolDto.id;
            val.testId = x.testId;
            val.name = x.testingProtocolDto.name;
            val.testingProtocolCalculationPeriodMonthDtos = x.testingProtocolDto.testingProtocolCalculationPeriodMonthDtos;
            return val;
          });

          // Refresh
          this.forecastMorbidityTestingProtocolMonthDtos?.forEach(month => {
            // UI
            month.testName = this.tests.find(x => x.id == month.testId)?.name;
            month.patientGroupName = this.patientGroups.find(x => x.id == month.patientGroupId)?.name;
            month.testingProtocolName = this.testingProtocols.find(x => x.id == month.testingProtocolId)?.name;
            month.testingProtocolCalculationPeriodMonthDtos = this.testingProtocols.find(x => x.id == month.testingProtocolId)?.testingProtocolCalculationPeriodMonthDtos;
            // set month data
            month?.testingProtocolCalculationPeriodMonthDtos?.forEach(testMonth => {
              testMonth.value = this.forecastMorbidityTestingProtocolMonthDtos.find(x => x.testingProtocolId == testMonth.testingProtocolId && x.calculationPeriodMonthId == testMonth.calculationPeriodMonthId)?.value;
            });
          });
          this.forecastMorbidityTestingProtocolMonthDtos = this.forecastMorbidityTestingProtocolMonthDtos?.filter((item, index) => {
            return this.forecastMorbidityTestingProtocolMonthDtos.findIndex(x => x.testId == item.testId && x.testingProtocolId == item.testingProtocolId && x.patientGroupId == item.patientGroupId) >= index;
          });

          this._ref.detectChanges();
        } else {
          this.alertService.error(res.message);
          this.loading = false;
          this._ref.detectChanges();
        }
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

    // Check duplication
    if (this.forecastMorbidityTestingProtocolMonthDtos.findIndex(x =>
      x.testId == this.form.getRawValue().testId
      && x.testingProtocolId == this.form.getRawValue().testingProtocolId
      && x.patientGroupId == this.form.getRawValue().patientGroupId) > -1) {
      this.alertService.error('It\'s have already added');
      return;
    }

    let val = new ForecastMorbidityTestingProtocolMonthDto();
    // Set the data
    val.programId = this.selectedProgram.id;
    val.testId = this.form.getRawValue().testId;
    val.testingProtocolId = this.form.getRawValue().testingProtocolId;
    val.patientGroupId = this.form.getRawValue().patientGroupId;
    // UI
    val.testName = this.tests.find(x => x.id == val.testId).name;
    val.patientGroupName = this.patientGroups.find(x => x.id == val.patientGroupId).name;
    val.testingProtocolName = this.testingProtocols.find(x => x.id == val.testingProtocolId).name;
    val.testingProtocolCalculationPeriodMonthDtos = this.testingProtocols.find(x => x.id == val.testingProtocolId).testingProtocolCalculationPeriodMonthDtos;

    // Add to the list
    this.forecastMorbidityTestingProtocolMonthDtos.push(val);
    this.form.reset();
  }


  deleteMonth(val: ForecastMorbidityTestingProtocolMonthDto) {
    let index = this.forecastMorbidityTestingProtocolMonthDtos.indexOf(val);
    this.forecastMorbidityTestingProtocolMonthDtos.splice(index, 1);
  }

  getFilteredProtocols(testId: number): TestingProtocolDto[] {
    return this.testingProtocols.filter(x => x.testId == testId);
  }

  get getFilteredMonths(): ForecastMorbidityTestingProtocolMonthDto[] {
    if (!this.selectedProgram) return [];
    return this.forecastMorbidityTestingProtocolMonthDtos.filter(x => x.programId == this.selectedProgram.id);
  }

  get getFinalForecastMonths(): ForecastMorbidityTestingProtocolMonthDto[] {
    if (!this.forecastMorbidityTestingProtocolMonthDtos) return [];
    let result: ForecastMorbidityTestingProtocolMonthDto[] = [];

    this.forecastMorbidityTestingProtocolMonthDtos?.forEach(x => {
      let forecastMonths = x.testingProtocolCalculationPeriodMonthDtos?.map(month => {
        let val = new ForecastMorbidityTestingProtocolMonthDto();
        val.calculationPeriodMonthId = month.calculationPeriodMonthId;
        val.value = month.value;
        val.programId = x.programId;
        val.testId = x.testId;
        val.testingProtocolId = x.testingProtocolId;
        val.patientGroupId = x.patientGroupId;
        return val;
      });

      result = result.concat(forecastMonths);
    });

    return result;
  }

}
