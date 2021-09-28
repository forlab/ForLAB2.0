import { Component, OnInit, Inject, Injector } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { PatientGroupDto } from 'src/@core/models/lookup/PatientGroup';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { PatientGroupsController } from 'src/@core/APIs/PatientGroupsController';
import { takeUntil, map } from 'rxjs/operators';

@Component({
  selector: 'app-add-edit-patient-group',
  templateUrl: './add-edit-patient-group.component.html',
  styleUrls: ['./add-edit-patient-group.component.sass']
})
export class AddEditPatientGroupComponent extends BaseService implements OnInit {

  patientGroupDto: PatientGroupDto = new PatientGroupDto();
  form: FormGroup;
  mode: 'create' | 'update' = 'create';

  constructor(@Inject(MAT_DIALOG_DATA) public data: PatientGroupDto,
    public dialogRef: MatDialogRef<AddEditPatientGroupComponent>,
    private fb: FormBuilder,
    public injector: Injector) {
    super(injector);
  }

  ngOnInit() {

    this.form = this.fb.group({
      name: new FormControl(null, [Validators.required]),
    });

    if (this.data && this.data.id) {
      this.mode = 'update';
      this.setFormData();
    } else {
      this.patientGroupDto = this.data || new PatientGroupDto();
    }

  }

  setFormData() {
    this.form.controls['name'].patchValue(this.data.name);
    this.patientGroupDto = JSON.parse(JSON.stringify(this.data));
    this._ref.detectChanges();
  }

  createObject() {
    this.httpService.POST(PatientGroupsController.CreatePatientGroup, this.patientGroupDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.alertService.success('Patient Group is created successfully');
            this.dialogRef.close(this.patientGroupDto);
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
    this.httpService.PUT(PatientGroupsController.UpdatePatientGroup, this.patientGroupDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.dialogRef.close(this.patientGroupDto);
            this.alertService.success('Patient Group is updated successfully');
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
    this.patientGroupDto.name = this.form.getRawValue().name;

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
