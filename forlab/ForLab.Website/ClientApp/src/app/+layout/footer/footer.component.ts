import { Component, OnInit, Injector } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { ContactInfoDto } from 'src/@core/models/CMS/ContactInfo';
import { ContactInfosController } from 'src/@core/APIs/ContactInfosController';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-footer',
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.scss']
})
export class FooterComponent extends BaseService implements OnInit {

  contactInfoDto: ContactInfoDto = new ContactInfoDto();
  
  constructor(public injector: Injector) {
    super(injector);
  }

  ngOnInit(): void {
    this.loadContactInfo();
  }

  navigate(url: string) {
    this.router.navigate([url])
  }

  loadContactInfo() {
    const url = ContactInfosController.GetContactInfoDetails;
    this.httpService.GET(url)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res.isPassed) {
          this.contactInfoDto = res.data;
        } else {
          // this.alertService.error(res.message);
          this.loading = false;
          this._ref.detectChanges();
        }
      });
  }

}
