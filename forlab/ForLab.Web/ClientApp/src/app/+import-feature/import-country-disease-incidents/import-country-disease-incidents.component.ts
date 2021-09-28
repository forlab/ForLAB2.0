import { Component, OnInit, ViewChild, Injector, ViewEncapsulation, Input, ChangeDetectionStrategy } from '@angular/core';
import { TableColumn } from 'src/@core/models/common/response';
import { SelectionModel } from '@angular/cdk/collections';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { BaseService } from 'src/@core/services/base.service';
import { MatPaginator } from '@angular/material/paginator';
import { CountryDiseaseIncidentDto } from 'src/@core/models/disease/CountryDiseaseIncident';
import { CountryDto } from 'src/@core/models/lookup/Country';
import { CountryDiseaseIncidentsController } from 'src/@core/APIs/CountryDiseaseIncidentsController';
import { map, takeUntil } from 'rxjs/operators';
import { CountriesController } from 'src/@core/APIs/CountriesController';
import { DiseaseDto } from 'src/@core/models/disease/Disease';
import { DiseasesController } from 'src/@core/APIs/DiseasesController';
import { FormValidationError } from 'src/@core/models/common/SharedModel';
import { TableVirtualScrollDataSource } from 'ng-table-virtual-scroll';
import { combineLatest } from 'rxjs';

@Component({
  selector: 'import-country-disease-incidents',
  templateUrl: './import-country-disease-incidents.component.html',
  styleUrls: ['./import-country-disease-incidents.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ImportCountryDiseaseIncidentsComponent extends BaseService implements OnInit {
  // Drp
  countries: CountryDto[] = [];
  diseases: DiseaseDto[] = [];
  // Table
  @Input('data') data: CountryDiseaseIncidentDto[] = [];
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  columns: TableColumn<CountryDiseaseIncidentDto>[] = [
    { label: 'Actions', property: 'actions', type: 'button', visible: true },
    { label: '#', property: 'index', type: 'custom', visible: true },
    { label: 'Country', property: 'countryId', type: 'select', visible: true },
    { label: 'Disease', property: 'diseaseId', type: 'select', visible: true },
    { label: 'Per 1k Population', property: 'incidencePer1kPopulation', type: 'number', visible: true },
    { label: 'Per 100k Population', property: 'incidencePer100kPopulation', type: 'number', visible: true },
    { label: 'Prevalence Rate', property: 'prevalenceRate', type: 'number', visible: true },
    { label: 'Prevalence Rate per 1k Population', property: 'prevalenceRatePer1kPopulation', type: 'number', visible: true },
    { label: 'Prevalence Rate per 100k Population', property: 'prevalenceRatePer100kPopulation', type: 'number', visible: true },
    { label: 'Note', property: 'note', type: 'text', visible: true },
  ];
  dataSource = new TableVirtualScrollDataSource();
  selection = new SelectionModel<CountryDiseaseIncidentDto>(true, []);
  // Form
  formArry: FormGroup[];
  // vars
  loadingData: boolean = true;
  errors: FormValidationError[] = [];
  successRes: any;
  done: boolean = false;
  // New Approch
  durations: string[] = [];

  constructor(private fb: FormBuilder, public injector: Injector) {
    super(injector);
  }

  ngOnInit() {

    // Set durations
    this.durations = Object.keys(this.data[0])?.filter(x => x != 'countryName' && x != 'diseaseName' && x != 'incidencePer1kPopulation' && x != 'incidencePer100kPopulation' && x != 'prevalenceRate' && x != 'prevalenceRatePer1kPopulation' && x != 'prevalenceRatePer100kPopulation' && x != 'note');
    this.durations?.forEach(x => {
      this.columns.push({ label: x, property: x, type: 'number', visible: true });
    });

    combineLatest([this.loadCountries(), this.loadDiseases()])
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(([countries, diseases]) => {
        // Set Drp data
        this.countries = countries;
        this.diseases = diseases;

        // Patch the Table Data
        this.formArry = Array.from(Array(this.data.length), (_, i) => i + 1).map(x => {
          let form = this.appendObject();

          // Country
          this.data[x - 1].countryId = countries.find(item => item.name.trim().toLowerCase() == this.data[x - 1].countryName.trim().toLowerCase())?.id;
          // Disease
          this.data[x - 1].diseaseId = diseases.find(item => item.name.trim().toLowerCase() == this.data[x - 1].diseaseName.trim().toLowerCase())?.id;

          // Patch FormGroup Value
          form.patchValue(this.data[x - 1]);
          return form;
        });

        this.loadingData = false;
        this.dataSource = new TableVirtualScrollDataSource(this.formArry);
        this.dataSource.paginator = this.paginator;
      });

  }

  loadCountries() {
    return this.httpService.GET(CountriesController.GetAllAsDrp).pipe(map(res => res.data));
  }
  loadDiseases() {
    return this.httpService.GET(DiseasesController.GetAllAsDrp).pipe(map(res => res.data));
  }

  get visibleColumns() {
    if (!this.columns) {
      return [];
    }
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  toggleColumnVisibility(column, event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    column.visible = !column.visible;
  }

  trackByProperty<T>(index: number, column: TableColumn<T>) {
    return column.property;
  }

  appendObject() {
    let formItem = this.fb.group({
      countryId: new FormControl(null, [Validators.required]),
      diseaseId: new FormControl(null, [Validators.required]),
      incidencePer1kPopulation: new FormControl(null, [Validators.required]),
      incidencePer100kPopulation: new FormControl(null, [Validators.required]),
      prevalenceRate: new FormControl(null, [Validators.required]),
      prevalenceRatePer1kPopulation: new FormControl(null, [Validators.required]),
      prevalenceRatePer100kPopulation: new FormControl(null, [Validators.required]),
      note: new FormControl(null),
    });

    this.durations?.forEach(x => {
      formItem.addControl(x, new FormControl(null));
    });

    return formItem;
  }

  addObject() {
    this.formArry.unshift(this.appendObject());
    this.dataSource = new TableVirtualScrollDataSource(this.formArry);
    this.dataSource.paginator = this.paginator;
  }

  deleteObject(row) {
    const index = this.formArry.indexOf(row);
    this.formArry.splice(index, 1);
    this.dataSource = new TableVirtualScrollDataSource(this.formArry);
    this.dataSource.paginator = this.paginator;
  }

  submit() {
    const invalidFormGroups = this.formArry?.filter(x => x.invalid);
    /** check form */
    this.errors = [];
    if (invalidFormGroups?.length > 0) {
      invalidFormGroups.forEach(x => x.markAllAsTouched());
      this.errors = this.getFormGroupsErrors(invalidFormGroups, this.formArry);
      return;
    }

    this.loading = true;
    let objectDtos: CountryDiseaseIncidentDto[] = [];

    this.formArry.forEach(element => {
      // Add durations
      this.durations?.forEach(x => {
        let val = new CountryDiseaseIncidentDto();
        val.countryId = Number(element.value.countryId);
        val.diseaseId = Number(element.value.diseaseId);
        val.incidencePer1kPopulation = Number(element.value.incidencePer1kPopulation);
        val.incidencePer100kPopulation = Number(element.value.incidencePer100kPopulation);
        val.prevalenceRate = Number(element.value.prevalenceRate);
        val.prevalenceRatePer1kPopulation = Number(element.value.prevalenceRatePer1kPopulation);
        val.prevalenceRatePer100kPopulation = Number(element.value.prevalenceRatePer100kPopulation);
        val.note = String(element.value.note);
        val.year = Number(x);
        val.incidence = element.value[x] ? Number(element.value[x]) : 0;
        objectDtos.push(val);
      });
    });

    this.httpService.POST(CountryDiseaseIncidentsController.ImportCountryDiseaseIncidents, objectDtos)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {

        if (res.isPassed) {
          this.successRes = res.data;
          this.done = true;
          this.alertService.success('Uploaded success');
          this._ref.detectChanges();
        } else {
          if (res.data) {
            this.errors = res.data;
          }
          this.loading = false;
          this.alertService.error(res.message)
          this._ref.detectChanges();
        }

      }, err => {
        this.alertService.exception();
        this.loading = false;
        this._ref.detectChanges();
      });

  }

  getRowNumber(i: number): number {
    const rowNumber = (this.paginator.pageIndex * this.paginator.pageSize) + (i+1);
    return this.formatInt(rowNumber , false)
  }
  
}
