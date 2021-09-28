import { Component, OnInit, Inject, Injector, ViewEncapsulation } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { LaboratoryPatientStatisticDto } from 'src/@core/models/laboratory/LaboratoryPatientStatistic';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { LaboratoryPatientStatisticsController } from 'src/@core/APIs/LaboratoryPatientStatisticsController';
import { takeUntil, map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { LaboratoriesController } from 'src/@core/APIs/LaboratoriesController';
import * as moment from 'moment';
import { CountriesController } from 'src/@core/APIs/CountriesController';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { RegionsController } from 'src/@core/APIs/RegionsController';
import { RxwebValidators } from '@rxweb/reactive-form-validators';
class Params {
  laboratoryId: number;
  fromLaboratoryDetails: boolean = false;
  laboratoryPatientStatisticDto: LaboratoryPatientStatisticDto;
}

@Component({
  selector: 'app-add-edit-laboratory-patient-statistic',
  templateUrl: './add-edit-laboratory-patient-statistic.component.html',
  styleUrls: ['./add-edit-laboratory-patient-statistic.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class AddEditLaboratoryPatientStatisticComponent extends BaseService implements OnInit {

  laboratoryPatientStatisticDto: LaboratoryPatientStatisticDto = new LaboratoryPatientStatisticDto();
  form: FormGroup;
  mode: 'create' | 'update' = 'create';
  // Drp
  countries$: Observable<any[]>;
  regions$: Observable<any[]>;
  laboratories$: Observable<any[]>;

  constructor(@Inject(MAT_DIALOG_DATA) public data: Params,
    public dialogRef: MatDialogRef<AddEditLaboratoryPatientStatisticComponent>,
    private fb: FormBuilder,
    public injector: Injector) {
    super(injector);
  }

  ngOnInit() {

    // Load Drp
    this.loadCountries();

    this.form = this.fb.group({
      laboratoryId: new FormControl(null, [Validators.required]),
      period: new FormControl(null, [Validators.required]),
      count: new FormControl(null, [Validators.required, RxwebValidators.minNumber({ value: 0 })]),
      
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

    if (this.data && this.data.laboratoryPatientStatisticDto && this.data.laboratoryPatientStatisticDto.id) {
      this.mode = 'update';
      this.setFormData();
    } else {
      this.laboratoryPatientStatisticDto = this.data.laboratoryPatientStatisticDto || new LaboratoryPatientStatisticDto();
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

  setFormData() {
    this.laboratoryPatientStatisticDto = JSON.parse(JSON.stringify(this.data.laboratoryPatientStatisticDto));
    this.form.controls['laboratoryId'].patchValue(this.laboratoryPatientStatisticDto.laboratoryId);
    this.form.controls['period'].patchValue(this.laboratoryPatientStatisticDto.period);
    this.form.controls['count'].patchValue(this.laboratoryPatientStatisticDto.count);
    // UI
    this.form.controls['countryId'].patchValue(this.laboratoryPatientStatisticDto.laboratoryRegionCountryId);
    this.form.controls['regionId'].patchValue(this.laboratoryPatientStatisticDto.laboratoryRegionId);
    this._ref.detectChanges();
  }

  createObject() {
    this.httpService.POST(LaboratoryPatientStatisticsController.CreateLaboratoryPatientStatistic, this.laboratoryPatientStatisticDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.alertService.success('Laboratory Patient Statistic is created successfully');
            this.dialogRef.close(this.laboratoryPatientStatisticDto);
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
    this.httpService.PUT(LaboratoryPatientStatisticsController.UpdateLaboratoryPatientStatistic, this.laboratoryPatientStatisticDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.dialogRef.close(this.laboratoryPatientStatisticDto);
            this.alertService.success('Laboratory Patient Statistic is updated successfully');
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
    this.laboratoryPatientStatisticDto.laboratoryId = Number(this.form.getRawValue().laboratoryId);
    this.laboratoryPatientStatisticDto.period = moment(this.form.getRawValue().period).add(1, 'd').toISOString();
    this.laboratoryPatientStatisticDto.count = this.form.getRawValue().count;

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
