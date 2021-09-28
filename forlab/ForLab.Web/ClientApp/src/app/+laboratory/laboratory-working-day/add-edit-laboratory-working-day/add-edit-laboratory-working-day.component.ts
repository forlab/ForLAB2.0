import { Component, OnInit, Inject, Injector, ViewEncapsulation } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { LaboratoryWorkingDayDto } from 'src/@core/models/laboratory/LaboratoryWorkingDay';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { LaboratoryWorkingDaysController } from 'src/@core/APIs/LaboratoryWorkingDaysController';
import { takeUntil, map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { LaboratoriesController } from 'src/@core/APIs/LaboratoriesController';
import { DaysOfWeek } from 'src/@core/models/enum/Enums';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { CountriesController } from 'src/@core/APIs/CountriesController';
import { RegionsController } from 'src/@core/APIs/RegionsController';
class Params {
  laboratoryId: number;
  fromLaboratoryDetails: boolean = false;
  laboratoryWorkingDayDto: LaboratoryWorkingDayDto;
}

@Component({
  selector: 'app-add-edit-laboratory-working-day',
  templateUrl: './add-edit-laboratory-working-day.component.html',
  styleUrls: ['./add-edit-laboratory-working-day.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class AddEditLaboratoryWorkingDayComponent extends BaseService implements OnInit {

  laboratoryWorkingDayDto: LaboratoryWorkingDayDto = new LaboratoryWorkingDayDto();
  form: FormGroup;
  mode: 'create' | 'update' = 'create';
  // Drp
  daysOfWeek = DaysOfWeek;
  // Drp
  countries$: Observable<any[]>;
  regions$: Observable<any[]>;
  laboratories$: Observable<any[]>;

  constructor(@Inject(MAT_DIALOG_DATA) public data: Params,
    public dialogRef: MatDialogRef<AddEditLaboratoryWorkingDayComponent>,
    private fb: FormBuilder,
    public injector: Injector) {
    super(injector);
  }

  ngOnInit() {

    // Load Drp
    this.loadCountries();

    this.form = this.fb.group({
      laboratoryId: new FormControl(null, [Validators.required]),
      day: new FormControl(null, [Validators.required]),
      fromTime: new FormControl(null, [Validators.required]),
      toTime: new FormControl(null, [Validators.required]),

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

    if (this.data && this.data.laboratoryWorkingDayDto && this.data.laboratoryWorkingDayDto.id) {
      this.mode = 'update';
      this.setFormData();
    } else {
      this.laboratoryWorkingDayDto = this.data.laboratoryWorkingDayDto || new LaboratoryWorkingDayDto();
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
    this.laboratoryWorkingDayDto = JSON.parse(JSON.stringify(this.data.laboratoryWorkingDayDto));
    this.form.controls['laboratoryId'].patchValue(this.laboratoryWorkingDayDto.laboratoryId);
    this.form.controls['day'].patchValue(this.laboratoryWorkingDayDto.day);
    this.form.controls['fromTime'].patchValue(this.convertTime12To24(this.laboratoryWorkingDayDto.formatedFromTime));
    this.form.controls['toTime'].patchValue(this.convertTime12To24(this.laboratoryWorkingDayDto.formatedToTime));
    // UI
    this.form.controls['countryId'].patchValue(this.laboratoryWorkingDayDto.laboratoryRegionCountryId);
    this.form.controls['regionId'].patchValue(this.laboratoryWorkingDayDto.laboratoryRegionId);
    this._ref.detectChanges();
  }

  createObject() {
    // We send body as FormData to make C# able to read time from string to TimeSpan
    let formData: FormData = new FormData();
    formData.append('laboratoryWorkingDayDto', JSON.stringify(this.laboratoryWorkingDayDto));

    this.httpService.POST(LaboratoryWorkingDaysController.CreateLaboratoryWorkingDay, formData, null, true)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.alertService.success('Working Day is created successfully');
            this.dialogRef.close(this.laboratoryWorkingDayDto);
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
    // We send body as FormData to make C# able to read time from string to TimeSpan
    let formData: FormData = new FormData();
    formData.append('laboratoryWorkingDayDto', JSON.stringify(this.laboratoryWorkingDayDto));

    this.httpService.PUT(LaboratoryWorkingDaysController.UpdateLaboratoryWorkingDay, formData, null, true)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.dialogRef.close(this.laboratoryWorkingDayDto);
            this.alertService.success('Working Day is updated successfully');
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
    this.laboratoryWorkingDayDto.laboratoryId = Number(this.form.getRawValue().laboratoryId);
    this.laboratoryWorkingDayDto.day = this.form.getRawValue().day;
    this.laboratoryWorkingDayDto.fromTime = this.form.getRawValue().fromTime;
    this.laboratoryWorkingDayDto.toTime = this.form.getRawValue().toTime;

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
