import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { VendorDto } from 'src/@core/models/vendor/Vendor';
import { VendorsController } from 'src/@core/APIs/VendorsController';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { takeUntil } from 'rxjs/operators';
import { VendorContactsComponent } from '../../vendor-contact/vendor-contacts/vendor-contacts.component';

@Component({
  selector: 'app-vendor-details',
  templateUrl: './vendor-details.component.html',
  styleUrls: ['./vendor-details.component.sass']
})
export class VendorDetailsComponent extends BaseService implements OnInit {

  // Children
  @ViewChild(VendorContactsComponent, { static: false }) vendorContactsComponent: VendorContactsComponent;

  vendorId: number;
  vendorDto: VendorDto = new VendorDto();
  loadingVendor = false;

  constructor(public injector: Injector) {
    super(injector);

    if (this.router.url.includes('details')) {
      this.activatedRoute.paramMap.subscribe(params => {
        this.vendorId = Number(params.get('vendorId'));
        this.loadDataById(this.vendorId);
      });
    }

  }

  ngOnInit(): void {
  }

  loadDataById(id: number) {
    this.loadingVendor = true;
    const url = VendorsController.GetVendorDetails;
    let params: QueryParamsDto[] = [
      { key: 'vendorId', value: id },
    ];

    this.httpService.GET(url, params)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res.isPassed) {
          this.vendorDto = res.data;
          this.loadingVendor = false;

          if (this.vendorDto.createdBy != this.loggedInUser?.id) {
            this.vendorContactsComponent.columns = this.vendorContactsComponent?.columns.filter(x => x.property != 'select' && x.property != 'actions');
          }

        } else {
          this.alertService.error(res.message);
          this.loading = false;
          this._ref.detectChanges();
        }
      });
  }

}
