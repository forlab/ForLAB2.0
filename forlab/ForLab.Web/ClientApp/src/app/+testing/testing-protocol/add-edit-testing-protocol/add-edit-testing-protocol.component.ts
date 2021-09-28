import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { TestingProtocolDto } from 'src/@core/models/testing/TestingProtocol';
import { CalculationPeriodEnum } from 'src/@core/models/enum/Enums';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { TestingProtocolsController } from 'src/@core/APIs/TestingProtocolsController';
import { takeUntil, map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { TestsController } from 'src/@core/APIs/TestsController';
import { PatientGroupsController } from 'src/@core/APIs/PatientGroupsController';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { TestingProtocolCalculationPeriodMonthDto } from 'src/@core/models/testing/TestingProtocolCalculationPeriodMonth';

@Component({
  selector: 'app-add-edit-testing-protocol',
  templateUrl: './add-edit-testing-protocol.component.html',
  styleUrls: ['./add-edit-testing-protocol.component.scss']
})
export class AddEditTestingProtocolComponent extends BaseService implements OnInit {

  originalTestingProtocolDto: TestingProtocolDto = new TestingProtocolDto();
  testingProtocolDto: TestingProtocolDto = new TestingProtocolDto();
  form: FormGroup;
  testingProtocolId: number;
  loadingTestingProtocol: boolean = false;
  isUpdate: boolean = false;
  // Drp
  calculationPeriodEnum = CalculationPeriodEnum;
  tests$: Observable<any[]>;
  patientGroups$: Observable<any[]>;

  constructor(private fb: FormBuilder, public injector: Injector) {
    super(injector);

    if (this.router.url.includes('update')) {
      this.isUpdate = true;
      this.activatedRoute.paramMap.subscribe(params => {
        this.testingProtocolId = Number(params.get('testingProtocolId'));
        this.loadDataById(this.testingProtocolId);
      });
    }

  }

  ngOnInit() {

    // Load Drp
    this.loadTests();
    this.loadPatientGroups();

    this.form = this.fb.group({
      testId: new FormControl(null, [Validators.required]),
      patientGroupId: new FormControl(null, [Validators.required]),
      calculationPeriodId: new FormControl(null, [Validators.required]),
      baseLine: new FormControl(null, [Validators.required]),
      testAfterFirstYear: new FormControl(null, [Validators.required]),
    });

    this.form.controls['calculationPeriodId'].valueChanges
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(calculationPeriodId => {
        this.calcMonths();
      });

  }

  loadDataById(id: number) {
    this.loadingTestingProtocol = true;
    const url = TestingProtocolsController.GetTestingProtocolDetails;
    let params: QueryParamsDto[] = [
      { key: 'testingProtocolId', value: id },
    ];

    this.httpService.GET(url, params)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res.isPassed) {
          this.testingProtocolDto = res.data;
          this.originalTestingProtocolDto = JSON.parse(JSON.stringify(res.data));
          this.loadingTestingProtocol = false;
          this.setFormValue();
        } else {
          this.alertService.error(res.message);
          this.loading = false;
          this._ref.detectChanges();
        }
      });
  }

  loadTests() {
    this.tests$ = this.httpService.GET(TestsController.GetAllAsDrp).pipe(map(res => res.data));
  }
  loadPatientGroups() {
    this.patientGroups$ = this.httpService.GET(PatientGroupsController.GetAllAsDrp).pipe(map(res => res.data));
  }

  setFormValue() {
    this.form.controls['testId'].patchValue(this.testingProtocolDto.testId);
    this.form.controls['patientGroupId'].patchValue(this.testingProtocolDto.patientGroupId);
    this.form.controls['calculationPeriodId'].patchValue(this.testingProtocolDto.calculationPeriodId, { emitEvent: false });
    this.form.controls['baseLine'].patchValue(this.testingProtocolDto.baseLine);
    this.form.controls['testAfterFirstYear'].patchValue(this.testingProtocolDto.testAfterFirstYear);
    this._ref.detectChanges();
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

    this.loading = true;

    // Set the data
    this.testingProtocolDto.testId = this.form.getRawValue().testId;
    this.testingProtocolDto.patientGroupId = this.form.getRawValue().patientGroupId;
    this.testingProtocolDto.calculationPeriodId = this.form.getRawValue().calculationPeriodId;
    this.testingProtocolDto.baseLine = this.form.getRawValue().baseLine;
    this.testingProtocolDto.testAfterFirstYear = this.form.getRawValue().testAfterFirstYear;

    // Incase Update
    if (this.isUpdate) {
      // Handle Months
      if (this.testingProtocolDto.calculationPeriodId == this.originalTestingProtocolDto.calculationPeriodId) {
        this.originalTestingProtocolDto.testingProtocolCalculationPeriodMonthDtos.forEach(x => {
          x.value = this.testingProtocolDto.testingProtocolCalculationPeriodMonthDtos.find(y => y.calculationPeriodMonthId == x.calculationPeriodMonthId).value;
        });
        this.testingProtocolDto.testingProtocolCalculationPeriodMonthDtos = this.originalTestingProtocolDto.testingProtocolCalculationPeriodMonthDtos;
      }
    }

    console.log(this.testingProtocolDto);

  }

  calcMonths() {
    const calculationPeriodId = this.form.getRawValue().calculationPeriodId;
    if (!calculationPeriodId) return;

    this.testingProtocolDto.testingProtocolCalculationPeriodMonthDtos = [];

    if (calculationPeriodId == this.calculationPeriodEnum.OneYear) {
      this.testingProtocolDto.testingProtocolCalculationPeriodMonthDtos = Array.from(Array(12), (_, i) => i + 1).map(x => {
        let val = new TestingProtocolCalculationPeriodMonthDto();
        val.calculationPeriodMonthId = x;
        val.calculationPeriodMonthName = `M${x}`;
        return val;
      });
    } else if (calculationPeriodId == this.calculationPeriodEnum.TwoYears) {
      this.testingProtocolDto.testingProtocolCalculationPeriodMonthDtos = Array.from(Array(24), (_, i) => i + 1).map(x => {
        let val = new TestingProtocolCalculationPeriodMonthDto();
        val.calculationPeriodMonthId = 12 + x;
        val.calculationPeriodMonthName = `M${x}`;
        return val;
      });
    }

  }
}
