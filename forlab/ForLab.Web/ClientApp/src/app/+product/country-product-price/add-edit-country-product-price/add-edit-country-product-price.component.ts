import { Component, OnInit, Inject, Injector } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { CountryProductPriceDto } from 'src/@core/models/product/CountryProductPrice';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { CountryProductPricesController } from 'src/@core/APIs/CountryProductPricesController';
import { takeUntil, map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { CountriesController } from 'src/@core/APIs/CountriesController';
import { ProductsController } from 'src/@core/APIs/ProductsController';
import * as moment from 'moment';
class Params {
  productId: number;
  fromProductDetails: boolean = false;
  countryProductPriceDto: CountryProductPriceDto;
}

@Component({
  selector: 'app-add-edit-country-product-price',
  templateUrl: './add-edit-country-product-price.component.html',
  styleUrls: ['./add-edit-country-product-price.component.sass']
})
export class AddEditCountryProductPriceComponent extends BaseService implements OnInit {

  countryProductPriceDto: CountryProductPriceDto = new CountryProductPriceDto();
  form: FormGroup;
  mode: 'create' | 'update' = 'create';
  // Drp
  countries$: Observable<any[]>;
  products$: Observable<any[]>;

  constructor(@Inject(MAT_DIALOG_DATA) public data: Params,
    public dialogRef: MatDialogRef<AddEditCountryProductPriceComponent>,
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
      countryId: new FormControl(null, [Validators.required]),
      price: new FormControl(null, [Validators.required]),
      packSize: new FormControl(null, [Validators.required]),
      fromDate: new FormControl(null, [Validators.required]),
    });


    if(this.data.fromProductDetails) {
      this.form.controls['productId'].patchValue(this.data.productId);
      this.form.controls['productId'].disable();
    }

    if (this.data && this.data.countryProductPriceDto && this.data.countryProductPriceDto.id) {
      this.mode = 'update';
      this.setFormData();
    } else {
      this.countryProductPriceDto = this.data.countryProductPriceDto || new CountryProductPriceDto();
    }

  }

  loadCountries() {
    this.countries$ = this.httpService.GET(CountriesController.GetAllAsDrp).pipe(map(res => res.data));
  }
  loadProducts() {
    this.products$ = this.httpService.GET(ProductsController.GetAllAsDrp).pipe(map(res => res.data));
  }

  setFormData() {
    this.countryProductPriceDto = JSON.parse(JSON.stringify(this.data.countryProductPriceDto));
    this.form.controls['productId'].patchValue(this.countryProductPriceDto.productId);
    this.form.controls['countryId'].patchValue(this.countryProductPriceDto.countryId);
    this.form.controls['price'].patchValue(this.countryProductPriceDto.price);
    this.form.controls['packSize'].patchValue(this.countryProductPriceDto.packSize);
    this.form.controls['fromDate'].patchValue(this.countryProductPriceDto.fromDate);
    this._ref.detectChanges();
  }

  createObject() {
    this.httpService.POST(CountryProductPricesController.CreateCountryProductPrice, this.countryProductPriceDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.alertService.success('Price is created successfully');
            this.dialogRef.close(this.countryProductPriceDto);
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
    this.httpService.PUT(CountryProductPricesController.UpdateCountryProductPrice, this.countryProductPriceDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.dialogRef.close(this.countryProductPriceDto);
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
    this.countryProductPriceDto.productId = Number(this.form.getRawValue().productId);
    this.countryProductPriceDto.countryId = Number(this.form.getRawValue().countryId);
    this.countryProductPriceDto.price = this.form.getRawValue().price;
    this.countryProductPriceDto.packSize = this.form.getRawValue().packSize;
    this.countryProductPriceDto.fromDate = moment(this.form.getRawValue().fromDate).add(1, 'd').toISOString();

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
