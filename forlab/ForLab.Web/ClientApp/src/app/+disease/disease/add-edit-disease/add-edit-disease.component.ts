import { Component, OnInit, Inject, Injector } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { DiseaseDto } from 'src/@core/models/disease/Disease';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { DiseasesController } from 'src/@core/APIs/DiseasesController';
import { takeUntil, map } from 'rxjs/operators';

@Component({
  selector: 'app-add-edit-disease',
  templateUrl: './add-edit-disease.component.html',
  styleUrls: ['./add-edit-disease.component.scss']
})
export class AddEditDiseaseComponent extends BaseService implements OnInit {

  diseaseDto: DiseaseDto = new DiseaseDto();
  form: FormGroup;
  mode: 'create' | 'update' = 'create';

  constructor(@Inject(MAT_DIALOG_DATA) public data: DiseaseDto,
    public dialogRef: MatDialogRef<AddEditDiseaseComponent>,
    private fb: FormBuilder,
    public injector: Injector) {
    super(injector);
  }

  ngOnInit() {

    this.form = this.fb.group({
      name: new FormControl(null, [Validators.required]),
      description: new FormControl(null, []),
    });

    if (this.data && this.data.id) {
      this.mode = 'update';
      this.setFormData();
    } else {
      this.diseaseDto = this.data || new DiseaseDto();
    }

  }

  setFormData() {
    this.form.controls['name'].patchValue(this.data.name);
    this.form.controls['description'].patchValue(this.data.description);
    this.diseaseDto = JSON.parse(JSON.stringify(this.data));
    this._ref.detectChanges();
  }

  createObject() {
    this.httpService.POST(DiseasesController.CreateDisease, this.diseaseDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.alertService.success('Disease is created successfully');
            this.dialogRef.close(this.diseaseDto);
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
    this.httpService.PUT(DiseasesController.UpdateDisease, this.diseaseDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.dialogRef.close(this.diseaseDto);
            this.alertService.success('Disease is updated successfully');
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
    this.diseaseDto.name = this.form.getRawValue().name;
    this.diseaseDto.description = this.form.getRawValue().description;

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
