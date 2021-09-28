import { Component, OnInit, Inject, Injector, ViewEncapsulation } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { LaboratoryConsumptionDto } from 'src/@core/models/laboratory/LaboratoryConsumption';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { LaboratoryConsumptionsController } from 'src/@core/APIs/LaboratoryConsumptionsController';
import { takeUntil, map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { LaboratoriesController } from 'src/@core/APIs/LaboratoriesController';
import { ProductsController } from 'src/@core/APIs/ProductsController';
import * as moment from 'moment';
import { CountriesController } from 'src/@core/APIs/CountriesController';
import { RegionsController } from 'src/@core/APIs/RegionsController';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { RxwebValidators } from '@rxweb/reactive-form-validators';
class Params {
  laboratoryId: number;
  fromLaboratoryDetails: boolean = false;
  laboratoryConsumptionDto: LaboratoryConsumptionDto;
}

@Component({
  selector: 'app-add-edit-laboratory-consumption',
  templateUrl: './add-edit-laboratory-consumption.component.html',
  styleUrls: ['./add-edit-laboratory-consumption.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class AddEditLaboratoryConsumptionComponent extends BaseService implements OnInit {

  laboratoryConsumptionDto: LaboratoryConsumptionDto = new LaboratoryConsumptionDto();
  form: FormGroup;
  mode: 'create' | 'update' = 'create';
  // Drp
  products$: Observable<any[]>;
  countries$: Observable<any[]>;
  regions$: Observable<any[]>;
  laboratories$: Observable<any[]>;

  constructor(@Inject(MAT_DIALOG_DATA) public data: Params,
    public dialogRef: MatDialogRef<AddEditLaboratoryConsumptionComponent>,
    private fb: FormBuilder,
    public injector: Injector) {
    super(injector);
  }

  ngOnInit() {

    // Load Drp
    this.loadCountries();
    this.loadProducts();

    this.form = this.fb.group({
      laboratoryId: new FormControl(null, [Validators.required]),
      productId: new FormControl(null, [Validators.required]),
      consumptionDuration: new FormControl(null, [Validators.required]),
      amountUsed: new FormControl(null, [Validators.required, RxwebValidators.minNumber({ value: 0 })]),

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

    if (this.data.fromLaboratoryDetails) {
      this.form.controls['laboratoryId'].patchValue(this.data.laboratoryId);
      this.form.controls['laboratoryId'].disable();
      this.form.controls['countryId'].disable();
      this.form.controls['regionId'].disable();
    }

    if (this.data && this.data.laboratoryConsumptionDto && this.data.laboratoryConsumptionDto.id) {
      this.mode = 'update';
      this.setFormData();
    } else {
      this.laboratoryConsumptionDto = this.data.laboratoryConsumptionDto || new LaboratoryConsumptionDto();
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
    this.laboratoryConsumptionDto = JSON.parse(JSON.stringify(this.data.laboratoryConsumptionDto));
    this.form.controls['laboratoryId'].patchValue(this.laboratoryConsumptionDto.laboratoryId);
    this.form.controls['productId'].patchValue(this.laboratoryConsumptionDto.productId);
    this.form.controls['consumptionDuration'].patchValue(this.laboratoryConsumptionDto.consumptionDuration);
    this.form.controls['amountUsed'].patchValue(this.laboratoryConsumptionDto.amountUsed);
    // UI
    this.form.controls['countryId'].patchValue(this.laboratoryConsumptionDto.laboratoryRegionCountryId);
    this.form.controls['regionId'].patchValue(this.laboratoryConsumptionDto.laboratoryRegionId);
    this._ref.detectChanges();
  }

  createObject() {
    this.httpService.POST(LaboratoryConsumptionsController.CreateLaboratoryConsumption, this.laboratoryConsumptionDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.alertService.success('Laboratory Consumption is created successfully');
            this.dialogRef.close(this.laboratoryConsumptionDto);
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
    this.httpService.PUT(LaboratoryConsumptionsController.UpdateLaboratoryConsumption, this.laboratoryConsumptionDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.dialogRef.close(this.laboratoryConsumptionDto);
            this.alertService.success('Laboratory Consumption is updated successfully');
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
    this.laboratoryConsumptionDto.laboratoryId = Number(this.form.getRawValue().laboratoryId);
    this.laboratoryConsumptionDto.productId = Number(this.form.getRawValue().productId);
    this.laboratoryConsumptionDto.consumptionDuration = moment(this.form.getRawValue().consumptionDuration).add(1, 'd').toISOString();
    this.laboratoryConsumptionDto.amountUsed = this.form.getRawValue().amountUsed;

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
