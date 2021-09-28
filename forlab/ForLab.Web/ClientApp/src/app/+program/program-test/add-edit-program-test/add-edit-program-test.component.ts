import { Component, OnInit, Inject, Injector, ViewEncapsulation } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { ProgramTestDto } from 'src/@core/models/program/ProgramTest';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ProgramTestsController } from 'src/@core/APIs/ProgramTestsController';
import { takeUntil, map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { ProgramsController } from 'src/@core/APIs/ProgramsController';
import { TestsController } from 'src/@core/APIs/TestsController';
import { PatientGroupsController } from 'src/@core/APIs/PatientGroupsController';
import { CalculationPeriodEnum } from 'src/@core/models/enum/Enums';
import { TestingProtocolDto } from 'src/@core/models/testing/TestingProtocol';
import { TestingProtocolCalculationPeriodMonthDto } from 'src/@core/models/testing/TestingProtocolCalculationPeriodMonth';
class Params {
  programId: number;
  fromProgramDetails: boolean = false;
  programTestDto: ProgramTestDto;
}

@Component({
  selector: 'app-add-edit-program-test',
  templateUrl: './add-edit-program-test.component.html',
  styleUrls: ['./add-edit-program-test.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class AddEditProgramTestComponent extends BaseService implements OnInit {

  programTestDto: ProgramTestDto = new ProgramTestDto();
  form: FormGroup;
  mode: 'create' | 'update' = 'create';
  // Drp
  programs$: Observable<any[]>;
  tests$: Observable<any[]>;
  patientGroups$: Observable<any[]>;
  calculationPeriodEnum = CalculationPeriodEnum;

  constructor(@Inject(MAT_DIALOG_DATA) public data: Params,
    public dialogRef: MatDialogRef<AddEditProgramTestComponent>,
    private fb: FormBuilder,
    public injector: Injector) {
    super(injector);
  }

  ngOnInit() {

    // Load Drp
    this.loadPrograms();
    this.loadPatientGroups();
    this.loadTests();

    this.form = this.fb.group({
      programId: new FormControl(null, [Validators.required]),
      testId: new FormControl(null, [Validators.required]),
      // Testing Protocol
      testingProtocolName: [null, Validators.compose([Validators.required])],
      patientGroupId: [null, Validators.compose([Validators.required])],
      baseLine: [null, Validators.compose([Validators.required])],
      testAfterFirstYear: [null, Validators.compose([Validators.required])],
      calculationPeriodId: [null, Validators.compose([Validators.required])],
    });

    this.form.controls['calculationPeriodId'].valueChanges
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(calculationPeriodId => {
        console.log('done');
        
        this.calcMonths();
      });

    if (this.data.fromProgramDetails) {
      this.form.controls['programId'].patchValue(this.data.programId);
      this.form.controls['programId'].disable();
    }

    if (this.data && this.data.programTestDto && this.data.programTestDto.id) {
      this.mode = 'update';
      this.setFormData();
    } else {
      this.programTestDto = this.data.programTestDto || new ProgramTestDto();
    }

  }

  loadPrograms() {
    this.programs$ = this.httpService.GET(ProgramsController.GetAllAsDrp).pipe(map(res => res.data));
  }
  loadTests() {
    this.tests$ = this.httpService.GET(TestsController.GetAllAsDrp).pipe(map(res => res.data));
  }
  loadPatientGroups() {
    this.patientGroups$ = this.httpService.GET(PatientGroupsController.GetAllAsDrp).pipe(map(res => res.data));
  }


  setFormData() {
    this.programTestDto = JSON.parse(JSON.stringify(this.data.programTestDto));
    this.form.controls['programId'].patchValue(this.programTestDto.programId);
    this.form.controls['testId'].patchValue(this.programTestDto.testId);
    // Testing Protocol
    this.form.controls['testingProtocolName'].patchValue(this.programTestDto.testingProtocolDto?.name);
    this.form.controls['patientGroupId'].patchValue(this.programTestDto.testingProtocolDto?.patientGroupId);
    this.form.controls['baseLine'].patchValue(this.programTestDto.testingProtocolDto?.baseLine);
    this.form.controls['testAfterFirstYear'].patchValue(this.programTestDto.testingProtocolDto?.testAfterFirstYear);
    this.form.controls['calculationPeriodId'].patchValue(this.programTestDto.testingProtocolDto?.calculationPeriodId, { emitEvent: false });
    this._ref.detectChanges();
  }

  createObject() {
    this.httpService.POST(ProgramTestsController.CreateProgramTest, this.programTestDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.alertService.success('Program Test is created successfully');
            this.dialogRef.close(this.programTestDto);
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


  updateObject() {
    this.httpService.PUT(ProgramTestsController.UpdateProgramTest, this.programTestDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.dialogRef.close(this.programTestDto);
            this.alertService.success('Program Test is updated successfully');
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

  save() {
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
    this.programTestDto.programId = Number(this.form.getRawValue().programId);
    this.programTestDto.testId = Number(this.form.getRawValue().testId);
    // Testing Protocol
    this.programTestDto.testingProtocolDto.name = this.form.getRawValue().testingProtocolName;
    this.programTestDto.testingProtocolDto.testId = Number(this.form.getRawValue().testId);
    this.programTestDto.testingProtocolDto.patientGroupId = Number(this.form.getRawValue().patientGroupId);
    this.programTestDto.testingProtocolDto.baseLine = Number(this.form.getRawValue().baseLine);
    this.programTestDto.testingProtocolDto.testAfterFirstYear = Number(this.form.getRawValue().testAfterFirstYear);
    this.programTestDto.testingProtocolDto.calculationPeriodId = Number(this.form.getRawValue().calculationPeriodId);

    if (this.mode === 'create') {
      this.createObject();
    } else if (this.mode === 'update') {
      this.updateObject();
    }
  }

  calcMonths() {
    const calculationPeriodId = this.form.getRawValue().calculationPeriodId;
    if (!calculationPeriodId) return;

    if (!this.programTestDto?.testingProtocolDto) {
      this.programTestDto.testingProtocolDto = new TestingProtocolDto();
    }

    if (calculationPeriodId == this.calculationPeriodEnum.OneYear) {
      this.programTestDto.testingProtocolDto.testingProtocolCalculationPeriodMonthDtos = Array.from(Array(12), (_, i) => i + 1).map(x => {
        let val = new TestingProtocolCalculationPeriodMonthDto();
        val.calculationPeriodMonthId = x;
        val.calculationPeriodMonthName = `M${x}`;
        return val;
      });
    } else if (calculationPeriodId == this.calculationPeriodEnum.TwoYears) {
      this.programTestDto.testingProtocolDto.testingProtocolCalculationPeriodMonthDtos = Array.from(Array(24), (_, i) => i + 1).map(x => {
        let val = new TestingProtocolCalculationPeriodMonthDto();
        val.calculationPeriodMonthId = 12 + x;
        val.calculationPeriodMonthName = `M${x}`;
        return val;
      });
    }

  }

  isCreateMode() {
    return this.mode === 'create';
  }

  isUpdateMode() {
    return this.mode === 'update';
  }
}
