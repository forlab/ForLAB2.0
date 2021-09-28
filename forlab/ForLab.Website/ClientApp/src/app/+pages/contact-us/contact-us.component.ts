import { Component, OnInit, Injector } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { InquiryQuestionDto } from 'src/@core/models/CMS/InquiryQuestion';
import { ContactInfoDto } from 'src/@core/models/CMS/ContactInfo';
import { InquiryQuestionsController } from 'src/@core/APIs/InquiryQuestionsController';
import { ContactInfosController } from 'src/@core/APIs/ContactInfosController';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-contact-us',
  templateUrl: './contact-us.component.html',
  styleUrls: ['./contact-us.component.scss']
})
export class ContactUsComponent extends BaseService implements OnInit {

  form: FormGroup;
  inquiryQuestionDto = new InquiryQuestionDto();
  contactInfoDto: ContactInfoDto = new ContactInfoDto();
  // Google maps
  lat: number = 52.3555;
  lng: number = 1.1743;

  constructor(public injector: Injector, private fb: FormBuilder) {
    super(injector);
  }

  ngOnInit(): void {
    this.loadContactInfo();

    this.form = this.fb.group({
      name: new FormControl(null, [Validators.required]),
      email: new FormControl(null, [Validators.required, Validators.email]),
      message: new FormControl(null, [Validators.required]),
    });
  }

  loadContactInfo() {
    const url = ContactInfosController.GetContactInfoDetails;
    this.httpService.GET(url)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res.isPassed) {
          this.contactInfoDto = res.data;
          this.lat = Number(this.contactInfoDto.latitude);
          this.lng = Number(this.contactInfoDto.longitude);
        } else {
          this.alertService.error(res.message);
          this.loading = false;
          this._ref.detectChanges();
        }
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
    this.inquiryQuestionDto.name = this.form.getRawValue().name;
    this.inquiryQuestionDto.email = this.form.getRawValue().email;
    this.inquiryQuestionDto.message = this.form.getRawValue().message;

    this.httpService.POST(InquiryQuestionsController.CreateInquiryQuestion, this.inquiryQuestionDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.alertService.success('Your message is sent successfully');
            this.form.reset();
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
