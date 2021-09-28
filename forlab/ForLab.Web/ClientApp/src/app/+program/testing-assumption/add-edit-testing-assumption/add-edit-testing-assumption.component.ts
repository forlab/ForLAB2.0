import { Component, OnInit, Inject, Injector, ViewEncapsulation } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { TestingAssumptionParameterDto } from 'src/@core/models/program/TestingAssumptionParameter';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TestingAssumptionParametersController } from 'src/@core/APIs/TestingAssumptionParametersController';
import { takeUntil, map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { ProgramsController } from 'src/@core/APIs/ProgramsController';
class Params {
  programId: number;
  fromProgramDetails: boolean = false;
  testingAssumptionParameterDto: TestingAssumptionParameterDto;
}

@Component({
  selector: 'app-add-edit-testing-assumption',
  templateUrl: './add-edit-testing-assumption.component.html',
  styleUrls: ['./add-edit-testing-assumption.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class AddEditTestingAssumptionComponent extends BaseService implements OnInit {

  testingAssumptionParameterDto: TestingAssumptionParameterDto = new TestingAssumptionParameterDto();
  form: FormGroup;
  mode: 'create' | 'update' = 'create';
  // Drp
  programs$: Observable<any[]>;

  constructor(@Inject(MAT_DIALOG_DATA) public data: Params,
    public dialogRef: MatDialogRef<AddEditTestingAssumptionComponent>,
    private fb: FormBuilder,
    public injector: Injector) {
    super(injector);
  }

  ngOnInit() {

    // Load Drp
    this.loadPrograms();

    this.form = this.fb.group({
      programId: new FormControl(null, [Validators.required]),
      name: new FormControl(null, [Validators.required]),
      isPercentage: new FormControl(false, [Validators.required]),
      isPositive: new FormControl(true, [Validators.required]),
    });

    if (this.data.fromProgramDetails) {
      this.form.controls['programId'].patchValue(this.data.programId);
      this.form.controls['programId'].disable();
    }

    if (this.data && this.data.testingAssumptionParameterDto && this.data.testingAssumptionParameterDto.id) {
      this.mode = 'update';
      this.setFormData();
    } else {
      this.testingAssumptionParameterDto = this.data.testingAssumptionParameterDto || new TestingAssumptionParameterDto();
    }


  }

  loadPrograms() {
    this.programs$ = this.httpService.GET(ProgramsController.GetAllAsDrp).pipe(map(res => res.data));
  }

  setFormData() {
    this.testingAssumptionParameterDto = JSON.parse(JSON.stringify(this.data.testingAssumptionParameterDto));
    this.form.controls['programId'].patchValue(this.testingAssumptionParameterDto.programId);
    this.form.controls['name'].patchValue(this.testingAssumptionParameterDto.name);
    this.form.controls['isPercentage'].patchValue(this.testingAssumptionParameterDto.isPercentage);
    this.form.controls['isPositive'].patchValue(this.testingAssumptionParameterDto.isPositive);
    this._ref.detectChanges();
  }

  createObject() {
    this.httpService.POST(TestingAssumptionParametersController.CreateTestingAssumptionParameter, this.testingAssumptionParameterDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.alertService.success('Testing Assumption is created successfully');
            this.dialogRef.close(this.testingAssumptionParameterDto);
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
    this.httpService.PUT(TestingAssumptionParametersController.UpdateTestingAssumptionParameter, this.testingAssumptionParameterDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.dialogRef.close(this.testingAssumptionParameterDto);
            this.alertService.success('Testing Assumption is updated successfully');
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
    this.testingAssumptionParameterDto.programId = Number(this.form.getRawValue().programId);
    this.testingAssumptionParameterDto.name = this.form.getRawValue().name;
    this.testingAssumptionParameterDto.isPercentage = this.form.getRawValue().isPercentage;
    this.testingAssumptionParameterDto.isNumeric = !this.form.getRawValue().isPercentage;
    this.testingAssumptionParameterDto.isPositive = this.form.getRawValue().isPositive;
    this.testingAssumptionParameterDto.isNegative = !this.form.getRawValue().isPositive;

    if (this.mode === 'create') {
      this.createObject();
    } else if (this.mode === 'update') {
      this.updateObject();
    }
  }


  isCreateMode() {
    return this.mode === 'create';
  }

  isUpdateMode() {
    return this.mode === 'update';
  }
}
