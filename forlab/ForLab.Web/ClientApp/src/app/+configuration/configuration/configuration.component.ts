import { Component, OnInit, Injector } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { ConfigurationDto } from 'src/@core/models/configuration/Configuration';
import { ConfigurationsController } from 'src/@core/APIs/ConfigurationsController';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-configuration',
  templateUrl: './configuration.component.html',
  styleUrls: ['./configuration.component.scss']
})
export class ConfigurationComponent extends BaseService implements OnInit {

  form: FormGroup;
  configurationDto: ConfigurationDto = new ConfigurationDto();
  loadingConfiguration: boolean = false;

  constructor(private fb: FormBuilder, public injector: Injector) {
    super(injector);
  }

  ngOnInit() {

    this.form = this.fb.group({
      numOfDaysToChangePassword: [null, Validators.compose([Validators.required])],
      accountLoginAttempts: [null, Validators.compose([Validators.required])],
      userPhotosize: [null, Validators.compose([Validators.required])],
      passwordExpiryTime: [null, Validators.compose([Validators.required])],
      timeToSessionTimeOut: [null, Validators.compose([Validators.required])],
      attachmentsMaxSize: [null, Validators.compose([Validators.required])],
      timesCountBeforePasswordReuse: [null, Validators.compose([Validators.required])],
    });

    this.loadConfiguration();
  }

  loadConfiguration() {
    this.loadingConfiguration = true;
    const url = ConfigurationsController.GetConfigurationDetails;
    this.httpService.GET(url)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res.isPassed) {
          this.configurationDto = res.data;
          console.log(this.configurationDto);
          this.loadingConfiguration = false;
          this.setFormValue();
        } else {
          this.alertService.error(res.message);
          this.loading = false;
          this._ref.detectChanges();
        }
      });

  }

  setFormValue() {
    this.form.controls["numOfDaysToChangePassword"].patchValue(this.configurationDto.numOfDaysToChangePassword);
    this.form.controls["accountLoginAttempts"].patchValue(this.configurationDto.accountLoginAttempts);
    this.form.controls["userPhotosize"].patchValue(this.configurationDto.userPhotosize);
    this.form.controls["passwordExpiryTime"].patchValue(this.configurationDto.passwordExpiryTime);
    this.form.controls["timeToSessionTimeOut"].patchValue(this.configurationDto.timeToSessionTimeOut);
    this.form.controls["attachmentsMaxSize"].patchValue(this.configurationDto.attachmentsMaxSize);
    this.form.controls["timesCountBeforePasswordReuse"].patchValue(this.configurationDto.timesCountBeforePasswordReuse);
    this._ref.detectChanges();
  }


  updateConfiguration() {

    const controls = this.form.controls;
    /** check form */
    if (this.form.invalid) {
      Object.keys(controls).forEach(controlName =>
        controls[controlName].markAsTouched()
      );
      return;
    }
    this.loading = true;

    // inialize
    this.configurationDto.numOfDaysToChangePassword = controls.numOfDaysToChangePassword.value;
    this.configurationDto.accountLoginAttempts = controls.accountLoginAttempts.value;
    this.configurationDto.userPhotosize = controls.userPhotosize.value;
    this.configurationDto.passwordExpiryTime = controls.passwordExpiryTime.value;
    this.configurationDto.timeToSessionTimeOut = controls.timeToSessionTimeOut.value;
    this.configurationDto.attachmentsMaxSize = controls.attachmentsMaxSize.value;
    this.configurationDto.timesCountBeforePasswordReuse = controls.timesCountBeforePasswordReuse.value;


    this.httpService.PUT(ConfigurationsController.UpdateConfiguration, this.configurationDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res.isPassed) {
          this.loading = false;
          this.alertService.success('Configuration is updated successfully');
          this._ref.detectChanges();
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

}
