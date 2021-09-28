import { Component, OnInit, Injector } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { AccountController } from 'src/@core/APIs/AccountController';
import { takeUntil } from 'rxjs/operators';
import { ResponseDto, QueryParamsDto } from 'src/@core/models/common/response';
import { Validators, FormGroup, FormBuilder } from '@angular/forms';
import { UserDto } from 'src/@core/models/security/User';
import { ConfigurationsController } from 'src/@core/APIs/ConfigurationsController';
import { ConfigurationDto } from 'src/@core/models/configuration/Configuration';
import { RxwebValidators } from '@rxweb/reactive-form-validators';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent extends BaseService implements OnInit {

  userDto: UserDto = new UserDto();
  imageToUpload: File;
  userId: number;
  configurationDto: ConfigurationDto = new ConfigurationDto();
  loadingUser = false;
  changePassForm: FormGroup;
  infoForm: FormGroup;
  hide = true;

  constructor(public injector: Injector, private fb: FormBuilder) {
    super(injector);
  }

  ngOnInit() {

    this.loadConfiguration();

    const user = JSON.parse(localStorage.getItem('lab_user'));
    if (user != null) {
      this.getLogedInUserData(user.id);
      this.userId = user.id;
    }

    this.changePassForm = this.fb.group({
      currentPassword: [null, Validators.compose([Validators.required, Validators.minLength(3), Validators.maxLength(100)])],
      newPassword: [null, Validators.compose([Validators.required, Validators.minLength(3), Validators.maxLength(100)])],
      confirmNewPassword: [null, Validators.compose([Validators.required, RxwebValidators.compare({fieldName:'newPassword'})])]
    });
    this.infoForm = this.fb.group({
      firstName: [null, Validators.compose([Validators.required])],
      lastName: [null, Validators.compose([Validators.required])],
      phoneNumber: [null, Validators.compose([Validators.required, RxwebValidators.numeric()])],
      email: [null, Validators.compose([Validators.required, Validators.email])],
      jobTitle: [null, Validators.compose([Validators.required])],
      address: [null, Validators.compose([Validators.required])],
    });

  }

  getLogedInUserData(userid: number) {

    let params: QueryParamsDto[] = [
      { key: 'userId', value: userid }
    ];

    this.loadingUser = true;

    this.httpService.GET(AccountController.GetLoggedInUserProfile, params)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe((res: ResponseDto) => {
        if (res.isPassed) {
          this.userDto = res.data;
          this.setFormValue();
          this.loadingUser = false;
        } else {
          this.loadingUser = false;
          this.alertService.error(res.message);
        }
      }
      );
  }

  setFormValue() {
    this.infoForm.controls["firstName"].patchValue(this.userDto.firstName);
    this.infoForm.controls["lastName"].patchValue(this.userDto.lastName);
    this.infoForm.controls["phoneNumber"].patchValue(this.userDto.phoneNumber);
    this.infoForm.controls["email"].patchValue(this.userDto.email);
    this.infoForm.controls["jobTitle"].patchValue(this.userDto.jobTitle);
    this.infoForm.controls["address"].patchValue(this.userDto.address);
    this._ref.detectChanges();
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

  updateUser() {

    const controls = this.infoForm.controls;
    /** check changePassForm */
    if (this.infoForm.invalid) {
      Object.keys(controls).forEach(controlName =>
        controls[controlName].markAsTouched()
      );
      return;
    }

    this.loading = true;

    // Set the values
    this.userDto.firstName = this.infoForm.getRawValue().firstName;
    this.userDto.lastName = this.infoForm.getRawValue().lastName;
    this.userDto.phoneNumber = this.infoForm.getRawValue().phoneNumber;
    this.userDto.email = this.infoForm.getRawValue().email;
    this.userDto.jobTitle = this.infoForm.getRawValue().jobTitle;
    this.userDto.callingCode = this.infoForm.getRawValue().callingCode;
    this.userDto.address = this.infoForm.getRawValue().address;

    var formData: FormData = new FormData();
    if (this.imageToUpload) {
      formData.append('PersonalImagePath', this.imageToUpload, this.imageToUpload.name);
    }
    formData.append('userDto', JSON.stringify(this.userDto));

    this.httpService.PUT(AccountController.UpdateUserProfile, formData, null, true)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res.isPassed) {
          this.loading = false;
          this.userDto.personalImagePath = res.data.personalImagePath;
          this.authService.updateStoredUserInfo(res.data);
          this.alertService.success('Profile is updated successfully');
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


  async changePassword() {

    const controls = this.changePassForm.controls;
    /** check changePassForm */
    if (this.changePassForm.invalid) {
      Object.keys(controls).forEach(controlName =>
        controls[controlName].markAsTouched()
      );
      return;
    }

    if (controls.newPassword.value != controls.confirmNewPassword.value) {
      this.alertService.error('Your passwords did not match');
      return;
    }

    this.loading = true;

    const location = await this.httpService.GetLocationInfo();

    const ChangePasswordData = {
      currentPassword: controls.currentPassword.value,
      newPassword: controls.newPassword.value,
      userId: this.userDto.id,
      email: this.userDto.email,
      locationDto: location
    };

    this.httpService.PUT(AccountController.ChangePassword, ChangePasswordData)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe((res: ResponseDto) => {
        if (res.isPassed) {
          this.loading = false;
          this.alertService.success("Password has been changed successfully");
          this.authService.logout();
        } else {
          this.loading = false;
          this.alertService.error(res.message);
          this._ref.detectChanges();
        }
      }, err => {
        this.alertService.exception();
        this.loading = false;
        this._ref.detectChanges();
      });

  }

  formatUserRoles(roles: string[]): string {
    return roles.join(', ');
  }

  previewImage(event, imageTag) {
    if (!event || !event.target.files[0]) {
      return;
    }
    if ((event.target.files[0].size / 1000) >= this.configurationDto.userPhotosize) {
      this.alertService.error(`Image size must be smaller than ${this.configurationDto.userPhotosize} KB`);
      return;
    } else {
      this.imageToUpload = event.target.files[0];
      this.userDto.reomveProfileImage = false;
      var reader = new FileReader();
      reader.onloadend = () => {
        imageTag.src = reader.result;
      };
      reader.readAsDataURL(this.imageToUpload);
    }
  }

  removeImg(imageTag, file) {
    this.userDto.personalImagePath = null;
    this.userDto.reomveProfileImage = true;
    this.imageToUpload = null;
    imageTag.src = 'assets/images/user/usrbig3.jpg';
    file.value = '';
    this._ref.detectChanges();
  }
}
