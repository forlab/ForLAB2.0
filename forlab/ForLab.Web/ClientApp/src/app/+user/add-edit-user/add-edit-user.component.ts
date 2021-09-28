import { Component, OnInit, Injector } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { takeUntil } from 'rxjs/operators';
import { UserDto } from 'src/@core/models/security/User';
import { UsersController } from 'src/@core/APIs/UsersController';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { UserSubscriptionLevelsEnum, UserStatusEnum } from 'src/@core/models/enum/Enums';
import { ConfigurationDto } from 'src/@core/models/configuration/Configuration';
import { ConfigurationsController } from 'src/@core/APIs/ConfigurationsController';
import { RxwebValidators } from '@rxweb/reactive-form-validators';

@Component({
  selector: 'app-add-edit-user',
  templateUrl: './add-edit-user.component.html',
  styleUrls: ['./add-edit-user.component.scss']
})
export class AddEditUserComponent extends BaseService implements OnInit {

  form: FormGroup;
  loadingUser: boolean = false;
  userId: number;
  configurationDto: ConfigurationDto = new ConfigurationDto();
  userDto: UserDto = new UserDto();
  isUpdate: boolean = false;
  isSuperAdmin: boolean = false;
  imageToUpload: File;
  // Drp
  userSubscriptionLevelsEnum = UserSubscriptionLevelsEnum;
  userStatusEnum = UserStatusEnum;

  constructor(private fb: FormBuilder, public injector: Injector) {
    super(injector);

    if (this.router.url.includes('update')) {
      this.isUpdate = true;
      this.activatedRoute.paramMap.subscribe(params => {
        this.userId = Number(params.get('userId'));
        this.loadDataById(this.userId);
      });
    }
  }

  ngOnInit() {

    this.loadConfiguration();

    this.form = this.fb.group({
      firstName: [null, Validators.compose([Validators.required])],
      lastName: [null, Validators.compose([Validators.required])],
      phoneNumber: [null, Validators.compose([Validators.required, RxwebValidators.numeric()])],
      email: [null, Validators.compose([Validators.required, Validators.email])],
      jobTitle: [null, Validators.compose([Validators.required])],
      address: [null, Validators.compose([Validators.required])],
      userSubscriptionLevelId: [null, Validators.compose([Validators.required])],
    });

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
  
  loadDataById(id: number) {
    this.loadingUser = true;
    const url = UsersController.GetUserDetails;
    let params: QueryParamsDto[] = [
      { key: 'userId', value: id },
    ];

    this.httpService.GET(url, params)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res.isPassed) {
          this.userDto = res.data;
          console.log(this.userDto);
          
          this.loadingUser = false;
          this.setFormValue();
        } else {
          this.alertService.error(res.message);
          this.loading = false;
          this._ref.detectChanges();
        }
      });

  }

  setFormValue() {
    this.form.controls["firstName"].patchValue(this.userDto.firstName);
    this.form.controls["lastName"].patchValue(this.userDto.lastName);
    this.form.controls["phoneNumber"].patchValue(this.userDto.phoneNumber);
    this.form.controls["email"].patchValue(this.userDto.email);
    this.form.controls["jobTitle"].patchValue(this.userDto.jobTitle);
    this.form.controls["address"].patchValue(this.userDto.address);
    this.form.controls["userSubscriptionLevelId"].patchValue(this.userDto.userSubscriptionLevelId);
    this.form.controls["userSubscriptionLevelId"].disable();

    if (this.userDto.userSubscriptionLevelId == null) {
      this.isSuperAdmin = true;
    }

    this._ref.detectChanges();
  }


  submitForm() {
    const controls = this.form.controls;
    /** check form */
    if (this.form.invalid) {
      Object.keys(controls).forEach(controlName =>
        controls[controlName].markAsTouched()
      );
      return;
    }

    this.loading = true;

    // Set the values
    this.userDto.firstName = this.form.getRawValue().firstName;
    this.userDto.lastName = this.form.getRawValue().lastName;
    this.userDto.phoneNumber = this.form.getRawValue().phoneNumber;
    this.userDto.email = this.form.getRawValue().email;
    this.userDto.jobTitle = this.form.getRawValue().jobTitle;
    this.userDto.callingCode = this.form.getRawValue().callingCode;
    this.userDto.address = this.form.getRawValue().address;
    this.userDto.userSubscriptionLevelId = Number(this.form.getRawValue().userSubscriptionLevelId);
    this.userDto.status = !this.isUpdate ? 'Active' : this.userDto.status;

    var formData: FormData = new FormData();
    if (this.imageToUpload) {
      formData.append('PersonalImagePath', this.imageToUpload, this.imageToUpload.name);
    }
    formData.append('userDto', JSON.stringify(this.userDto));


    if (this.isUpdate) {

      // call service
      this.httpService.PUT(UsersController.UpdateUser, formData, null, true)
        .pipe(takeUntil(this.ngUnsubscribe))
        .subscribe(res => {
          if (res.isPassed) {
            this.loading = false;
            this.alertService.success('User is updated successfully');
            this.router.navigate(['/user/users']);
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

    } else {

      // call service
      this.httpService.POST(UsersController.CreateUser, formData, null, true)
        .pipe(takeUntil(this.ngUnsubscribe))
        .subscribe(res => {
          if (res.isPassed) {
            this.loading = false;
            this.alertService.success('User is created successfully');
            this.router.navigate(['/user/users']);
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

  previewImage(event, imageTag) {
    if(!event || !event.target.files[0]) {
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
