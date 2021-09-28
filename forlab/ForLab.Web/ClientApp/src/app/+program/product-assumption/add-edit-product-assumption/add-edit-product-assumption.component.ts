import { Component, OnInit, Inject, Injector, ViewEncapsulation } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { ProductAssumptionParameterDto } from 'src/@core/models/program/ProductAssumptionParameter';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ProductAssumptionParametersController } from 'src/@core/APIs/ProductAssumptionParametersController';
import { takeUntil, map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { ProgramsController } from 'src/@core/APIs/ProgramsController';
class Params {
  programId: number;
  fromProgramDetails: boolean = false;
  productAssumptionParameterDto: ProductAssumptionParameterDto;
}

@Component({
  selector: 'app-add-edit-product-assumption',
  templateUrl: './add-edit-product-assumption.component.html',
  styleUrls: ['./add-edit-product-assumption.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class AddEditProductAssumptionComponent extends BaseService implements OnInit {

  productAssumptionParameterDto: ProductAssumptionParameterDto = new ProductAssumptionParameterDto();
  form: FormGroup;
  mode: 'create' | 'update' = 'create';
  // Drp
  programs$: Observable<any[]>;

  constructor(@Inject(MAT_DIALOG_DATA) public data: Params,
    public dialogRef: MatDialogRef<AddEditProductAssumptionComponent>,
    private fb: FormBuilder,
    public injector: Injector) {
    super(injector);
  }

  ngOnInit() {

    // Load Drp
    this.loadPrograms();

    this.form = this.fb.group({
      programId: new FormControl(null, [Validators.required]),
      name: new FormControl(null, [Validators.required]),
      isPercentage: new FormControl(false, [Validators.required]),
      isPositive: new FormControl(true, [Validators.required]),
    });

    if (this.data.fromProgramDetails) {
      this.form.controls['programId'].patchValue(this.data.programId);
      this.form.controls['programId'].disable();
    }

    if (this.data && this.data.productAssumptionParameterDto && this.data.productAssumptionParameterDto.id) {
      this.mode = 'update';
      this.setFormData();
    } else {
      this.productAssumptionParameterDto = this.data.productAssumptionParameterDto || new ProductAssumptionParameterDto();
    }


  }

  loadPrograms() {
    this.programs$ = this.httpService.GET(ProgramsController.GetAllAsDrp).pipe(map(res => res.data));
  }

  setFormData() {
    this.productAssumptionParameterDto = JSON.parse(JSON.stringify(this.data.productAssumptionParameterDto));
    this.form.controls['programId'].patchValue(this.productAssumptionParameterDto.programId);
    this.form.controls['name'].patchValue(this.productAssumptionParameterDto.name);
    this.form.controls['isPercentage'].patchValue(this.productAssumptionParameterDto.isPercentage);
    this.form.controls['isPositive'].patchValue(this.productAssumptionParameterDto.isPositive);
    this._ref.detectChanges();
  }

  createObject() {
    this.httpService.POST(ProductAssumptionParametersController.CreateProductAssumptionParameter, this.productAssumptionParameterDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.alertService.success('Product Assumption is created successfully');
            this.dialogRef.close(this.productAssumptionParameterDto);
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
    this.httpService.PUT(ProductAssumptionParametersController.UpdateProductAssumptionParameter, this.productAssumptionParameterDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.dialogRef.close(this.productAssumptionParameterDto);
            this.alertService.success('Product Assumption is updated successfully');
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
    this.productAssumptionParameterDto.programId = Number(this.form.getRawValue().programId);
    this.productAssumptionParameterDto.name = this.form.getRawValue().name;
    this.productAssumptionParameterDto.isPercentage = this.form.getRawValue().isPercentage;
    this.productAssumptionParameterDto.isNumeric = !this.form.getRawValue().isPercentage;
    this.productAssumptionParameterDto.isPositive = this.form.getRawValue().isPositive;
    this.productAssumptionParameterDto.isNegative = !this.form.getRawValue().isPositive;

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
