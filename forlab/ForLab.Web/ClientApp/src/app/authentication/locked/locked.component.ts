import { Component, OnInit, Injector } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BaseService } from 'src/@core/services/base.service';
import { AccountController } from 'src/@core/APIs/AccountController';
import { takeUntil } from 'rxjs/operators';
declare const $: any;

@Component({
  selector: 'app-locked',
  templateUrl: './locked.component.html',
  styleUrls: ['./locked.component.scss']
})
export class LockedComponent extends BaseService implements OnInit {

  form: FormGroup;
  submitted = false;
  returnUrl: string;
  hide = true;

  constructor(
    private formBuilder: FormBuilder,
    public injector: Injector
  ) {
    super(injector);
  }

  ngOnInit() {
    this.form = this.formBuilder.group({
      password: [null, Validators.compose([Validators.required, Validators.minLength(3), Validators.maxLength(320)])],
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

  get f() {
    return this.form.controls;
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

    const authData = {
      email: this.loggedInUser.email,
      password: controls.password.value,
      locationDto: location
    };

    this.httpService.POST(AccountController.Login, authData)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res.isPassed) {

          this.loading = false;

          // check if the user need to change his password
          if (res.data.changePassword) {
            this.router.navigate([`/authentication/reset-password`], { queryParams: { email: res.data.email, token: res.data.token } });
          } else {

            if (res.data && res.data.userRoles) {
              this.permissionsService.loadPermissions(res.data.userRoles);
            }

            // sete info in local storage
            this.authService.storeUserDate(res.data);
            this.alertService.success('You have logged in successfully');
            this.router.navigate([`/dashboard/main`]);
          }
        } else {
          this.alertService.error(res.message);
          this.loading = false;
          this._ref.detectChanges();
          if (res.message == 'Your password is expired') {
            this.router.navigate([`/authentication/change-password/${controls.email.value}`]);
          }
        }
      }, err => {
        this.alertService.exception();
        this.loading = false;
        this._ref.detectChanges();
      });
  }
}
