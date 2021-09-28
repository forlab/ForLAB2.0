import { Component, OnInit, Inject, Injector } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { ChannelVideoDto } from 'src/@core/models/CMS/ChannelVideo';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ChannelVideosController } from 'src/@core/APIs/ChannelVideosController';
import { takeUntil } from 'rxjs/operators';
import { ConfigurationDto } from 'src/@core/models/configuration/Configuration';
import { ConfigurationsController } from 'src/@core/APIs/ConfigurationsController';
import { RxwebValidators } from '@rxweb/reactive-form-validators';

@Component({
  selector: 'app-add-edit-channel-video',
  templateUrl: './add-edit-channel-video.component.html',
  styleUrls: ['./add-edit-channel-video.component.scss']
})
export class AddEditChannelVideoComponent extends BaseService implements OnInit {

  channelVideoDto: ChannelVideoDto = new ChannelVideoDto();
  fileToUpload: File;
  configurationDto: ConfigurationDto = new ConfigurationDto();
  form: FormGroup;
  mode: 'create' | 'update' = 'create';

  constructor(@Inject(MAT_DIALOG_DATA) public data: ChannelVideoDto,
    public dialogRef: MatDialogRef<AddEditChannelVideoComponent>,
    private fb: FormBuilder,
    public injector: Injector) {
    super(injector);
  }

  ngOnInit() {

    this.loadConfiguration();

    this.form = this.fb.group({
      title: new FormControl(null, [Validators.required]),
      description: new FormControl(null, []),
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
      this.channelVideoDto = this.data || new ChannelVideoDto();
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
    this.form.controls['description'].patchValue(this.data.description);
    this.form.controls['isExternalResource'].patchValue(this.data.isExternalResource);
    this.form.controls['attachmentUrl'].patchValue(this.data.attachmentUrl);
    this.channelVideoDto = JSON.parse(JSON.stringify(this.data));
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
    this.channelVideoDto.attachmentName = event.target.files[0].name;
    this.channelVideoDto.extensionFormat = event.target.files[0].name.substr(event.target.files[0].name.lastIndexOf('.') + 1);
    this.channelVideoDto.attachmentSize = event.target.files[0].size;
  }


  createObject() {
    let formData: FormData = new FormData();
    if (this.fileToUpload) {
      formData.append('fileToUpload', this.fileToUpload, this.fileToUpload.name);
    }
    formData.append('channelVideoDto', JSON.stringify(this.channelVideoDto));

    this.httpService.POST(ChannelVideosController.CreateChannelVideo, formData, null, true)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.alertService.success('Channel Video is created successfully');
            this.dialogRef.close(this.channelVideoDto);
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
    formData.append('channelVideoDto', JSON.stringify(this.channelVideoDto));

    this.httpService.PUT(ChannelVideosController.UpdateChannelVideo, formData, null, true)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.dialogRef.close(this.channelVideoDto);
            this.alertService.success('Channel Video is updated successfully');
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

    if (!this.form.getRawValue().isExternalResource && !this.fileToUpload) {
      this.alertService.error('Please upload the file first');
      return;
    }

    this.loading = true;

    // Set the data
    this.channelVideoDto.title = this.form.getRawValue().title;
    this.channelVideoDto.description = this.form.getRawValue().description;
    this.channelVideoDto.isExternalResource = this.form.getRawValue().isExternalResource;
    if (this.form.getRawValue().isExternalResource) {
      this.channelVideoDto.attachmentUrl = this.getId(this.form.getRawValue().attachmentUrl);
      this.channelVideoDto.attachmentName = null;
      this.channelVideoDto.extensionFormat = null;
      this.channelVideoDto.attachmentSize = null;
    }

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

  getId(url) {
    var regExp = /^.*(youtu.be\/|v\/|u\/\w\/|embed\/|watch\?v=|\&v=)([^#\&\?]*).*/;
    var match = url.match(regExp);

    if (match && match[2].length == 11) {
      return `//www.youtube.com/embed/` + match[2];
    } else {
      return 'error';
    }
  }

}
