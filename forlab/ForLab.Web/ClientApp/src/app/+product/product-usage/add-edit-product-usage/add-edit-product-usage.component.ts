import { Component, OnInit, Inject, Injector } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { ProductUsageDto } from 'src/@core/models/product/ProductUsage';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ProductUsagesController } from 'src/@core/APIs/ProductUsagesController';
import { takeUntil, map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { ProductsController } from 'src/@core/APIs/ProductsController';
import { CountryPeriodEnum, ProductTypeEnum } from 'src/@core/models/enum/Enums';
import { TestsController } from 'src/@core/APIs/TestsController';
import { InstrumentsController } from 'src/@core/APIs/InstrumentsController';
import { ProductDto } from 'src/@core/models/product/Product';
import { QueryParamsDto } from 'src/@core/models/common/response';
class Params {
  productId: number;
  fromProductDetails: boolean = false;
  testId: number;
  fromTestDetails: boolean = false;
  productUsageDto: ProductUsageDto;
}

@Component({
  selector: 'app-add-edit-product-usage',
  templateUrl: './add-edit-product-usage.component.html',
  styleUrls: ['./add-edit-product-usage.component.scss']
})
export class AddEditProductUsageComponent extends BaseService implements OnInit {

  productUsageDto: ProductUsageDto = new ProductUsageDto();
  form: FormGroup;
  mode: 'create' | 'update' = 'create';
  // Drp
  countryPeriodEnum = CountryPeriodEnum;
  productTypeEnum = ProductTypeEnum;
  products: ProductDto[] = [];
  tests$: Observable<any[]>;
  instruments$: Observable<any[]>;

  constructor(@Inject(MAT_DIALOG_DATA) public data: Params,
    public dialogRef: MatDialogRef<AddEditProductUsageComponent>,
    private fb: FormBuilder,
    public injector: Injector) {
    super(injector);
  }

  ngOnInit() {

    // Load Drp
    this.loadProducts();
    this.loadTests();
    this.loadInstruments();

    this.form = this.fb.group({
      productId: new FormControl(null, [Validators.required]),
      testId: new FormControl(null, [Validators.required]),
      instrumentId: new FormControl(null),
      amount: new FormControl(null, [Validators.required]),
      isForControl: new FormControl(false, [Validators.required]),
      perPeriod: new FormControl(true, [Validators.required]),
      perPeriodPerInstrument: new FormControl(false),
      countryPeriodId: new FormControl(null, [Validators.required]),
    });

    this.form.controls['perPeriod'].valueChanges
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(perPeriod => {
        this.form.controls['perPeriodPerInstrument'].patchValue(!perPeriod);
        if (perPeriod) {
          this.form.controls['countryPeriodId'].setValidators([Validators.required]);
        } else {
          this.form.controls['countryPeriodId'].clearValidators();
        }
        this.form.controls['countryPeriodId'].updateValueAndValidity();
      });

    this.form.controls['perPeriodPerInstrument'].valueChanges
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(perPeriodPerInstrument => {
        if (perPeriodPerInstrument) {
          this.form.controls['instrumentId'].setValidators([Validators.required]);
        } else {
          this.form.controls['instrumentId'].clearValidators();
        }
        this.form.controls['instrumentId'].updateValueAndValidity();
      });

    if (this.data.fromProductDetails) {
      this.form.controls['productId'].patchValue(this.data.productId, { emitEvent: false });
      this.form.controls['productId'].disable();
      // Remove Required
      this.form.controls['testId'].clearValidators();
      // Refresh
      this.form.controls['testId'].updateValueAndValidity();
    } else if (this.data.fromTestDetails) {
      // Patch
      this.form.controls['perPeriod'].patchValue(false);
      this.form.controls['perPeriodPerInstrument'].patchValue(false);
      // Add Required
      this.form.controls['testId'].setValidators([Validators.required]);
      this.form.controls['instrumentId'].setValidators([Validators.required]);
      // Remove Required
      this.form.controls['countryPeriodId'].clearValidators();
      // Refresh
      this.form.controls['testId'].updateValueAndValidity();
      this.form.controls['instrumentId'].updateValueAndValidity();
    }

    if (this.data && this.data.productUsageDto && this.data.productUsageDto.id) {
      this.mode = 'update';
      this.setFormData();
    } else {
      this.productUsageDto = this.data.productUsageDto || new ProductUsageDto();
    }

  }

  loadProducts() {
    let params: QueryParamsDto[] = [];
    if (this.data.fromTestDetails) {
      params.push({ key: 'productTypeIds', value: [this.productTypeEnum.QualityControl.toString(), this.productTypeEnum.Reagents.toString()].join(',') });
    } else if (this.data.fromProductDetails) {
      params.push({ key: 'productTypeIds', value: [this.productTypeEnum.Consumables.toString(), this.productTypeEnum.Calibrators.toString()].join(',') });
    }

    this.httpService.GET(ProductsController.GetAllAsDrp, params)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.products = res.data;
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
  loadTests() {
    this.tests$ = this.httpService.GET(TestsController.GetAllAsDrp).pipe(map(res => res.data));
  }
  loadInstruments() {
    this.instruments$ = this.httpService.GET(InstrumentsController.GetAllAsDrp).pipe(map(res => res.data));
  }

  setFormData() {
    this.productUsageDto = JSON.parse(JSON.stringify(this.data.productUsageDto));
    this.form.controls['productId'].patchValue(this.productUsageDto.productId, { emitEvent: false });
    this.form.controls['testId'].patchValue(this.productUsageDto.testId);
    this.form.controls['instrumentId'].patchValue(this.productUsageDto.instrumentId);
    this.form.controls['amount'].patchValue(this.productUsageDto.amount);
    this.form.controls['isForControl'].patchValue(this.productUsageDto.isForControl);
    this.form.controls['perPeriod'].patchValue(this.productUsageDto.perPeriod);
    this.form.controls['perPeriodPerInstrument'].patchValue(this.productUsageDto.perPeriodPerInstrument);
    this.form.controls['countryPeriodId'].patchValue(this.productUsageDto.countryPeriodId);
    this._ref.detectChanges();
  }

  createObject() {
    this.httpService.POST(ProductUsagesController.CreateProductUsage, this.productUsageDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.alertService.success('Product Usage is created successfully');
            this.dialogRef.close(this.productUsageDto);
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
    this.httpService.PUT(ProductUsagesController.UpdateProductUsage, this.productUsageDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.dialogRef.close(this.productUsageDto);
            this.alertService.success('Product Usage is updated successfully');
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
    this.productUsageDto.productId = Number(this.form.getRawValue().productId);
    this.productUsageDto.testId = Number(this.form.getRawValue().testId);
    this.productUsageDto.instrumentId = Number(this.form.getRawValue().instrumentId);
    this.productUsageDto.amount = this.form.getRawValue().amount;
    this.productUsageDto.isForControl = this.form.getRawValue().isForControl;
    this.productUsageDto.perPeriod = this.form.getRawValue().perPeriod;
    this.productUsageDto.perPeriodPerInstrument = this.form.getRawValue().perPeriodPerInstrument;
    this.productUsageDto.countryPeriodId = Number(this.form.getRawValue().countryPeriodId);

    if (this.data.fromProductDetails) {
      this.productUsageDto.testId = null;
      this.productUsageDto.isForControl = false;
      if(this.productUsageDto.perPeriod) {
        this.productUsageDto.instrumentId = null;
      }
    } else if (this.data.fromTestDetails) {
      this.productUsageDto.perPeriod = false;
      this.productUsageDto.perPeriodPerInstrument = false;
      this.productUsageDto.countryPeriodId = null;
    }

    if (this.productUsageDto.countryPeriodId == 0) {
      this.productUsageDto.countryPeriodId = null;
    }
    if (this.productUsageDto.instrumentId == 0) {
      this.productUsageDto.instrumentId = null;
    }
    if (this.getProductTypeId != this.productTypeEnum.QualityControl) {
      this.productUsageDto.isForControl = false;
    }

    if (this.mode === 'create') {
      this.createObject();
    } else if (this.mode === 'update') {
      this.updateObject();
    }
  }

  get getProductTypeId(): number {
    if (!this.form.getRawValue().productId || !this.products || this.products.length == 0) return 0;
    return this.products.find(x => x.id == this.form.getRawValue().productId).productTypeId || 0;
  }

  isCreateMode() {
    return this.mode === 'create';
  }

  isUpdateMode() {
    return this.mode === 'update';
  }
}
