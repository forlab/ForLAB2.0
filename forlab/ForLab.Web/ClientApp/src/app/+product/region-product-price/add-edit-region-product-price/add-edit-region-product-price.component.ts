import { Component, OnInit, Inject, Injector } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { RegionProductPriceDto } from 'src/@core/models/product/RegionProductPrice';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { RegionProductPricesController } from 'src/@core/APIs/RegionProductPricesController';
import { takeUntil, map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { CountriesController } from 'src/@core/APIs/CountriesController';
import { ProductsController } from 'src/@core/APIs/ProductsController';
import * as moment from 'moment';
import { RegionsController } from 'src/@core/APIs/RegionsController';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { UserSubscriptionsController } from 'src/@core/APIs/UserSubscriptionsController';
class Params {
  productId: number;
  fromProductDetails: boolean = false;
  regionProductPriceDto: RegionProductPriceDto;
}

@Component({
  selector: 'app-add-edit-region-product-price',
  templateUrl: './add-edit-region-product-price.component.html',
  styleUrls: ['./add-edit-region-product-price.component.sass']
})
export class AddEditRegionProductPriceComponent extends BaseService implements OnInit {

  regionProductPriceDto: RegionProductPriceDto = new RegionProductPriceDto();
  form: FormGroup;
  mode: 'create' | 'update' = 'create';
  // Drp
  countries$: Observable<any[]>;
  regions$: Observable<any[]>;
  products$: Observable<any[]>;

  constructor(@Inject(MAT_DIALOG_DATA) public data: Params,
    public dialogRef: MatDialogRef<AddEditRegionProductPriceComponent>,
    private fb: FormBuilder,
    public injector: Injector) {
    super(injector);
  }

  ngOnInit() {

    // Load Drp
    this.loadCountries();
    this.loadProducts();

    this.form = this.fb.group({
      productId: new FormControl(null, [Validators.required]),
      regionId: new FormControl(null, [Validators.required]),
      price: new FormControl(null, [Validators.required]),
      packSize: new FormControl(null, [Validators.required]),
      fromDate: new FormControl(null, [Validators.required]),

      // UI
      countryId: new FormControl(null, []),
    });

    this.form.controls['countryId'].valueChanges
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(countryId => {
        if (countryId) {
          this.loadRegions(countryId);
        }
      });

    if(this.data.fromProductDetails) {
      this.form.controls['productId'].patchValue(this.data.productId);
      this.form.controls['productId'].disable();
    }

    if (this.data && this.data.regionProductPriceDto && this.data.regionProductPriceDto.id) {
      this.mode = 'update';
      this.setFormData();
    } else {
      this.regionProductPriceDto = this.data.regionProductPriceDto || new RegionProductPriceDto();
    }

  }

  loadCountries() {
    this.countries$ = this.httpService.GET(CountriesController.GetAllAsDrp).pipe(map(res => res.data));
  }
  loadRegions(countryId: number) {
    let params: QueryParamsDto[] = [
      { key: 'applicationUserId', value: this.loggedInUser.id },
      { key: 'countryId', value: countryId },
    ];
    this.regions$ = this.httpService.GET(UserSubscriptionsController.GetUserRegionsAsDrp, params).pipe(map(res => res.data));
  }
  loadProducts() {
    this.products$ = this.httpService.GET(ProductsController.GetAllAsDrp).pipe(map(res => res.data));
  }

  setFormData() {
    this.regionProductPriceDto = JSON.parse(JSON.stringify(this.data.regionProductPriceDto));
    this.form.controls['productId'].patchValue(this.regionProductPriceDto.productId);
    this.form.controls['regionId'].patchValue(this.regionProductPriceDto.regionId);
    this.form.controls['price'].patchValue(this.regionProductPriceDto.price);
    this.form.controls['packSize'].patchValue(this.regionProductPriceDto.packSize);
    this.form.controls['fromDate'].patchValue(this.regionProductPriceDto.fromDate);
    // UI
    this.form.controls['countryId'].patchValue(this.regionProductPriceDto.regionCountryId);
    this._ref.detectChanges();
  }

  createObject() {
    this.httpService.POST(RegionProductPricesController.CreateRegionProductPrice, this.regionProductPriceDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.alertService.success('Price is created successfully');
            this.dialogRef.close(this.regionProductPriceDto);
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
    this.httpService.PUT(RegionProductPricesController.UpdateRegionProductPrice, this.regionProductPriceDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.dialogRef.close(this.regionProductPriceDto);
            this.alertService.success('Price is updated successfully');
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
    this.regionProductPriceDto.productId = Number(this.form.getRawValue().productId);
    this.regionProductPriceDto.regionId = Number(this.form.getRawValue().regionId);
    this.regionProductPriceDto.price = this.form.getRawValue().price;
    this.regionProductPriceDto.packSize = this.form.getRawValue().packSize;
    this.regionProductPriceDto.fromDate = moment(this.form.getRawValue().fromDate).add(1, 'd').toISOString();

    if (this.mode === 'create') {
      this.createObject();
    } else if (this.mode === 'update') {
      this.updateObject();
    }
  }


  isCreateMode() {
    return this.mode === 'create';
  }

  isUpdateMode() {
    return this.mode === 'update';
  }
}
