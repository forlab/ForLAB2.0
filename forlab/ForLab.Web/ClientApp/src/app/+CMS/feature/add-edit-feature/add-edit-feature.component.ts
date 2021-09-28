import { Component, OnInit, Inject, Injector } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { FeatureDto } from 'src/@core/models/CMS/Feature';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FeaturesController } from 'src/@core/APIs/FeaturesController';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-add-edit-feature',
  templateUrl: './add-edit-feature.component.html',
  styleUrls: ['./add-edit-feature.component.scss']
})
export class AddEditFeatureComponent extends BaseService implements OnInit {
  
  featureDto: FeatureDto = new FeatureDto();
  form: FormGroup;
  mode: 'create' | 'update' = 'create';
  imageToUpload: File;

  constructor(@Inject(MAT_DIALOG_DATA) public data: FeatureDto,
    public dialogRef: MatDialogRef<AddEditFeatureComponent>,
    private fb: FormBuilder,
    public injector: Injector) {
    super(injector);
  }

  ngOnInit() {

    this.form = this.fb.group({
      title: new FormControl(null, [Validators.required]),
      description: new FormControl(null, [Validators.required]),
    });

    if (this.data && this.data.id) {
      this.mode = 'update';
      this.setFormData();
    } else {
      this.featureDto = this.data || new FeatureDto();
    }

  }

  setFormData() {
    this.form.controls['title'].patchValue(this.data.title);
    this.form.controls['description'].patchValue(this.data.description);
    this.featureDto = JSON.parse(JSON.stringify(this.data));
    this._ref.detectChanges();
  }

  createObject() {
    var formData: FormData = new FormData();
    if (this.imageToUpload) {
      formData.append('logoPath', this.imageToUpload, this.imageToUpload.name);
    }
    formData.append('featureDto', JSON.stringify(this.featureDto));

    this.httpService.POST(FeaturesController.CreateFeature, formData, null, true)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.alertService.success('Feature is created successfully');
            this.dialogRef.close(this.featureDto);
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
    var formData: FormData = new FormData();
    if (this.imageToUpload) {
      formData.append('logoPath', this.imageToUpload, this.imageToUpload.name);
    }
    formData.append('featureDto', JSON.stringify(this.featureDto));

    this.httpService.PUT(FeaturesController.UpdateFeature, formData, null, true)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.dialogRef.close(this.featureDto);
            this.alertService.success('Feature is updated successfully');
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
    this.featureDto.title = this.form.getRawValue().title;
    this.featureDto.description = this.form.getRawValue().description;
    console.log(this.featureDto);

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

  previewImage(event, imageTag) {
    if(!event || !event.target.files[0]) {
      return;
    }
    this.imageToUpload = event.target.files[0];
    var reader = new FileReader();
    reader.onloadend = () => {
      imageTag.src = reader.result;
    };
    reader.readAsDataURL(this.imageToUpload);
  }
}
