import { Component, OnInit, Inject, Injector, ViewEncapsulation } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { PatientAssumptionParameterDto } from 'src/@core/models/program/PatientAssumptionParameter';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { PatientAssumptionParametersController } from 'src/@core/APIs/PatientAssumptionParametersController';
import { takeUntil, map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { ProgramsController } from 'src/@core/APIs/ProgramsController';
class Params {
  programId: number;
  fromProgramDetails: boolean = false;
  patientAssumptionParameterDto: PatientAssumptionParameterDto;
}

@Component({
  selector: 'app-add-edit-patient-assumption',
  templateUrl: './add-edit-patient-assumption.component.html',
  styleUrls: ['./add-edit-patient-assumption.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class AddEditPatientAssumptionComponent extends BaseService implements OnInit {

  patientAssumptionParameterDto: PatientAssumptionParameterDto = new PatientAssumptionParameterDto();
  form: FormGroup;
  mode: 'create' | 'update' = 'create';
  // Drp
  programs$: Observable<any[]>;

  constructor(@Inject(MAT_DIALOG_DATA) public data: Params,
    public dialogRef: MatDialogRef<AddEditPatientAssumptionComponent>,
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

    if (this.data && this.data.patientAssumptionParameterDto && this.data.patientAssumptionParameterDto.id) {
      this.mode = 'update';
      this.setFormData();
    } else {
      this.patientAssumptionParameterDto = this.data.patientAssumptionParameterDto || new PatientAssumptionParameterDto();
    }


  }

  loadPrograms() {
    this.programs$ = this.httpService.GET(ProgramsController.GetAllAsDrp).pipe(map(res => res.data));
  }

  setFormData() {
    this.patientAssumptionParameterDto = JSON.parse(JSON.stringify(this.data.patientAssumptionParameterDto));
    this.form.controls['programId'].patchValue(this.patientAssumptionParameterDto.programId);
    this.form.controls['name'].patchValue(this.patientAssumptionParameterDto.name);
    this.form.controls['isPercentage'].patchValue(this.patientAssumptionParameterDto.isPercentage);
    this.form.controls['isPositive'].patchValue(this.patientAssumptionParameterDto.isPositive);
    this._ref.detectChanges();
  }

  createObject() {
    this.httpService.POST(PatientAssumptionParametersController.CreatePatientAssumptionParameter, this.patientAssumptionParameterDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.alertService.success('Patient Assumption is created successfully');
            this.dialogRef.close(this.patientAssumptionParameterDto);
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
    this.httpService.PUT(PatientAssumptionParametersController.UpdatePatientAssumptionParameter, this.patientAssumptionParameterDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.dialogRef.close(this.patientAssumptionParameterDto);
            this.alertService.success('Patient Assumption is updated successfully');
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
    this.patientAssumptionParameterDto.programId = Number(this.form.getRawValue().programId);
    this.patientAssumptionParameterDto.name = this.form.getRawValue().name;
    this.patientAssumptionParameterDto.isPercentage = this.form.getRawValue().isPercentage;
    this.patientAssumptionParameterDto.isNumeric = !this.form.getRawValue().isPercentage;
    this.patientAssumptionParameterDto.isPositive = this.form.getRawValue().isPositive;
    this.patientAssumptionParameterDto.isNegative = !this.form.getRawValue().isPositive;

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
