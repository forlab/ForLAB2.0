import { Component, OnInit, Injector, ElementRef, ViewChild } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { ContactInfoDto } from 'src/@core/models/CMS/ContactInfo';
import { ContactInfosController } from 'src/@core/APIs/ContactInfosController';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { takeUntil } from 'rxjs/operators';
import { GooglePlaceDirective } from 'ngx-google-places-autocomplete';
import { RxwebValidators } from '@rxweb/reactive-form-validators';

@Component({
  selector: 'app-contact-info',
  templateUrl: './contact-info.component.html',
  styleUrls: ['./contact-info.component.scss']
})
export class ContactInfoComponent extends BaseService implements OnInit {

  form: FormGroup;
  contactInfoDto: ContactInfoDto = new ContactInfoDto();
  loadingContactInfo: boolean = false;
  // Google maps
  @ViewChild('placesRef', { static: true }) placesRef: GooglePlaceDirective;
  @ViewChild('search', { static: true }) public searchElement: ElementRef;
  lat: number = 52.3555;
  lng: number = 1.1743;

  constructor(private fb: FormBuilder, public injector: Injector) {
    super(injector);
  }

  ngOnInit() {

    this.form = this.fb.group({
      phone: [null, Validators.compose([Validators.required])],
      email: [null, Validators.compose([Validators.required, Validators.email])],
      address: [null, Validators.compose([Validators.required])],
      latitude: [null, Validators.compose([Validators.required, RxwebValidators.latitude()])],
      longitude: [null, Validators.compose([Validators.required, RxwebValidators.longitude()])],
      facebook: [null, Validators.compose([RxwebValidators.url()])],
      twitter: [null, Validators.compose([RxwebValidators.url()])],
      linkedIn: [null, Validators.compose([RxwebValidators.url()])],
    });

    this.loadContactInfo();
  }

  loadContactInfo() {
    this.loadingContactInfo = true;
    const url = ContactInfosController.GetContactInfoDetails;
    this.httpService.GET(url)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res.isPassed) {
          this.contactInfoDto = res.data;
          console.log(this.contactInfoDto);
          this.loadingContactInfo = false;
          this.setFormValue();
        } else {
          this.alertService.error(res.message);
          this.loading = false;
          this._ref.detectChanges();
        }
      });

  }

  setFormValue() {
    this.form.controls["phone"].patchValue(this.contactInfoDto.phone);
    this.form.controls["email"].patchValue(this.contactInfoDto.email);
    this.form.controls["address"].patchValue(this.contactInfoDto.address);
    this.form.controls["latitude"].patchValue(this.contactInfoDto.latitude);
    this.form.controls["longitude"].patchValue(this.contactInfoDto.longitude);
    this.form.controls["facebook"].patchValue(this.contactInfoDto.facebook);
    this.form.controls["twitter"].patchValue(this.contactInfoDto.twitter);
    this.form.controls["linkedIn"].patchValue(this.contactInfoDto.linkedIn);
    this.lng = Number(this.contactInfoDto.longitude);
    this.lat = Number(this.contactInfoDto.latitude);
    this._ref.detectChanges();
  }


  // Google Maps
  handleAddressChange(address: any) {
    this.lng = address.geometry.location.lng();
    this.lat = address.geometry.location.lat();
    this.form.controls['address'].patchValue(address.formatted_address);
    this.form.controls['latitude'].patchValue(this.lat);
    this.form.controls['longitude'].patchValue(this.lng);
  }
  placeMarker($event) {
    this.lng = $event.coords.lng;
    this.lat = $event.coords.lat;
    this.form.controls['latitude'].patchValue(this.lat);
    this.form.controls['longitude'].patchValue(this.lng);
  }



  updateContactInfo() {

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
    this.contactInfoDto.phone = controls.phone.value;
    this.contactInfoDto.email = controls.email.value;
    this.contactInfoDto.address = controls.address.value;
    this.contactInfoDto.latitude = String(controls.latitude.value);
    this.contactInfoDto.longitude = String(controls.longitude.value);
    this.contactInfoDto.facebook = controls.facebook.value;
    this.contactInfoDto.twitter = controls.twitter.value;
    this.contactInfoDto.linkedIn = controls.linkedIn.value;


    this.httpService.PUT(ContactInfosController.UpdateContactInfo, this.contactInfoDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res.isPassed) {
          this.loading = false;
          this.alertService.success('Contact Info is updated successfully');
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
