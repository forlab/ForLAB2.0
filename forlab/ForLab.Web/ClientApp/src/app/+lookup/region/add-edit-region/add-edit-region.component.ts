import { Component, OnInit, Inject, Injector } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { RegionDto } from 'src/@core/models/lookup/Region';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { RegionsController } from 'src/@core/APIs/RegionsController';
import { takeUntil, map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { CountriesController } from 'src/@core/APIs/CountriesController';

@Component({
  selector: 'app-add-edit-region',
  templateUrl: './add-edit-region.component.html',
  styleUrls: ['./add-edit-region.component.sass']
})
export class AddEditRegionComponent extends BaseService implements OnInit {

  regionDto: RegionDto = new RegionDto();
  form: FormGroup;
  mode: 'create' | 'update' = 'create';
  // Drp
  countries$: Observable<any[]>;

  constructor(@Inject(MAT_DIALOG_DATA) public data: RegionDto,
    public dialogRef: MatDialogRef<AddEditRegionComponent>,
    private fb: FormBuilder,
    public injector: Injector) {
    super(injector);
  }

  ngOnInit() {

    this.loadCountries();

    this.form = this.fb.group({
      name: new FormControl(null, [Validators.required]),
      countryId: new FormControl(null, [Validators.required]),
      shortName: new FormControl(null, [Validators.required]),
    });

    if (this.data && this.data.id) {
      this.mode = 'update';
      this.setFormData();
    } else {
      this.regionDto = this.data || new RegionDto();
    }

  }

  loadCountries() {
    this.countries$ = this.httpService.GET(CountriesController.GetAllAsDrp).pipe(map(res => res.data));
  }

  setFormData() {
    this.form.controls['name'].patchValue(this.data.name);
    this.form.controls['countryId'].patchValue(this.data.countryId);
    this.form.controls['shortName'].patchValue(this.data.shortName);
    this.regionDto = JSON.parse(JSON.stringify(this.data));
    this._ref.detectChanges();
  }

  createObject() {
    this.httpService.POST(RegionsController.CreateRegion, this.regionDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.alertService.success('Region is created successfully');
            this.dialogRef.close(this.regionDto);
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
    this.httpService.PUT(RegionsController.UpdateRegion, this.regionDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.dialogRef.close(this.regionDto);
            this.alertService.success('Region is updated successfully');
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
    this.regionDto.name = this.form.getRawValue().name;
    this.regionDto.countryId = this.form.getRawValue().countryId;
    this.regionDto.shortName = this.form.getRawValue().shortName;

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
