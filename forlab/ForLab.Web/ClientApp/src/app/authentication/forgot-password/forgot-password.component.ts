import { Component, OnInit, Injector } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BaseService } from 'src/@core/services/base.service';
import { AccountController } from 'src/@core/APIs/AccountController';
import { takeUntil } from 'rxjs/operators';
declare const $: any;

@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.scss']
})
export class ForgotPasswordComponent extends BaseService implements OnInit {

  form: FormGroup;
  submitted = false;
  returnUrl: string;

  constructor(
    private formBuilder: FormBuilder,
    public injector: Injector
  ) {
    super(injector);
  }

  ngOnInit() {

    this.form = this.formBuilder.group({
      email: [null, Validators.compose([Validators.required, Validators.email, Validators.minLength(3), Validators.maxLength(320)])],
    });

    // get return url from route parameters or default to '/'
    this.returnUrl = this.activatedRoute.snapshot.queryParams['returnUrl'] || '/';
    //    [Focus input] * /
    $('.input100').each(function () {
      $(this).on('blur', function () {
        if (
          $(this)
            .val()
            .trim() != ''
        ) {
          $(this).addClass('has-val');
        } else {
          $(this).removeClass('has-val');
        }
      });
    });

  }

  async onSubmit() {
    const controls = this.form.controls;
    /** check form */
    if (this.form.invalid) {
      Object.keys(controls).forEach(controlName =>
        controls[controlName].markAsTouched()
      );
      return;
    }
    this.loading = true;

    const location = await this.httpService.GetLocationInfo();
    const rest = {
      email: controls.email.value,
      locationDto: location
    };

    this.httpService.POST(AccountController.ForgetPassword, rest)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res.isPassed) {
          this.alertService.success('Email is sent successfully, please check your inbox');
          this.router.navigateByUrl('/authentication/signin');
          this._ref.detectChanges();
        } else {
          this.alertService.error(res.message);
          this._ref.detectChanges();
        }
      }, err => {
        this.alertService.exception();
        this.loading = false;
        this._ref.detectChanges();
      });
  }

}
