import { Component, OnInit, Inject, Injector } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { VendorContactDto } from 'src/@core/models/vendor/VendorContact';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { VendorContactsController } from 'src/@core/APIs/VendorContactsController';
import { takeUntil, map } from 'rxjs/operators';
import { RxwebValidators } from '@rxweb/reactive-form-validators';
import { Observable } from 'rxjs';
import { VendorsController } from 'src/@core/APIs/VendorsController';
class Params {
  vendorId: number;
  fromVendorDetails: boolean = false;
  vendorContactDto: VendorContactDto;
}

@Component({
  selector: 'app-add-edit-vendor-contact',
  templateUrl: './add-edit-vendor-contact.component.html',
  styleUrls: ['./add-edit-vendor-contact.component.sass']
})
export class AddEditVendorContactComponent extends BaseService implements OnInit {

  vendorContactDto: VendorContactDto = new VendorContactDto();
  form: FormGroup;
  mode: 'create' | 'update' = 'create';
  // Drp
  vendors$: Observable<any[]>;

  constructor(@Inject(MAT_DIALOG_DATA) public data: Params,
    public dialogRef: MatDialogRef<AddEditVendorContactComponent>,
    private fb: FormBuilder,
    public injector: Injector) {
    super(injector);
  }

  ngOnInit() {

    this.loadVendors();
    
    this.form = this.fb.group({
      vendorId: new FormControl(null, [Validators.required]),
      name: new FormControl(null, [Validators.required]),
      email: new FormControl(null, [Validators.required, Validators.email]),
      telephone: new FormControl(null, [Validators.required, RxwebValidators.numeric()]),
    });


    if(this.data.fromVendorDetails) {
      this.form.controls['vendorId'].patchValue(this.data.vendorId);
      this.form.controls['vendorId'].disable();
    }

    if (this.data && this.data.vendorContactDto && this.data.vendorContactDto.id) {
      this.mode = 'update';
      this.setFormData();
    } else {
      this.vendorContactDto = this.data.vendorContactDto || new VendorContactDto();
    }

  }

  loadVendors() {
    this.vendors$ = this.httpService.GET(VendorsController.GetAllAsDrp).pipe(map(res => res.data));
  }
  
  setFormData() {
    this.vendorContactDto = JSON.parse(JSON.stringify(this.data.vendorContactDto));
    this.form.controls['vendorId'].patchValue(this.vendorContactDto.vendorId);
    this.form.controls['name'].patchValue(this.vendorContactDto.name);
    this.form.controls['email'].patchValue(this.vendorContactDto.email);
    this.form.controls['telephone'].patchValue(this.vendorContactDto.telephone);
    this._ref.detectChanges();
  }

  createObject() {
    this.httpService.POST(VendorContactsController.CreateVendorContact, this.vendorContactDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.alertService.success('Vendor Contact is created successfully');
            this.dialogRef.close(this.vendorContactDto);
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
    this.httpService.PUT(VendorContactsController.UpdateVendorContact, this.vendorContactDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.dialogRef.close(this.vendorContactDto);
            this.alertService.success('Vendor Contact is updated successfully');
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
    this.vendorContactDto.vendorId = this.form.getRawValue().vendorId;
    this.vendorContactDto.name = this.form.getRawValue().name;
    this.vendorContactDto.email = this.form.getRawValue().email;
    this.vendorContactDto.telephone = this.form.getRawValue().telephone;
    console.log(this.vendorContactDto);

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
