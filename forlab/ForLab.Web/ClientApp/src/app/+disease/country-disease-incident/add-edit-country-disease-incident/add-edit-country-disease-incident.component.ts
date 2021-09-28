import { Component, OnInit, Inject, Injector } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { CountryDiseaseIncidentDto } from 'src/@core/models/disease/CountryDiseaseIncident';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { CountryDiseaseIncidentsController } from 'src/@core/APIs/CountryDiseaseIncidentsController';
import { takeUntil, map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { CountriesController } from 'src/@core/APIs/CountriesController';
import { DiseasesController } from 'src/@core/APIs/DiseasesController';

@Component({
  selector: 'app-add-edit-country-disease-incident',
  templateUrl: './add-edit-country-disease-incident.component.html',
  styleUrls: ['./add-edit-country-disease-incident.component.scss']
})
export class AddEditCountryDiseaseIncidentComponent extends BaseService implements OnInit {

  countryDiseaseIncidentDto: CountryDiseaseIncidentDto = new CountryDiseaseIncidentDto();
  form: FormGroup;
  mode: 'create' | 'update' = 'create';
  // Drp
  countries$: Observable<any[]>;
  diseases$: Observable<any[]>;

  constructor(@Inject(MAT_DIALOG_DATA) public data: CountryDiseaseIncidentDto,
    public dialogRef: MatDialogRef<AddEditCountryDiseaseIncidentComponent>,
    private fb: FormBuilder,
    public injector: Injector) {
    super(injector);
  }

  ngOnInit() {

    // Load Drp
    this.loadCountries();
    this.loadDiseases();

    this.form = this.fb.group({
      countryId: new FormControl(null, [Validators.required]),
      diseaseId: new FormControl(null, [Validators.required]),
      year: new FormControl(null, [Validators.required]),
      incidence: new FormControl(null, [Validators.required]),
      incidencePer1kPopulation: new FormControl(null, [Validators.required]),
      incidencePer100kPopulation: new FormControl(null, [Validators.required]),
      prevalenceRate: new FormControl(null, [Validators.required]),
      prevalenceRatePer1kPopulation: new FormControl(null, [Validators.required]),
      prevalenceRatePer100kPopulation: new FormControl(null, [Validators.required]),
      note: new FormControl(null, []),
    });

    if (this.data && this.data.id) {
      this.mode = 'update';
      this.setFormData();
    } else {
      this.countryDiseaseIncidentDto = this.data || new CountryDiseaseIncidentDto();
    }

  }

  loadCountries() {
    this.countries$ = this.httpService.GET(CountriesController.GetAllAsDrp).pipe(map(res => res.data));
  }
  loadDiseases() {
    this.diseases$ = this.httpService.GET(DiseasesController.GetAllAsDrp).pipe(map(res => res.data));
  }

  setFormData() {
    this.countryDiseaseIncidentDto = JSON.parse(JSON.stringify(this.data));
    this.form.controls['countryId'].patchValue(this.countryDiseaseIncidentDto.countryId);
    this.form.controls['diseaseId'].patchValue(this.countryDiseaseIncidentDto.diseaseId);
    this.form.controls['year'].patchValue(this.countryDiseaseIncidentDto.year);
    this.form.controls['incidence'].patchValue(this.countryDiseaseIncidentDto.incidence);
    this.form.controls['incidencePer1kPopulation'].patchValue(this.countryDiseaseIncidentDto.incidencePer1kPopulation);
    this.form.controls['incidencePer100kPopulation'].patchValue(this.countryDiseaseIncidentDto.incidencePer100kPopulation);
    this.form.controls['prevalenceRate'].patchValue(this.countryDiseaseIncidentDto.prevalenceRate);
    this.form.controls['prevalenceRatePer1kPopulation'].patchValue(this.countryDiseaseIncidentDto.prevalenceRatePer1kPopulation);
    this.form.controls['prevalenceRatePer100kPopulation'].patchValue(this.countryDiseaseIncidentDto.prevalenceRatePer100kPopulation);
    this.form.controls['note'].patchValue(this.countryDiseaseIncidentDto.note);
    this._ref.detectChanges();
  }

  createObject() {
    this.httpService.POST(CountryDiseaseIncidentsController.CreateCountryDiseaseIncident, this.countryDiseaseIncidentDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.alertService.success('Country Disease Incident is created successfully');
            this.dialogRef.close(this.countryDiseaseIncidentDto);
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
    this.httpService.PUT(CountryDiseaseIncidentsController.UpdateCountryDiseaseIncident, this.countryDiseaseIncidentDto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            this.dialogRef.close(this.countryDiseaseIncidentDto);
            this.alertService.success('Country Disease Incident is updated successfully');
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
    this.countryDiseaseIncidentDto.countryId = this.form.getRawValue().countryId;
    this.countryDiseaseIncidentDto.diseaseId = this.form.getRawValue().diseaseId;
    this.countryDiseaseIncidentDto.year = this.form.getRawValue().year;
    this.countryDiseaseIncidentDto.incidence = this.form.getRawValue().incidence;
    this.countryDiseaseIncidentDto.incidencePer1kPopulation = this.form.getRawValue().incidencePer1kPopulation;
    this.countryDiseaseIncidentDto.incidencePer100kPopulation = this.form.getRawValue().incidencePer100kPopulation;
    this.countryDiseaseIncidentDto.prevalenceRate = this.form.getRawValue().prevalenceRate;
    this.countryDiseaseIncidentDto.prevalenceRatePer1kPopulation = this.form.getRawValue().prevalenceRatePer1kPopulation;
    this.countryDiseaseIncidentDto.prevalenceRatePer100kPopulation = this.form.getRawValue().prevalenceRatePer100kPopulation;
    this.countryDiseaseIncidentDto.note = this.form.getRawValue().note;

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
