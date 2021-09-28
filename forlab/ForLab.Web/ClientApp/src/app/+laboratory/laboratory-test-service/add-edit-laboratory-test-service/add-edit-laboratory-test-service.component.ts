import { Component, OnInit, Inject, Injector, ViewEncapsulation } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { LaboratoryTestServiceDto } from 'src/@core/models/laboratory/LaboratoryTestService';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { LaboratoryTestServicesController } from 'src/@core/APIs/LaboratoryTestServicesController';
import { takeUntil, map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { LaboratoriesController } from 'src/@core/APIs/LaboratoriesController';
import { TestsController } from 'src/@core/APIs/TestsController';
import * as moment from 'moment';
import { RegionsController } from 'src/@core/APIs/RegionsController';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { CountriesController } from 'src/@core/APIs/CountriesController';
import { RxwebValidators } from '@rxweb/reactive-form-validators';
class Params {
  laboratoryId: number;
  fromLaboratoryDetails: boolean = false;
  laboratoryTestServiceDto: LaboratoryTestServiceDto;
}

@Component({
  selector: 'app-add-edit-laboratory-test-service',
  templateUrl: './add-edit-laboratory-test-service.component.html',
  styleUrls: ['./add-edit-laboratory-test-service.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class AddEditLaboratoryTestServiceComponent extends BaseService implements OnInit {

  laboratoryTestServiceDto: LaboratoryTestServiceDto = new LaboratoryTestServiceDto();
  form: FormGroup;
  mode: 'create' | 'update' = 'create';
  // Drp
  tests$: Observable<any[]>;
  countries$: Observable<any[]>;
  regions$: Observable<any[]>;
  laboratories$: Observable<any[]>;

  constructor(@Inject(MAT_DIALOG_DATA) public data: Params,
    public dialogRef: MatDialogRef<AddEditLaboratoryTestServiceComponent>,
    private fb: FormBuilder,
    public injector: Injector) {
    super(injector);
  }

  ngOnInit() {

    // Load Drp
    this.loadCountries();
    this.loadTests();

    this.form = this.fb.group({
      laboratoryId: new FormControl(null, [Validators.required]),
      testId: new FormControl(null, [Validators.required]),
      serviceDuration: new FormControl(null, [Validators.required]),
      testPerformed: new FormControl(null, [Validators.required, RxwebValidators.minNumber({ value: 0 })]),

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

    if (this.data && this.data.laboratoryTestServiceDto && this.data.laboratoryTestServiceDto.id) {
      this.mode = 'update';
      this.setFormData();
    } else {
      this.laboratoryTestServiceDto = this.data.laboratoryTestServiceDto || new LaboratoryTestServiceDto();
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
  loadTests() {
    this.tests$ = this.httpService.GET(TestsController.GetAllAsDrp).pipe(map(res => res.data));
  }

  setFormData() {
    this.laboratoryTestServiceDto = JSON.parse(JSON.stringify(this.data.laboratoryTestServiceDto));
    this.form.controls['laboratoryId'].patchValue(this.laboratoryTestServiceDto.laboratoryId);
    this.form.controls['testId'].patchValue(this.laboratoryTestServiceDto.testId);
    this.form.controls['serviceDuration'].patchValue(this.laboratoryTestServiceDto.serviceDuration);
    this.form.controls['testPerformed'].patchValue(this.laboratoryTestServiceDto.testPerformed);
    // UI
    this.form.controls['countryId'].patchValue(this.laboratoryTestServiceDto.laboratoryRegionCountryId);
    this.form.controls['regionId'].patchValue(this.laboratoryTestServiceDto.laboratoryRegionId);
    this._ref.detectChanges();
  }

  createObject() {
    this.httpService.POST(LaboratoryTestServicesController.CreateLaboratoryTestService, this.laboratoryTestServiceDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.alertService.success('Laboratory Test Service is created successfully');
            this.dialogRef.close(this.laboratoryTestServiceDto);
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
    this.httpService.PUT(LaboratoryTestServicesController.UpdateLaboratoryTestService, this.laboratoryTestServiceDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.dialogRef.close(this.laboratoryTestServiceDto);
            this.alertService.success('Laboratory Test Service is updated successfully');
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
    this.laboratoryTestServiceDto.laboratoryId = Number(this.form.getRawValue().laboratoryId);
    this.laboratoryTestServiceDto.testId = Number(this.form.getRawValue().testId);
    this.laboratoryTestServiceDto.serviceDuration = moment(this.form.getRawValue().serviceDuration).add(1, 'd').toISOString();
    this.laboratoryTestServiceDto.testPerformed = this.form.getRawValue().testPerformed;

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
