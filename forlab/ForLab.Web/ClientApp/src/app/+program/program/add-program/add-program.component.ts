import { Component, OnInit, Injector, ViewChild, Input } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { ProgramDto } from 'src/@core/models/program/Program';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { ProgramsController } from 'src/@core/APIs/ProgramsController';
import { DiseasesController } from 'src/@core/APIs/DiseasesController';
import { takeUntil, map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { ImportTestingAssumptionParametersComponent } from 'src/app/+import-feature/import-testing-assumption-parameters/import-testing-assumption-parameters.component';
import { ImportProductAssumptionParametersComponent } from 'src/app/+import-feature/import-product-assumption-parameters/import-product-assumption-parameters.component';
import { ImportPatientAssumptionParametersComponent } from 'src/app/+import-feature/import-patient-assumption-parameters/import-patient-assumption-parameters.component';
import { TestingAssumptionParameterDto } from 'src/@core/models/program/TestingAssumptionParameter';
import { PatientAssumptionParameterDto } from 'src/@core/models/program/PatientAssumptionParameter';
import { ProductAssumptionParameterDto } from 'src/@core/models/program/ProductAssumptionParameter';
import { ProgramTestDto } from 'src/@core/models/program/ProgramTest';
import { ImportProgramTestsComponent } from 'src/app/+import-feature/import-program-tests/import-program-tests.component';
import { TestingProtocolDto } from 'src/@core/models/testing/TestingProtocol';
import { TestingProtocolCalculationPeriodMonthDto } from 'src/@core/models/testing/TestingProtocolCalculationPeriodMonth';

@Component({
  selector: 'app-add-program',
  templateUrl: './add-program.component.html',
  styleUrls: ['./add-program.component.scss']
})
export class AddProgramComponent extends BaseService implements OnInit {

  // Children
  @ViewChild(ImportProgramTestsComponent, { static: false }) importProgramTestsComponent: ImportProgramTestsComponent;
  @ViewChild(ImportTestingAssumptionParametersComponent, { static: false }) importTestingAssumptionParametersComponent: ImportTestingAssumptionParametersComponent;
  @ViewChild(ImportProductAssumptionParametersComponent, { static: false }) importProductAssumptionParametersComponent: ImportProductAssumptionParametersComponent;
  @ViewChild(ImportPatientAssumptionParametersComponent, { static: false }) importPatientAssumptionParametersComponent: ImportPatientAssumptionParametersComponent;
  // Vars
  programDto: ProgramDto = new ProgramDto();
  form: FormGroup;
  isLinear = true;
  // Drp
  diseases$: Observable<any[]>;

  constructor(private fb: FormBuilder, public injector: Injector) {
    super(injector);
  }

  ngOnInit() {
    this.loadDiseases();

    this.form = this.fb.group({
      diseaseId: new FormControl(null, [Validators.required]),
      name: new FormControl(null, [Validators.required]),
      numberOfYears: new FormControl(1, [Validators.required]),
    });

  }

  loadDiseases() {
    this.diseases$ = this.httpService.GET(DiseasesController.GetAllAsDrp).pipe(map(res => res.data));
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

    // Set the values
    this.programDto.name = this.form.getRawValue().name;
    this.programDto.diseaseId = Number(this.form.getRawValue().diseaseId);
    this.programDto.numberOfYears = Number(this.form.getRawValue().numberOfYears);
    // Rest
    this.programDto.programTestDtos = [];
    this.programDto.testingAssumptionParameterDtos = [];
    this.programDto.patientAssumptionParameterDtos = [];
    this.programDto.productAssumptionParameterDtos = [];
    // Set ProgramTestDtos
    this.programDto.programTestDtos = this.importProgramTestsComponent.formArry?.map((element, i) => {
      let val = new ProgramTestDto();
      val.testId = Number(element.value.testId);
      val.programId = Number(element.value.programId);
      // TestingProtocol
      val.testingProtocolDto = new TestingProtocolDto();
      val.testingProtocolDto.name = String(element.value.testingProtocolName);
      val.testingProtocolDto.testId = Number(element.value.testId);
      val.testingProtocolDto.patientGroupId = Number(element.value.patientGroupId);
      val.testingProtocolDto.baseLine = Number(element.value.baseLine);
      val.testingProtocolDto.testAfterFirstYear = Number(element.value.testAfterFirstYear);
      // Month
      var testingProtocol = this.importProgramTestsComponent?.data[i]?.testingProtocolDto;
      val.testingProtocolDto.calculationPeriodId = testingProtocol?.calculationPeriodId || 1;
      if (testingProtocol?.testingProtocolCalculationPeriodMonthDtos?.length > 0) {
        val.testingProtocolDto.testingProtocolCalculationPeriodMonthDtos = testingProtocol?.testingProtocolCalculationPeriodMonthDtos
      } else {
        val.testingProtocolDto.testingProtocolCalculationPeriodMonthDtos = Array.from(Array(12), (_, i) => i + 1).map(x => {
          let month = new TestingProtocolCalculationPeriodMonthDto();
          month.calculationPeriodMonthId = x;
          return month;
        });
      }

      return val;
    });
    // Set TestingAssumptionParameterDtos
    this.programDto.testingAssumptionParameterDtos = this.importTestingAssumptionParametersComponent?.formArry?.map(element => {
      let val = new TestingAssumptionParameterDto();
      val.name = element.value.name;
      val.isPercentage = element.value.isPercentage;
      val.isNumeric = !element.value.isPercentage;
      val.isPositive = element.value.isPositive;
      val.isNegative = !element.value.isPositive;
      val.programId = Number(element.value.programId);
      return val;
    });
    // Set PatientAssumptionParameterDtos
    this.programDto.patientAssumptionParameterDtos = this.importPatientAssumptionParametersComponent.formArry?.map(element => {
      let val = new PatientAssumptionParameterDto();
      val.name = element.value.name;
      val.isPercentage = element.value.isPercentage;
      val.isNumeric = !element.value.isPercentage;
      val.isPositive = element.value.isPositive;
      val.isNegative = !element.value.isPositive;
      val.programId = Number(element.value.programId);
      return val;
    });
    // Set PatientAssumptionParameterDtos
    this.programDto.productAssumptionParameterDtos = this.importProductAssumptionParametersComponent.formArry?.map(element => {
      let val = new ProductAssumptionParameterDto();
      val.name = element.value.name;
      val.isPercentage = element.value.isPercentage;
      val.isNumeric = !element.value.isPercentage;
      val.isPositive = element.value.isPositive;
      val.isNegative = !element.value.isPositive;
      val.programId = Number(element.value.programId);
      return val;
    });

    console.log(this.programDto);

    this.httpService.POST(ProgramsController.CreateProgram, this.programDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.alertService.success('Program is created successfully');
            this.router.navigate(['/program/programs'])
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
