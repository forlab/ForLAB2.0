import { Component, OnInit, Inject, Injector } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { UsefulResourceDto } from 'src/@core/models/CMS/UsefulResource';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { UsefulResourcesController } from 'src/@core/APIs/UsefulResourcesController';
import { takeUntil } from 'rxjs/operators';
import { ConfigurationDto } from 'src/@core/models/configuration/Configuration';
import { ConfigurationsController } from 'src/@core/APIs/ConfigurationsController';
import { RxwebValidators } from '@rxweb/reactive-form-validators';

@Component({
  selector: 'app-add-edit-useful-resource',
  templateUrl: './add-edit-useful-resource.component.html',
  styleUrls: ['./add-edit-useful-resource.component.scss']
})
export class AddEditUsefulResourceComponent extends BaseService implements OnInit {

  usefulResourceDto: UsefulResourceDto = new UsefulResourceDto();
  fileToUpload: File;
  configurationDto: ConfigurationDto = new ConfigurationDto();
  form: FormGroup;
  mode: 'create' | 'update' = 'create';

  constructor(@Inject(MAT_DIALOG_DATA) public data: UsefulResourceDto,
    public dialogRef: MatDialogRef<AddEditUsefulResourceComponent>,
    private fb: FormBuilder,
    public injector: Injector) {
    super(injector);
  }

  ngOnInit() {

    this.loadConfiguration();

    this.form = this.fb.group({
      title: new FormControl(null, [Validators.required]),
      isExternalResource: new FormControl(true, [Validators.required]),
      attachmentUrl: new FormControl(null, [Validators.required, RxwebValidators.url()]),
    });

    this.form.controls['isExternalResource'].valueChanges
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(isExternalResource => {
        this.form.controls['attachmentUrl'].patchValue(null);
        if (isExternalResource) {
          this.form.controls['attachmentUrl'].setValidators([Validators.required]);
        } else {
          this.form.controls['attachmentUrl'].clearValidators();
        }
        this.form.controls['attachmentUrl'].updateValueAndValidity();
      });

    if (this.data && this.data.id) {
      this.mode = 'update';
      this.setFormData();
    } else {
      this.usefulResourceDto = this.data || new UsefulResourceDto();
    }

  }

  loadConfiguration() {
    const url = ConfigurationsController.GetConfigurationDetails;
    this.httpService.GET(url)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res.isPassed) {
          this.configurationDto = res.data;
        } else {
          this.alertService.error(res.message);
          this.loading = false;
          this._ref.detectChanges();
        }
      });
  }

  setFormData() {
    this.form.controls['title'].patchValue(this.data.title);
    this.form.controls['isExternalResource'].patchValue(this.data.isExternalResource);
    this.form.controls['attachmentUrl'].patchValue(this.data.attachmentUrl);
    this.usefulResourceDto = JSON.parse(JSON.stringify(this.data));
    this._ref.detectChanges();
  }

  saveFileToVariable(event) {
    if (!event || !event.target.files[0]) {
      return;
    }
    if ((event.target.files[0].size / 1000) >= this.configurationDto.attachmentsMaxSize) {
      this.alertService.error(`File size must be smaller than ${this.configurationDto.attachmentsMaxSize} KB`);
      return;
    }

    this.fileToUpload = event.target.files[0];
    this.usefulResourceDto.attachmentName = event.target.files[0].name;
    this.usefulResourceDto.extensionFormat = event.target.files[0].name.substr(event.target.files[0].name.lastIndexOf('.') + 1);
    this.usefulResourceDto.attachmentSize = event.target.files[0].size;
  }


  createObject() {
    let formData: FormData = new FormData();
    if (this.fileToUpload) {
      formData.append('fileToUpload', this.fileToUpload, this.fileToUpload.name);
    }
    formData.append('usefulResourceDto', JSON.stringify(this.usefulResourceDto));

    this.httpService.POST(UsefulResourcesController.CreateUsefulResource, formData, null, true)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.alertService.success('Useful Resource is created successfully');
            this.dialogRef.close(this.usefulResourceDto);
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
    let formData: FormData = new FormData();
    if (this.fileToUpload) {
      formData.append('fileToUpload', this.fileToUpload, this.fileToUpload.name);
    }
    formData.append('usefulResourceDto', JSON.stringify(this.usefulResourceDto));

    this.httpService.PUT(UsefulResourcesController.UpdateUsefulResource, formData, null, true)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.dialogRef.close(this.usefulResourceDto);
            this.alertService.success('Useful Resource is updated successfully');
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

    if(!this.form.getRawValue().isExternalResource && !this.fileToUpload) {
      this.alertService.error('Please upload the file first');
      return;
    }

    this.loading = true;

    // Set the data
    this.usefulResourceDto.title = this.form.getRawValue().title;
    this.usefulResourceDto.isExternalResource = this.form.getRawValue().isExternalResource;
    if(this.form.getRawValue().isExternalResource) {
      this.usefulResourceDto.attachmentUrl = this.form.getRawValue().attachmentUrl;
      this.usefulResourceDto.attachmentName = null;
      this.usefulResourceDto.extensionFormat = null;
      this.usefulResourceDto.attachmentSize = null;
    }
    console.log(this.usefulResourceDto);

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
