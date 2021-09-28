import { Component, OnInit, Inject, Injector } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { LaboratoryCategoryDto } from 'src/@core/models/lookup/LaboratoryCategory';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { LaboratoryCategoriesController } from 'src/@core/APIs/LaboratoryCategoriesController';
import { takeUntil, map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { CountriesController } from 'src/@core/APIs/CountriesController';

@Component({
  selector: 'app-add-edit-laboratory-category',
  templateUrl: './add-edit-laboratory-category.component.html',
  styleUrls: ['./add-edit-laboratory-category.component.sass']
})
export class AddEditLaboratoryCategoryComponent extends BaseService implements OnInit {

  laboratoryCategoryDto: LaboratoryCategoryDto = new LaboratoryCategoryDto();
  form: FormGroup;
  mode: 'create' | 'update' = 'create';

  constructor(@Inject(MAT_DIALOG_DATA) public data: LaboratoryCategoryDto,
    public dialogRef: MatDialogRef<AddEditLaboratoryCategoryComponent>,
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
      this.laboratoryCategoryDto = this.data || new LaboratoryCategoryDto();
    }

  }

  setFormData() {
    this.form.controls['name'].patchValue(this.data.name);
    this.laboratoryCategoryDto = JSON.parse(JSON.stringify(this.data));
    this._ref.detectChanges();
  }

  createObject() {
    this.httpService.POST(LaboratoryCategoriesController.CreateLaboratoryCategory, this.laboratoryCategoryDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.alertService.success('Laboratory Category is created successfully');
            this.dialogRef.close(this.laboratoryCategoryDto);
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
    this.httpService.PUT(LaboratoryCategoriesController.UpdateLaboratoryCategory, this.laboratoryCategoryDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.dialogRef.close(this.laboratoryCategoryDto);
            this.alertService.success('Laboratory Category is updated successfully');
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
    this.laboratoryCategoryDto.name = this.form.getRawValue().name;

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
