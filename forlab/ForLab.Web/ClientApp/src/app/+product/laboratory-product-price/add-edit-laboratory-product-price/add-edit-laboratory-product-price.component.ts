import { Component, OnInit, Inject, Injector } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { LaboratoryProductPriceDto } from 'src/@core/models/product/LaboratoryProductPrice';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { LaboratoryProductPricesController } from 'src/@core/APIs/LaboratoryProductPricesController';
import { takeUntil, map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { CountriesController } from 'src/@core/APIs/CountriesController';
import { ProductsController } from 'src/@core/APIs/ProductsController';
import * as moment from 'moment';
import { LaboratoriesController } from 'src/@core/APIs/LaboratoriesController';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { RegionsController } from 'src/@core/APIs/RegionsController';
class Params {
  productId: number;
  fromProductDetails: boolean = false;
  laboratoryProductPriceDto: LaboratoryProductPriceDto;
}

@Component({
  selector: 'app-add-edit-laboratory-product-price',
  templateUrl: './add-edit-laboratory-product-price.component.html',
  styleUrls: ['./add-edit-laboratory-product-price.component.sass']
})
export class AddEditLaboratoryProductPriceComponent extends BaseService implements OnInit {

  laboratoryProductPriceDto: LaboratoryProductPriceDto = new LaboratoryProductPriceDto();
  form: FormGroup;
  mode: 'create' | 'update' = 'create';
  // Drp
  countries$: Observable<any[]>;
  regions$: Observable<any[]>;
  laboratories$: Observable<any[]>;
  products$: Observable<any[]>;

  constructor(@Inject(MAT_DIALOG_DATA) public data: Params,
    public dialogRef: MatDialogRef<AddEditLaboratoryProductPriceComponent>,
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
      laboratoryId: new FormControl(null, [Validators.required]),
      price: new FormControl(null, [Validators.required]),
      packSize: new FormControl(null, [Validators.required]),
      fromDate: new FormControl(null, [Validators.required]),

      // UI
      countryId: new FormControl(null, []),
      regionId: new FormControl(null, []),
    });

    this.form.controls['countryId'].valueChanges
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(countryId => {
        if (countryId) {
          this.loadRegions(countryId);
        }
      });
    this.form.controls['regionId'].valueChanges
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(regionId => {
        if (regionId) {
          this.loadLaboratories(regionId);
        }
      });

    if(this.data.fromProductDetails) {
      this.form.controls['productId'].patchValue(this.data.productId);
      this.form.controls['productId'].disable();
    }

    if (this.data && this.data.laboratoryProductPriceDto && this.data.laboratoryProductPriceDto.id) {
      this.mode = 'update';
      this.setFormData();
    } else {
      this.laboratoryProductPriceDto = this.data.laboratoryProductPriceDto || new LaboratoryProductPriceDto();
    }

  }

  loadCountries() {
    this.countries$ = this.httpService.GET(CountriesController.GetAllAsDrp).pipe(map(res => res.data));
  }
  loadRegions(countryId: number) {
    let params: QueryParamsDto[] = [
      { key: 'countryId', value: countryId }
    ];
    this.regions$ = this.httpService.GET(RegionsController.GetAllAsDrp, params).pipe(map(res => res.data));
  }
  loadLaboratories(regionId: number) {
    let params: QueryParamsDto[] = [
      { key: 'regionId', value: regionId }
    ];
    this.laboratories$ = this.httpService.GET(LaboratoriesController.GetAllAsDrp, params).pipe(map(res => res.data));
  }
  loadProducts() {
    this.products$ = this.httpService.GET(ProductsController.GetAllAsDrp).pipe(map(res => res.data));
  }

  setFormData() {
    this.laboratoryProductPriceDto = JSON.parse(JSON.stringify(this.data.laboratoryProductPriceDto));
    this.form.controls['productId'].patchValue(this.laboratoryProductPriceDto.productId);
    this.form.controls['laboratoryId'].patchValue(this.laboratoryProductPriceDto.laboratoryId);
    this.form.controls['price'].patchValue(this.laboratoryProductPriceDto.price);
    this.form.controls['packSize'].patchValue(this.laboratoryProductPriceDto.packSize);
    this.form.controls['fromDate'].patchValue(this.laboratoryProductPriceDto.fromDate);
    // UI
    this.form.controls['countryId'].patchValue(this.laboratoryProductPriceDto.laboratoryRegionCountryId);
    this.form.controls['regionId'].patchValue(this.laboratoryProductPriceDto.laboratoryRegionId);
    this._ref.detectChanges();
  }

  createObject() {
    this.httpService.POST(LaboratoryProductPricesController.CreateLaboratoryProductPrice, this.laboratoryProductPriceDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.alertService.success('Price is created successfully');
            this.dialogRef.close(this.laboratoryProductPriceDto);
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
    this.httpService.PUT(LaboratoryProductPricesController.UpdateLaboratoryProductPrice, this.laboratoryProductPriceDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.dialogRef.close(this.laboratoryProductPriceDto);
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
    this.laboratoryProductPriceDto.productId = Number(this.form.getRawValue().productId);
    this.laboratoryProductPriceDto.laboratoryId = Number(this.form.getRawValue().laboratoryId);
    this.laboratoryProductPriceDto.price = this.form.getRawValue().price;
    this.laboratoryProductPriceDto.packSize = this.form.getRawValue().packSize;
    this.laboratoryProductPriceDto.fromDate = moment(this.form.getRawValue().fromDate).add(1, 'd').toISOString();

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
