import { Component, OnInit, Inject, Injector } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { ProductDto } from 'src/@core/models/product/Product';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ProductsController } from 'src/@core/APIs/ProductsController';
import { ProductTypeEnum, ProductBasicUnitEnum } from 'src/@core/models/enum/Enums';
import { takeUntil, map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { VendorsController } from 'src/@core/APIs/VendorsController';

@Component({
  selector: 'app-add-edit-product',
  templateUrl: './add-edit-product.component.html',
  styleUrls: ['./add-edit-product.component.sass']
})
export class AddEditProductComponent extends BaseService implements OnInit {

  productDto: ProductDto = new ProductDto();
  form: FormGroup;
  mode: 'create' | 'update' = 'create';
  // Drp
  productTypeEnum = ProductTypeEnum;
  productBasicUnitEnum = ProductBasicUnitEnum;
  vendors$: Observable<any[]>;

  constructor(@Inject(MAT_DIALOG_DATA) public data: ProductDto,
    public dialogRef: MatDialogRef<AddEditProductComponent>,
    private fb: FormBuilder,
    public injector: Injector) {
    super(injector);
  }

  ngOnInit() {

    // Load Drp
    this.loadVendors();

    this.form = this.fb.group({
      name: new FormControl(null, [Validators.required]),
      vendorId: new FormControl(null, [Validators.required]),
      manufacturerPrice: new FormControl(null, [Validators.required]),
      productTypeId: new FormControl(null, [Validators.required]),
      catalogNo: new FormControl(null, [Validators.required]),
      productBasicUnitId: new FormControl(null, [Validators.required]),
      packSize: new FormControl(null, [Validators.required]),
    });

    if (this.data && this.data.id) {
      this.mode = 'update';
      this.setFormData();
    } else {
      this.productDto = this.data || new ProductDto();
    }

  }

  loadVendors() {
    this.vendors$ = this.httpService.GET(VendorsController.GetAllAsDrp).pipe(map(res => res.data));
  }

  setFormData() {
    this.form.controls['name'].patchValue(this.data.name);
    this.form.controls['vendorId'].patchValue(this.data.vendorId);
    this.form.controls['manufacturerPrice'].patchValue(this.data.manufacturerPrice);
    this.form.controls['productTypeId'].patchValue(this.data.productTypeId);
    this.form.controls['catalogNo'].patchValue(this.data.catalogNo);
    this.form.controls['productBasicUnitId'].patchValue(this.data.productBasicUnitId);
    this.form.controls['packSize'].patchValue(this.data.packSize);
    this.productDto = JSON.parse(JSON.stringify(this.data));
    this._ref.detectChanges();
  }

  createObject() {
    this.httpService.POST(ProductsController.CreateProduct, this.productDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.alertService.success('Product is created successfully');
            this.dialogRef.close(this.productDto);
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
    this.httpService.PUT(ProductsController.UpdateProduct, this.productDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.dialogRef.close(this.productDto);
            this.alertService.success('Product is updated successfully');
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
    this.productDto.name = this.form.getRawValue().name;
    this.productDto.vendorId = Number(this.form.getRawValue().vendorId);
    this.productDto.manufacturerPrice = this.form.getRawValue().manufacturerPrice;
    this.productDto.productTypeId = Number(this.form.getRawValue().productTypeId);
    this.productDto.productBasicUnitId = Number(this.form.getRawValue().productBasicUnitId);
    this.productDto.catalogNo = this.form.getRawValue().catalogNo;
    this.productDto.packSize = Number(this.form.getRawValue().packSize);

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
