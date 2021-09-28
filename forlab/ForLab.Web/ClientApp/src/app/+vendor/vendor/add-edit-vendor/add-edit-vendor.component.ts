import { Component, OnInit, Inject, Injector } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { VendorDto } from 'src/@core/models/vendor/Vendor';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { VendorsController } from 'src/@core/APIs/VendorsController';
import { takeUntil } from 'rxjs/operators';
import { RxwebValidators } from '@rxweb/reactive-form-validators';

@Component({
  selector: 'app-add-edit-vendor',
  templateUrl: './add-edit-vendor.component.html',
  styleUrls: ['./add-edit-vendor.component.scss']
})
export class AddEditVendorComponent extends BaseService implements OnInit {
  
  vendorDto: VendorDto = new VendorDto();
  form: FormGroup;
  mode: 'create' | 'update' = 'create';

  constructor(@Inject(MAT_DIALOG_DATA) public data: VendorDto,
    public dialogRef: MatDialogRef<AddEditVendorComponent>,
    private fb: FormBuilder,
    public injector: Injector) {
    super(injector);
  }

  ngOnInit() {

    this.form = this.fb.group({
      name: new FormControl(null, [Validators.required]),
      email: new FormControl(null, [Validators.required, Validators.email]),
      telephone: new FormControl(null, [Validators.required, RxwebValidators.numeric()]),
      url: new FormControl(null, [RxwebValidators.url()]),
      address: new FormControl(null, []),
    });

    if (this.data && this.data.id) {
      this.mode = 'update';
      this.setFormData();
    } else {
      this.vendorDto = this.data || new VendorDto();
    }

  }

  setFormData() {
    this.form.controls['name'].patchValue(this.data.name);
    this.form.controls['email'].patchValue(this.data.email);
    this.form.controls['telephone'].patchValue(this.data.telephone);
    this.form.controls['url'].patchValue(this.data.url);
    this.form.controls['address'].patchValue(this.data.address);
    this.vendorDto = JSON.parse(JSON.stringify(this.data));
    this._ref.detectChanges();
  }

  createObject() {
    this.httpService.POST(VendorsController.CreateVendor, this.vendorDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.alertService.success('Vendor is created successfully');
            this.dialogRef.close(this.vendorDto);
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
    this.httpService.PUT(VendorsController.UpdateVendor, this.vendorDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.dialogRef.close(this.vendorDto);
            this.alertService.success('Vendor is updated successfully');
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
    this.vendorDto.name = this.form.getRawValue().name;
    this.vendorDto.email = this.form.getRawValue().email;
    this.vendorDto.telephone = this.form.getRawValue().telephone;
    this.vendorDto.url = this.form.getRawValue().url;
    this.vendorDto.address = this.form.getRawValue().address;
    console.log(this.vendorDto);

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
