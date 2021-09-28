import { Component, OnInit, Injector } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UserSubscriptionLevelsEnum, UserStatusEnum, ContinentEnum } from 'src/@core/models/enum/Enums';
import { BaseService } from 'src/@core/services/base.service';
import { ConfigurationsController } from 'src/@core/APIs/ConfigurationsController';
import { takeUntil, map } from 'rxjs/operators';
import { ConfigurationDto } from 'src/@core/models/configuration/Configuration';
import { Observable } from 'rxjs';
import { CountriesController } from 'src/@core/APIs/CountriesController';
import { RegionsController } from 'src/@core/APIs/RegionsController';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { LaboratoriesController } from 'src/@core/APIs/LaboratoriesController';
import { UserDto } from 'src/@core/models/security/User';
import { UsersController } from 'src/@core/APIs/UsersController';
import { UserCountrySubscriptionDto, UserRegionSubscriptionDto, UserLaboratorySubscriptionDto } from 'src/@core/models/security/UserSubscription';

@Component({
  selector: 'app-signup',
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.scss']
})
export class SignupComponent extends BaseService implements OnInit {

  isLinear = true;
  firstFormGroup: FormGroup;
  secondFormGroup: FormGroup;
  imageToUpload: File;
  submitted = false;
  returnUrl: string;
  hide = true;
  chide = true;
  configurationDto: ConfigurationDto = new ConfigurationDto();
  userDto: UserDto = new UserDto();
  // Drp
  userSubscriptionLevelsEnum = UserSubscriptionLevelsEnum;
  userStatusEnum = UserStatusEnum;
  continentEnum = ContinentEnum;
  countries$: Observable<any[]>;
  regions$: Observable<any[]>;
  laboratories$: Observable<any[]>;

  constructor(
    private formBuilder: FormBuilder,
    public injector: Injector
  ) {
    super(injector);
  }

  ngOnInit() {

    this.loadConfiguration();

    this.firstFormGroup = this.formBuilder.group({
      firstName: [null, Validators.compose([Validators.required])],
      lastName: [null, Validators.compose([Validators.required])],
      phoneNumber: [null, Validators.compose([Validators.required])],
      email: [null, Validators.compose([Validators.required, Validators.email])],
      jobTitle: [null, Validators.compose([Validators.required])],
      address: [null, Validators.compose([Validators.required])],
      password: [null, Validators.compose([Validators.required, Validators.minLength(3), Validators.maxLength(320)])],
    });

    this.secondFormGroup = this.formBuilder.group({
      userSubscriptionLevelId: [null, Validators.compose([Validators.required])],
      continentId: [null, Validators.compose([Validators.required])],
      countryId: [null, Validators.compose([Validators.required])],
      regionId: [null, Validators.compose([Validators.required])],
      laboratoryId: [null],
    });

    //#region On Value Changes
    this.secondFormGroup.controls['userSubscriptionLevelId'].valueChanges
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(userSubscriptionLevelId => {
        this.secondFormGroup.controls['continentId'].patchValue(null, { emitEvent: false });
        this.secondFormGroup.controls['countryId'].patchValue(null, { emitEvent: false });
        this.secondFormGroup.controls['regionId'].patchValue(null, { emitEvent: false });
        this.secondFormGroup.controls['laboratoryId'].patchValue(null, { emitEvent: false });
        this.countries$ = null;
        this.regions$ = null;
        this.laboratories$ = null;
        this.userDto.userCountrySubscriptionDtos = [];
        this.userDto.userRegionSubscriptionDtos = [];
        this.userDto.userLaboratorySubscriptionDtos = [];

        // Update validation
        const levelId = Number(userSubscriptionLevelId);
        if (levelId == this.userSubscriptionLevelsEnum.CountryLevel) {
          this.secondFormGroup.controls['continentId'].setValidators([Validators.required]);
          this.secondFormGroup.controls['countryId'].setValidators([Validators.required]);
          this.secondFormGroup.controls['regionId'].clearValidators();
          this.secondFormGroup.controls['laboratoryId'].clearValidators();
        } else if (levelId == this.userSubscriptionLevelsEnum.RegionLevel) {
          this.secondFormGroup.controls['continentId'].setValidators([Validators.required]);
          this.secondFormGroup.controls['countryId'].setValidators([Validators.required]);
          this.secondFormGroup.controls['regionId'].setValidators([Validators.required]);
          this.secondFormGroup.controls['laboratoryId'].clearValidators();
        } else if (levelId == this.userSubscriptionLevelsEnum.LaboratoryLevel) {
          this.secondFormGroup.controls['continentId'].setValidators([Validators.required]);
          this.secondFormGroup.controls['countryId'].setValidators([Validators.required]);
          this.secondFormGroup.controls['regionId'].setValidators([Validators.required]);
          // this.secondFormGroup.controls['laboratoryId'].setValidators([Validators.required]);
        } else if (levelId == this.userSubscriptionLevelsEnum.ViewOnlyLevel) {
          this.secondFormGroup.controls['continentId'].clearValidators();
          this.secondFormGroup.controls['countryId'].clearValidators();
          this.secondFormGroup.controls['regionId'].clearValidators();
          this.secondFormGroup.controls['laboratoryId'].clearValidators();
        }

        this.secondFormGroup.controls['continentId'].updateValueAndValidity();
        this.secondFormGroup.controls['countryId'].updateValueAndValidity();
        this.secondFormGroup.controls['regionId'].updateValueAndValidity();
        this.secondFormGroup.controls['laboratoryId'].updateValueAndValidity();
      });
    this.secondFormGroup.controls['continentId'].valueChanges
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(continentId => {
        if (continentId) this.loadCountries(Number(continentId));
        this.secondFormGroup.controls['countryId'].patchValue(null, { emitEvent: false });
        this.secondFormGroup.controls['regionId'].patchValue(null, { emitEvent: false });
        this.secondFormGroup.controls['laboratoryId'].patchValue(null, { emitEvent: false });
      });
    this.secondFormGroup.controls['countryId'].valueChanges
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(countryId => {
        const levelId = Number(this.secondFormGroup.getRawValue().userSubscriptionLevelId);
        if (countryId && (levelId == this.userSubscriptionLevelsEnum.RegionLevel || levelId == this.userSubscriptionLevelsEnum.LaboratoryLevel)) this.loadRegions(Number(countryId));
        this.secondFormGroup.controls['regionId'].patchValue(null, { emitEvent: false });
        this.secondFormGroup.controls['laboratoryId'].patchValue(null, { emitEvent: false });
      });
    this.secondFormGroup.controls['regionId'].valueChanges
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(regionId => {
        const levelId = Number(this.secondFormGroup.getRawValue().userSubscriptionLevelId);
        if (regionId && levelId == this.userSubscriptionLevelsEnum.LaboratoryLevel) this.loadLaboratories(Number(regionId));
        this.secondFormGroup.controls['laboratoryId'].patchValue(null, { emitEvent: false });
      });
    //#endregion

    // get return url from route parameters or default to '/'
    this.returnUrl = this.activatedRoute.snapshot.queryParams['returnUrl'] || '/';
  }

  loadCountries(continentId: number) {
    console.log(continentId);

    const url = CountriesController.GetAllAsDrp;
    let params: QueryParamsDto[] = [
      { key: 'continentId', value: continentId },
    ];
    this.countries$ = this.httpService.GET(url, params).pipe(map(res => res.data));
  }
  loadRegions(countryId: number) {
    console.log(countryId);
    
    const url = RegionsController.GetAllAsDrp;
    let params: QueryParamsDto[] = [
      { key: 'countryId', value: countryId },
    ];
    this.regions$ = this.httpService.GET(url, params).pipe(map(res => res.data));
  }
  loadLaboratories(regionId: number) {
    console.log(regionId);
    const url = LaboratoriesController.GetAllAsDrp;
    let params: QueryParamsDto[] = [
      { key: 'regionId', value: regionId },
    ];
    this.laboratories$ = this.httpService.GET(url, params).pipe(map(res => res.data));
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

  register() {
    const controls = this.firstFormGroup.controls;
    const controls2 = this.secondFormGroup.controls;
    /** check form */
    if (this.firstFormGroup.invalid) {
      Object.keys(controls).forEach(controlName =>
        controls[controlName].markAsTouched()
      );
      return;
    }
    if (this.secondFormGroup.invalid) {
      Object.keys(controls2).forEach(controlName =>
        controls2[controlName].markAsTouched()
      );
      return;
    }

    this.loading = true;

    // First Form
    this.userDto.firstName = this.firstFormGroup.getRawValue().firstName;
    this.userDto.lastName = this.firstFormGroup.getRawValue().lastName;
    this.userDto.phoneNumber = this.firstFormGroup.getRawValue().phoneNumber;
    this.userDto.email = this.firstFormGroup.getRawValue().email;
    this.userDto.jobTitle = this.firstFormGroup.getRawValue().jobTitle;
    this.userDto.callingCode = this.firstFormGroup.getRawValue().callingCode;
    this.userDto.address = this.firstFormGroup.getRawValue().address;
    this.userDto.password = this.firstFormGroup.getRawValue().password;
    this.userDto.status = 'Active';
    // Second Form
    this.userDto.userSubscriptionLevelId = Number(this.secondFormGroup.getRawValue().userSubscriptionLevelId);
    // User Countries
    if(this.userDto.userSubscriptionLevelId == this.userSubscriptionLevelsEnum.CountryLevel) {
      this.userDto.userRegionSubscriptionDtos = [];
      this.userDto.userLaboratorySubscriptionDtos = [];
      let values = this.secondFormGroup.getRawValue().countryId as string[];
      this.userDto.userCountrySubscriptionDtos = values.map(x => {
        let val = new UserCountrySubscriptionDto();
        val.countryId = Number(x);
        return val;
      })
    }
    // User Regions
    else if(this.userDto.userSubscriptionLevelId == this.userSubscriptionLevelsEnum.RegionLevel) {
      this.userDto.userCountrySubscriptionDtos = [];
      this.userDto.userLaboratorySubscriptionDtos = [];
      let values = this.secondFormGroup.getRawValue().regionId as string[];
      this.userDto.userRegionSubscriptionDtos = values.map(x => {
        let val = new UserRegionSubscriptionDto();
        val.regionId = Number(x);
        return val;
      })
    }
    // User Laboratories
    else if(this.userDto.userSubscriptionLevelId == this.userSubscriptionLevelsEnum.LaboratoryLevel) {
      this.userDto.regionId = Number(this.secondFormGroup.getRawValue().regionId);
      this.userDto.userCountrySubscriptionDtos = [];
      this.userDto.userRegionSubscriptionDtos = [];
      let values = this.secondFormGroup.getRawValue().laboratoryId as string[];
      this.userDto.userLaboratorySubscriptionDtos = values?.map(x => {
        let val = new UserLaboratorySubscriptionDto();
        val.laboratoryId = Number(x);
        return val;
      })
    }
    console.log(this.userDto);
    

    var formData: FormData = new FormData();
    if (this.imageToUpload) {
      formData.append('PersonalImagePath', this.imageToUpload, this.imageToUpload.name);
    }
    formData.append('userDto', JSON.stringify(this.userDto));


    this.httpService.POST(UsersController.Register, formData, null, true)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res.isPassed) {
          this.loading = false;
          this.alertService.success('You are registred successfully');
          this.submitted = true;
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

  previewImage(event, imageTag) {
    if (!event || !event.target.files[0]) {
      return;
    }
    if ((event.target.files[0].size / 1000) >= this.configurationDto.userPhotosize) {
      this.alertService.error(`Image size must be smaller than ${this.configurationDto.userPhotosize} KB`);
      return;
    } else {
      this.imageToUpload = event.target.files[0];
      var reader = new FileReader();
      reader.onloadend = () => {
        imageTag.src = reader.result;
      };
      reader.readAsDataURL(this.imageToUpload);
    }
  }

  removeImg(imageTag, file) {
    this.imageToUpload = null;
    imageTag.src = 'assets/images/user/usrbig3.jpg';
    file.value = '';
    this._ref.detectChanges();
  }

}
