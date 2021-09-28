import { Component, OnInit, Inject, Injector } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { LaboratoryLevelDto } from 'src/@core/models/lookup/LaboratoryLevel';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { LaboratoryLevelsController } from 'src/@core/APIs/LaboratoryLevelsController';
import { takeUntil, map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { CountriesController } from 'src/@core/APIs/CountriesController';

@Component({
  selector: 'app-add-edit-laboratory-level',
  templateUrl: './add-edit-laboratory-level.component.html',
  styleUrls: ['./add-edit-laboratory-level.component.sass']
})
export class AddEditLaboratoryLevelComponent extends BaseService implements OnInit {

  laboratoryLevelDto: LaboratoryLevelDto = new LaboratoryLevelDto();
  form: FormGroup;
  mode: 'create' | 'update' = 'create';

  constructor(@Inject(MAT_DIALOG_DATA) public data: LaboratoryLevelDto,
    public dialogRef: MatDialogRef<AddEditLaboratoryLevelComponent>,
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
      this.laboratoryLevelDto = this.data || new LaboratoryLevelDto();
    }

  }

  setFormData() {
    this.form.controls['name'].patchValue(this.data.name);
    this.laboratoryLevelDto = JSON.parse(JSON.stringify(this.data));
    this._ref.detectChanges();
  }

  createObject() {
    this.httpService.POST(LaboratoryLevelsController.CreateLaboratoryLevel, this.laboratoryLevelDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.alertService.success('Patient Group is created successfully');
            this.dialogRef.close(this.laboratoryLevelDto);
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
    this.httpService.PUT(LaboratoryLevelsController.UpdateLaboratoryLevel, this.laboratoryLevelDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.dialogRef.close(this.laboratoryLevelDto);
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
    this.laboratoryLevelDto.name = this.form.getRawValue().name;

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
