import { Component, OnInit, ViewChild, Injector, ViewEncapsulation, Input, ChangeDetectionStrategy } from '@angular/core';
import { TableColumn } from 'src/@core/models/common/response';
import { SelectionModel } from '@angular/cdk/collections';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { BaseService } from 'src/@core/services/base.service';
import { MatPaginator } from '@angular/material/paginator';
import { LaboratoryInstrumentDto } from 'src/@core/models/laboratory/LaboratoryInstrument';
import { LaboratoryInstrumentsController } from 'src/@core/APIs/LaboratoryInstrumentsController';
import { map, takeUntil } from 'rxjs/operators';
import { LaboratoryDto } from 'src/@core/models/lookup/Laboratory';
import { InstrumentDto } from 'src/@core/models/product/Instrument';
import { LaboratoriesController } from 'src/@core/APIs/LaboratoriesController';
import { InstrumentsController } from 'src/@core/APIs/InstrumentsController';
import { CountryDto } from 'src/@core/models/lookup/Country';
import { RegionDto } from 'src/@core/models/lookup/Region';
import { RegionsController } from 'src/@core/APIs/RegionsController';
import { CountriesController } from 'src/@core/APIs/CountriesController';
import { RxwebValidators } from '@rxweb/reactive-form-validators';
import { FormValidationError } from 'src/@core/models/common/SharedModel';
import { TableVirtualScrollDataSource } from 'ng-table-virtual-scroll';
import { combineLatest } from 'rxjs';

@Component({
  selector: 'import-laboratory-instruments',
  templateUrl: './import-laboratory-instruments.component.html',
  styleUrls: ['./import-laboratory-instruments.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ImportLaboratoryInstrumentsComponent extends BaseService implements OnInit {
  // Drp
  countries: CountryDto[] = [];
  regions: RegionDto[] = [];
  laboratories: LaboratoryDto[] = [];
  instruments: InstrumentDto[] = [];
  // Table
  @Input('data') data: LaboratoryInstrumentDto[] = [];
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  columns: TableColumn<LaboratoryInstrumentDto>[] = [
    { label: 'Actions', property: 'actions', type: 'button', visible: true },
    { label: '#', property: 'index', type: 'custom', visible: true },
    { label: 'Country', property: 'laboratoryRegionCountryId', type: 'select', visible: true },
    { label: 'Region', property: 'laboratoryRegionId', type: 'select', visible: true },
    { label: 'Laboratory', property: 'laboratoryId', type: 'select', visible: true },
    { label: 'Instrument', property: 'instrumentId', type: 'select', visible: true },
    { label: 'Quantity', property: 'quantity', type: 'number', visible: true },
    { label: 'Test Run Percentage', property: 'testRunPercentage', type: 'number', visible: true },
  ];
  dataSource = new TableVirtualScrollDataSource();
  selection = new SelectionModel<LaboratoryInstrumentDto>(true, []);
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

    combineLatest([this.loadCountries(), this.loadRegions(), this.loadLaboratories(), this.loadInstruments()])
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(([countries, regions, laboratories, instruments]) => {
        // Set Drp data
        this.countries = countries;
        this.regions = regions;
        this.laboratories = laboratories;
        this.instruments = instruments;

        // Patch the Table Data
        this.formArry = Array.from(Array(this.data.length), (_, i) => i + 1).map(x => {
          let form = this.appendObject();

          // Country
          this.data[x - 1].laboratoryRegionCountryId = countries.find(item => item.name.trim().toLowerCase() == this.data[x - 1].countryName.trim().toLowerCase())?.id;
          // Region
          this.data[x - 1].laboratoryRegionId = regions.find(item => item.countryId == this.data[x - 1].laboratoryRegionCountryId && item.name.trim().toLowerCase() == this.data[x - 1].regionName.trim().toLowerCase())?.id;
          // Laboratory
          this.data[x - 1].laboratoryId = laboratories.find(item => item.regionId == this.data[x - 1].laboratoryRegionId && item.name.trim().toLowerCase() == this.data[x - 1].laboratoryName.trim().toLowerCase())?.id;
          // Instrument
          this.data[x - 1].instrumentId = instruments.find(item => item.name.trim().toLowerCase() == this.data[x - 1].instrumentName.trim().toLowerCase())?.id;

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
  loadRegions() {
    return this.httpService.GET(RegionsController.GetAllAsDrp).pipe(map(res => res.data));
  }
  loadLaboratories() {
    return this.httpService.GET(LaboratoriesController.GetAllAsDrp).pipe(map(res => res.data));
  }
  loadInstruments() {
    return this.httpService.GET(InstrumentsController.GetAllAsDrp).pipe(map(res => res.data));
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
      laboratoryRegionCountryId: [null, Validators.compose([Validators.required])],
      laboratoryRegionId: [null, Validators.compose([Validators.required])],
      laboratoryId: [null, Validators.compose([Validators.required])],
      instrumentId: [null, Validators.compose([Validators.required])],
      quantity: [null, Validators.compose([Validators.required, RxwebValidators.minNumber({ value: 0 })])],
      testRunPercentage: [null, Validators.compose([Validators.required, RxwebValidators.minNumber({ value: 0 }), RxwebValidators.maxNumber({ value: 100 })])],
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
    let objectDtos: LaboratoryInstrumentDto[] = [];

    objectDtos = this.formArry.map(element => {
      let val = new LaboratoryInstrumentDto();
      val.laboratoryId = Number(element.value.laboratoryId);
      val.instrumentId = Number(element.value.instrumentId);
      val.quantity = Number(element.value.quantity);
      val.testRunPercentage = Number(element.value.testRunPercentage);
      return val;
    });

    this.httpService.POST(LaboratoryInstrumentsController.ImportLaboratoryInstruments, objectDtos)
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

  getFilteredRegions(countryId: number): RegionDto[] {
    return this.regions.filter(x => x.countryId == countryId);
  }
  getFilteredLabs(regionId: number): LaboratoryDto[] {
    return this.laboratories.filter(x => x.regionId == regionId);
  }

  getRowNumber(i: number): number {
    const rowNumber = (this.paginator.pageIndex * this.paginator.pageSize) + (i + 1);
    return this.formatInt(rowNumber, false)
  }

}
