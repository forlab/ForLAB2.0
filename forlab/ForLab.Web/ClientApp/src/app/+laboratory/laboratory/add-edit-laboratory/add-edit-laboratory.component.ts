import { Component, OnInit, Injector, ViewChild, ElementRef } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { LaboratoryDto } from 'src/@core/models/lookup/Laboratory';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { LaboratoriesController } from 'src/@core/APIs/LaboratoriesController';
import { takeUntil, map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { LaboratoryCategoriesController } from 'src/@core/APIs/LaboratoryCategoriesController';
import { LaboratoryLevelsController } from 'src/@core/APIs/LaboratoryLevelsController';
import { GooglePlaceDirective } from 'ngx-google-places-autocomplete';
import { CountryDto } from 'src/@core/models/lookup/Country';
import { UserSubscriptionsController } from 'src/@core/APIs/UserSubscriptionsController';

@Component({
  selector: 'app-add-edit-laboratory',
  templateUrl: './add-edit-laboratory.component.html',
  styleUrls: ['./add-edit-laboratory.component.scss']
})
export class AddEditLaboratoryComponent extends BaseService implements OnInit {

  laboratoryId: number;
  LoadingLaboratory: boolean = false;
  isUpdate: boolean = false;
  laboratoryDto: LaboratoryDto = new LaboratoryDto();
  form: FormGroup;
  // Drp
  userCountries: CountryDto[];
  regions$: Observable<any[]>;
  laboratoryCategories$: Observable<any[]>;
  laboratoryLevels$: Observable<any[]>;
  // Google maps
  @ViewChild('placesRef', { static: true }) placesRef: GooglePlaceDirective;
  @ViewChild('search', { static: true }) public searchElement: ElementRef;
  lat: number = 52.3555;
  lng: number = 1.1743;

  constructor(
    private fb: FormBuilder,
    public injector: Injector) {
    super(injector);

    if (this.router.url.includes('update')) {
      this.isUpdate = true;
      this.activatedRoute.paramMap.subscribe(params => {
        this.laboratoryId = Number(params.get('laboratoryId'));
        this.loadDataById(this.laboratoryId);
      });
    }
  }

  ngOnInit() {

    // Load Drp
    this.loadUserCountries();
    this.loadLaboratoryCategories();
    this.loadLaboratoryLevels();

    this.form = this.fb.group({
      name: new FormControl(null, [Validators.required]),
      regionId: new FormControl(null, [Validators.required]),
      laboratoryCategoryId: new FormControl(null, [Validators.required]),
      laboratoryLevelId: new FormControl(null, [Validators.required]),
      latitude: new FormControl(null, [Validators.required]),
      longitude: new FormControl(null, [Validators.required]),
      // UI
      countryId: new FormControl(null, [Validators.required]),
    });

    this.form.controls['countryId'].valueChanges
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(countryId => {
        if (countryId) {
          this.loadRegions(countryId);
        }
      });

  }

  loadDataById(id: number) {
    this.LoadingLaboratory = true;
    const url = LaboratoriesController.GetLaboratoryDetails;
    let params: QueryParamsDto[] = [
      { key: 'laboratoryId', value: id },
    ];

    this.httpService.GET(url, params)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res.isPassed) {
          this.laboratoryDto = res.data;
          this.LoadingLaboratory = false;
          this.setFormValue();
        } else {
          this.alertService.error(res.message);
          this.loading = false;
          this._ref.detectChanges();
        }
      });

  }

  loadUserCountries() {
    let params: QueryParamsDto[] = [
      { key: 'applicationUserId', value: this.loggedInUser.id },
    ];
    this.httpService.GET(UserSubscriptionsController.GetUserCountriesAsDrp, params)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.userCountries = res.data;
            if (this.userCountries && this.userCountries.length == 1) {
              this.form.controls['countryId'].patchValue(this.userCountries[0].id);
            }
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

  loadRegions(countryId: number) {
    let params: QueryParamsDto[] = [
      { key: 'applicationUserId', value: this.loggedInUser.id },
      { key: 'countryId', value: countryId },
    ];
    this.regions$ = this.httpService.GET(UserSubscriptionsController.GetUserRegionsAsDrp, params).pipe(map(res => res.data));
  }

  loadLaboratoryCategories() {
    this.laboratoryCategories$ = this.httpService.GET(LaboratoryCategoriesController.GetAllAsDrp).pipe(map(res => res.data));
  }
  loadLaboratoryLevels() {
    this.laboratoryLevels$ = this.httpService.GET(LaboratoryLevelsController.GetAllAsDrp).pipe(map(res => res.data));
  }

  // Google Maps
  handleAddressChange(address: any) {
    this.lng = address.geometry.location.lng();
    this.lat = address.geometry.location.lat();
    this.form.controls['latitude'].patchValue(this.lat);
    this.form.controls['longitude'].patchValue(this.lng);
  }
  placeMarker($event) {
    this.lng = $event.coords.lng;
    this.lat = $event.coords.lat;
    this.form.controls['latitude'].patchValue(this.lat);
    this.form.controls['longitude'].patchValue(this.lng);
  }

  setFormValue() {
    this.form.controls['name'].patchValue(this.laboratoryDto.name);
    this.form.controls['regionId'].patchValue(this.laboratoryDto.regionId);
    this.form.controls['laboratoryCategoryId'].patchValue(this.laboratoryDto.laboratoryCategoryId);
    this.form.controls['laboratoryLevelId'].patchValue(this.laboratoryDto.laboratoryLevelId);
    this.form.controls['latitude'].patchValue(this.laboratoryDto.latitude);
    this.form.controls['longitude'].patchValue(this.laboratoryDto.longitude);
    this.form.controls['countryId'].patchValue(this.laboratoryDto.regionCountryId);
    this.lng = Number(this.laboratoryDto.longitude);
    this.lat = Number(this.laboratoryDto.latitude);
    this._ref.detectChanges();
  }

  createObject() {
    this.httpService.POST(LaboratoriesController.CreateLaboratory, this.laboratoryDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.alertService.success('Laboratory is created successfully');
            this.router.navigate(['/laboratory/laboratories']);
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
    this.httpService.PUT(LaboratoriesController.UpdateLaboratory, this.laboratoryDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.alertService.success('Laboratory is updated successfully');
            this.router.navigate(['/laboratory/laboratories']);
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

    // Set the data
    this.laboratoryDto.name = this.form.getRawValue().name;
    this.laboratoryDto.regionId = Number(this.form.getRawValue().regionId);
    this.laboratoryDto.laboratoryCategoryId = Number(this.form.getRawValue().laboratoryCategoryId);
    this.laboratoryDto.laboratoryLevelId = Number(this.form.getRawValue().laboratoryLevelId);
    this.laboratoryDto.latitude = String(this.form.getRawValue().latitude);
    this.laboratoryDto.longitude = String(this.form.getRawValue().longitude);

    if (!this.isUpdate) {
      this.createObject();
    } else {
      this.updateObject();
    }
  }

}
