import { Component, Injector, OnInit } from '@angular/core';
import { takeUntil } from 'rxjs/operators';
import { AccountController } from 'src/@core/APIs/AccountController';
import { BaseService } from 'src/@core/services/base.service';

@Component({
  selector: 'app-verify-email',
  templateUrl: './verify-email.component.html',
  styleUrls: ['./verify-email.component.scss']
})
export class VerifyEmailComponent extends BaseService implements OnInit {

  email: string;
  token: string;
  verifed = false;

  constructor(public injector: Injector) {
    super(injector);

    this.activatedRoute.queryParams
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(params => {
        if (params && params['token'] && params['email']) {
          this.token = params['token'];
          this.email = params['email'];
          this.confirmEmail();
        }

      });

  }

  ngOnInit() {
  }

  async confirmEmail() {

    this.loading = true;

    let location = await this.httpService.GetLocationInfo();
    const authData = {
      email: this.email,
      token: this.token,
      locationDto: location,
    };

    this.httpService.POST(AccountController.ConfirmEmailAddress, authData)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res.isPassed) {
          this.loading = false;
          this.alertService.success(`${this.email} has been verified successful. Please log in to your account`);
          this.verifed = true;
        } else {
          this.alertService.error(res.message);
          this.loading = false;
          this.verifed = false;
          this._ref.detectChanges();
        }
      }, err => {
        this.alertService.exception();
        this.loading = false;
        this.verifed = false;
        this._ref.detectChanges();
      });

  }

}
