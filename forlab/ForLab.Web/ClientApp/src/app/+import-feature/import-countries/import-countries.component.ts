import { Component, OnInit, ViewChild, Injector, ViewEncapsulation, Input, ChangeDetectionStrategy } from '@angular/core';
import { TableColumn } from 'src/@core/models/common/response';
import { SelectionModel } from '@angular/cdk/collections';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { BaseService } from 'src/@core/services/base.service';
import { MatPaginator } from '@angular/material/paginator';
import { CountryDto } from 'src/@core/models/lookup/Country';
import { CountriesController } from 'src/@core/APIs/CountriesController';
import { takeUntil } from 'rxjs/operators';
import { RxwebValidators } from '@rxweb/reactive-form-validators';
import { ContinentEnum, CountryPeriodEnum } from 'src/@core/models/enum/Enums';
import { FormValidationError } from 'src/@core/models/common/SharedModel';
import { TableVirtualScrollDataSource } from 'ng-table-virtual-scroll';

@Component({
  selector: 'import-countries',
  templateUrl: './import-countries.component.html',
  styleUrls: ['./import-countries.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ImportCountriesComponent extends BaseService implements OnInit {
  // Drp
  continentEnum = ContinentEnum;
  countryPeriodEnum = CountryPeriodEnum;
  // Table
  @Input('data') data: CountryDto[] = [];
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  columns: TableColumn<CountryDto>[] = [
    { label: 'Actions', property: 'actions', type: 'button', visible: true },
    { label: '#', property: 'index', type: 'custom', visible: true },
    { label: 'Name', property: 'name', type: 'text', visible: true },
    { label: 'Short Name', property: 'shortName', type: 'text', visible: true },
    { label: 'Continent', property: 'continentId', type: 'select', visible: true },
    { label: 'Period', property: 'countryPeriodId', type: 'select', visible: true },
    { label: 'Short Code', property: 'shortCode', type: 'text', visible: true },
    { label: 'Native Name', property: 'nativeName', type: 'text', visible: true },
    { label: 'Currency Code', property: 'currencyCode', type: 'text', visible: true },
    { label: 'Calling Code', property: 'callingCode', type: 'text', visible: true },
    { label: 'Population', property: 'population', type: 'number', visible: true },
    { label: 'Latitude', property: 'latitude', type: 'text', visible: true },
    { label: 'Longitude', property: 'longitude', type: 'text', visible: true },
  ];
  dataSource = new TableVirtualScrollDataSource();
  selection = new SelectionModel<CountryDto>(true, []);
  // Form
  formArry: FormGroup[];
  // vars
  loadingData: boolean = true;
  errors: FormValidationError[] = [];
  successRes: any;
  done: boolean = false;

  constructor(private fb: FormBuilder, public injector: Injector) {
    super(injector);
  }

  ngOnInit() {
    // Convert Enum to List
    const continents = this.convertEnumToList(this.continentEnum);
    const countryPeriods = this.convertEnumToList(this.countryPeriodEnum);

    // Patch the Table Data
    this.formArry = Array.from(Array(this.data.length), (_, i) => i + 1).map(x => {
      let form = this.appendObject();

      // Continent
      this.data[x - 1].continentId = continents.find(item => item.name.trim().toLowerCase() == this.data[x - 1].continentName.trim().toLowerCase())?.id;
      // CountryPeriod
      this.data[x - 1].countryPeriodId = countryPeriods.find(item => item.name.trim().toLowerCase() == this.data[x - 1].countryPeriodName.trim().toLowerCase())?.id;

      // Patch FormGroup Value
      form.patchValue(this.data[x - 1]);
      return form;
    });

    this.loadingData = false;
    this.dataSource = new TableVirtualScrollDataSource(this.formArry);
    setTimeout(() => { this.dataSource.paginator = this.paginator; })
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
    return this.fb.group({
      name: [null, Validators.compose([Validators.required, RxwebValidators.unique()])],
      continentId: [null, Validators.compose([Validators.required])],
      countryPeriodId: [null, Validators.compose([Validators.required])],
      shortCode: [null, Validators.compose([Validators.required, RxwebValidators.unique()])],
      shortName: [null, Validators.compose([Validators.required])],
      nativeName: [null, Validators.compose([Validators.required])],
      currencyCode: [null, Validators.compose([Validators.required])],
      callingCode: [null, Validators.compose([Validators.required])],
      latitude: [null, Validators.compose([Validators.required])],
      longitude: [null, Validators.compose([Validators.required])],
      population: [null, Validators.compose([])],
    })
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
    let objectDtos: CountryDto[] = [];

    objectDtos = this.formArry.map(element => {
      let val = new CountryDto();
      val.name = String(element.value.name);
      val.continentId = Number(element.value.continentId);
      val.countryPeriodId = Number(element.value.countryPeriodId);
      val.shortCode = String(element.value.shortCode);
      val.shortName = String(element.value.shortName);
      val.nativeName = String(element.value.nativeName);
      val.currencyCode = String(element.value.currencyCode);
      val.callingCode = String(element.value.callingCode);
      val.latitude = String(element.value.latitude);
      val.longitude = String(element.value.longitude);
      val.population = Number(element.value.population) || 0;
      return val;
    });

    this.httpService.POST(CountriesController.ImportCountries, objectDtos)
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
    const rowNumber = (this.paginator.pageIndex * this.paginator.pageSize) + (i + 1);
    return this.formatInt(rowNumber, false)
  }

}
