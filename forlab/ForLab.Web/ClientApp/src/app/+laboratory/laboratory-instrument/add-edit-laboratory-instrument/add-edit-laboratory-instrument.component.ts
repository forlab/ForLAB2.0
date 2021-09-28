import { Component, OnInit, Inject, Injector, ViewEncapsulation } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { LaboratoryInstrumentDto } from 'src/@core/models/laboratory/LaboratoryInstrument';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { LaboratoryInstrumentsController } from 'src/@core/APIs/LaboratoryInstrumentsController';
import { takeUntil, map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { LaboratoriesController } from 'src/@core/APIs/LaboratoriesController';
import { InstrumentsController } from 'src/@core/APIs/InstrumentsController';
import { CountriesController } from 'src/@core/APIs/CountriesController';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { RegionsController } from 'src/@core/APIs/RegionsController';
import { RxwebValidators } from '@rxweb/reactive-form-validators';
class Params {
  laboratoryId: number;
  fromLaboratoryDetails: boolean = false;
  laboratoryInstrumentDto: LaboratoryInstrumentDto;
}

@Component({
  selector: 'app-add-edit-laboratory-instrument',
  templateUrl: './add-edit-laboratory-instrument.component.html',
  styleUrls: ['./add-edit-laboratory-instrument.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class AddEditLaboratoryInstrumentComponent extends BaseService implements OnInit {

  laboratoryInstrumentDto: LaboratoryInstrumentDto = new LaboratoryInstrumentDto();
  form: FormGroup;
  mode: 'create' | 'update' = 'create';
  // Drp
  instruments$: Observable<any[]>;
  countries$: Observable<any[]>;
  regions$: Observable<any[]>;
  laboratories$: Observable<any[]>;

  constructor(@Inject(MAT_DIALOG_DATA) public data: Params,
    public dialogRef: MatDialogRef<AddEditLaboratoryInstrumentComponent>,
    private fb: FormBuilder,
    public injector: Injector) {
    super(injector);
  }

  ngOnInit() {

    // Load Drp
    this.loadCountries();
    this.loadInstruments();

    this.form = this.fb.group({
      laboratoryId: new FormControl(null, [Validators.required]),
      instrumentId: new FormControl(null, [Validators.required]),
      quantity: new FormControl(null, [Validators.required, RxwebValidators.minNumber({ value: 0 })]),
      testRunPercentage: new FormControl(null, [Validators.required, RxwebValidators.minNumber({ value: 0 }), RxwebValidators.maxNumber({ value: 100 })]),

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

    if (this.data && this.data.laboratoryInstrumentDto && this.data.laboratoryInstrumentDto.id) {
      this.mode = 'update';
      this.setFormData();
    } else {
      this.laboratoryInstrumentDto = this.data.laboratoryInstrumentDto || new LaboratoryInstrumentDto();
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
  loadInstruments() {
    this.instruments$ = this.httpService.GET(InstrumentsController.GetAllAsDrp).pipe(map(res => res.data));
  }

  setFormData() {
    this.laboratoryInstrumentDto = JSON.parse(JSON.stringify(this.data.laboratoryInstrumentDto));
    this.form.controls['laboratoryId'].patchValue(this.laboratoryInstrumentDto.laboratoryId);
    this.form.controls['instrumentId'].patchValue(this.laboratoryInstrumentDto.instrumentId);
    this.form.controls['quantity'].patchValue(this.laboratoryInstrumentDto.quantity);
    this.form.controls['testRunPercentage'].patchValue(this.laboratoryInstrumentDto.testRunPercentage);
    // UI
    this.form.controls['countryId'].patchValue(this.laboratoryInstrumentDto.laboratoryRegionCountryId);
    this.form.controls['regionId'].patchValue(this.laboratoryInstrumentDto.laboratoryRegionId);
    this._ref.detectChanges();
  }

  createObject() {
    this.httpService.POST(LaboratoryInstrumentsController.CreateLaboratoryInstrument, this.laboratoryInstrumentDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.alertService.success('Laboratory Instrument is created successfully');
            this.dialogRef.close(this.laboratoryInstrumentDto);
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
    this.httpService.PUT(LaboratoryInstrumentsController.UpdateLaboratoryInstrument, this.laboratoryInstrumentDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.dialogRef.close(this.laboratoryInstrumentDto);
            this.alertService.success('Laboratory Instrument is updated successfully');
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
    this.laboratoryInstrumentDto.laboratoryId = Number(this.form.getRawValue().laboratoryId);
    this.laboratoryInstrumentDto.instrumentId = Number(this.form.getRawValue().instrumentId);
    this.laboratoryInstrumentDto.quantity = this.form.getRawValue().quantity;
    this.laboratoryInstrumentDto.testRunPercentage = this.form.getRawValue().testRunPercentage;

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
