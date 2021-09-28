import { Component, OnInit, Injector, ViewChild, ElementRef } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { takeUntil } from 'rxjs/operators';
import { CountryDto } from 'src/@core/models/lookup/Country';
import { CountriesController } from 'src/@core/APIs/CountriesController';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { ContinentEnum, CountryPeriodEnum } from 'src/@core/models/enum/Enums';
import { GooglePlaceDirective } from 'ngx-google-places-autocomplete';
import { RxwebValidators } from '@rxweb/reactive-form-validators';

@Component({
  selector: 'app-add-edit-country',
  templateUrl: './add-edit-country.component.html',
  styleUrls: ['./add-edit-country.component.scss']
})
export class AddEditCountryComponent extends BaseService implements OnInit {

  form: FormGroup;
  loadingCountry: boolean = false;
  countryId: number;
  countryDto: CountryDto = new CountryDto();
  isUpdate: boolean = false;
  // Drp
  continentEnum = ContinentEnum;
  countryPeriodEnum = CountryPeriodEnum;
  // Google maps
  @ViewChild('placesRef', { static: true }) placesRef: GooglePlaceDirective;
  @ViewChild('search', { static: true }) public searchElement: ElementRef;
  lat: number = 52.3555;
  lng: number = 1.1743;

  constructor(private fb: FormBuilder, public injector: Injector) {
    super(injector);

    if (this.router.url.includes('update')) {
      this.isUpdate = true;
      this.activatedRoute.paramMap.subscribe(params => {
        this.countryId = Number(params.get('countryId'));
        this.loadDataById(this.countryId);
      });
    }

  }

  ngOnInit() {

    this.form = this.fb.group({
      name: [null, Validators.compose([Validators.required])],
      continentId: [null, Validators.compose([Validators.required])],
      countryPeriodId: [null, Validators.compose([Validators.required])],
      shortCode: [null, Validators.compose([Validators.required])],
      shortName: [null, Validators.compose([Validators.required])],
      nativeName: [null, Validators.compose([Validators.required])],
      currencyCode: [null, Validators.compose([Validators.required])],
      callingCode: [null, Validators.compose([Validators.required])],
      latitude: [null, Validators.compose([Validators.required, RxwebValidators.latitude()])],
      longitude: [null, Validators.compose([Validators.required, RxwebValidators.longitude()])],
      population: [null, Validators.compose([])],
    });

  }

  loadDataById(id: number) {
    this.loadingCountry = true;
    const url = CountriesController.GetCountryDetails;
    let params: QueryParamsDto[] = [
      { key: 'countryId', value: id },
    ];

    this.httpService.GET(url, params)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res.isPassed) {
          this.countryDto = res.data;
          this.loadingCountry = false;
          this.setFormValue();
        } else {
          this.alertService.error(res.message);
          this.loading = false;
          this._ref.detectChanges();
        }
      });
  }

  setFormValue() {
    this.form.controls["name"].patchValue(this.countryDto.name);
    this.form.controls["continentId"].patchValue(this.countryDto.continentId);
    this.form.controls["countryPeriodId"].patchValue(this.countryDto.countryPeriodId);
    this.form.controls["shortCode"].patchValue(this.countryDto.shortCode);
    this.form.controls["shortName"].patchValue(this.countryDto.shortName);
    this.form.controls["nativeName"].patchValue(this.countryDto.nativeName);
    this.form.controls["currencyCode"].patchValue(this.countryDto.currencyCode);
    this.form.controls["callingCode"].patchValue(this.countryDto.callingCode);
    this.form.controls["latitude"].patchValue(this.countryDto.latitude);
    this.form.controls["longitude"].patchValue(this.countryDto.longitude);
    this.form.controls["population"].patchValue(this.countryDto.population);
    this.lng = Number(this.countryDto.longitude);
    this.lat = Number(this.countryDto.latitude);
    this._ref.detectChanges();
  }


  // Google Maps
  handleAddressChange(address: any) {
    this.lng = address.geometry.location.lng();
    this.lat = address.geometry.location.lat();
    this.form.controls['name'].patchValue(address.name);
    this.form.controls['latitude'].patchValue(this.lat);
    this.form.controls['longitude'].patchValue(this.lng);
  }
  placeMarker($event) {
    this.lng = $event.coords.lng;
    this.lat = $event.coords.lat;
    this.form.controls['latitude'].patchValue(this.lat);
    this.form.controls['longitude'].patchValue(this.lng);
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
    this.countryDto.name = this.form.getRawValue().name;
    this.countryDto.continentId = Number(this.form.getRawValue().continentId);
    this.countryDto.countryPeriodId = Number(this.form.getRawValue().countryPeriodId);
    this.countryDto.shortCode = this.form.getRawValue().shortCode;
    this.countryDto.shortName = this.form.getRawValue().shortName;
    this.countryDto.nativeName = this.form.getRawValue().nativeName;
    this.countryDto.currencyCode = this.form.getRawValue().currencyCode;
    this.countryDto.callingCode = this.form.getRawValue().callingCode;
    this.countryDto.latitude = String(this.form.getRawValue().latitude);
    this.countryDto.longitude = String(this.form.getRawValue().longitude);
    this.countryDto.population = this.form.getRawValue().population || 0;

    if (this.isUpdate) {

      // call service
      this.httpService.PUT(CountriesController.UpdateCountry, this.countryDto)
        .pipe(takeUntil(this.ngUnsubscribe))
        .subscribe(res => {
          if (res.isPassed) {
            this.loading = false;
            this.alertService.success('Country is updated successfully');
            this.router.navigate(['/lookup/countries']);
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
      this.httpService.POST(CountriesController.CreateCountry, this.countryDto)
        .pipe(takeUntil(this.ngUnsubscribe))
        .subscribe(res => {
          if (res.isPassed) {
            this.loading = false;
            this.alertService.success('Country is created successfully');
            this.router.navigate(['/lookup/countries']);
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

}
