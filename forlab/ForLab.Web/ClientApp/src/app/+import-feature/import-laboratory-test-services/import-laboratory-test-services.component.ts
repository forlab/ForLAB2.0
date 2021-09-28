import { Component, OnInit, ViewChild, Injector, ViewEncapsulation, Input, ChangeDetectionStrategy } from '@angular/core';
import { TableColumn } from 'src/@core/models/common/response';
import { SelectionModel } from '@angular/cdk/collections';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { BaseService } from 'src/@core/services/base.service';
import { MatPaginator } from '@angular/material/paginator';
import { LaboratoryTestServiceDto } from 'src/@core/models/laboratory/LaboratoryTestService';
import { LaboratoryTestServicesController } from 'src/@core/APIs/LaboratoryTestServicesController';
import { map, takeUntil } from 'rxjs/operators';
import { LaboratoryDto } from 'src/@core/models/lookup/Laboratory';
import { TestDto } from 'src/@core/models/testing/Test';
import { LaboratoriesController } from 'src/@core/APIs/LaboratoriesController';
import { TestsController } from 'src/@core/APIs/TestsController';
import * as moment from 'moment';
import { CountryDto } from 'src/@core/models/lookup/Country';
import { RegionDto } from 'src/@core/models/lookup/Region';
import { RegionsController } from 'src/@core/APIs/RegionsController';
import { CountriesController } from 'src/@core/APIs/CountriesController';
import { FormValidationError } from 'src/@core/models/common/SharedModel';
import { TableVirtualScrollDataSource } from 'ng-table-virtual-scroll';
import { combineLatest } from 'rxjs';

@Component({
  selector: 'import-laboratory-test-services',
  templateUrl: './import-laboratory-test-services.component.html',
  styleUrls: ['./import-laboratory-test-services.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ImportLaboratoryTestServicesComponent extends BaseService implements OnInit {
  // Drp
  countries: CountryDto[] = [];
  regions: RegionDto[] = [];
  laboratories: LaboratoryDto[] = [];
  tests: TestDto[] = [];
  // Table
  @Input('data') data: LaboratoryTestServiceDto[] = [];
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  columns: TableColumn<LaboratoryTestServiceDto>[] = [
    { label: 'Actions', property: 'actions', type: 'button', visible: true },
    { label: '#', property: 'index', type: 'custom', visible: true },
    { label: 'Country', property: 'laboratoryRegionCountryId', type: 'select', visible: true },
    { label: 'Region', property: 'laboratoryRegionId', type: 'select', visible: true },
    { label: 'Laboratory', property: 'laboratoryId', type: 'select', visible: true },
    { label: 'Test', property: 'testId', type: 'select', visible: true },
  ];
  dataSource = new TableVirtualScrollDataSource();
  selection = new SelectionModel<LaboratoryTestServiceDto>(true, []);
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
    this.durations = Object.keys(this.data[0])?.filter(x => x != 'countryName' && x != 'regionName' && x != 'laboratoryName' && x != 'testName');
    this.durations?.forEach(x => {
      this.columns.push({ label: x, property: x, type: 'number', visible: true });
    });

    combineLatest([this.loadCountries(), this.loadRegions(), this.loadLaboratories(), this.loadTests()])
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(([countries, regions, laboratories, tests]) => {
        // Set Drp data
        this.countries = countries;
        this.regions = regions;
        this.laboratories = laboratories;
        this.tests = tests;

        // Patch the Table Data
        this.formArry = Array.from(Array(this.data.length), (_, i) => i + 1).map(x => {
          let form = this.appendObject();

          // Country
          this.data[x - 1].laboratoryRegionCountryId = countries.find(item => item.name.trim().toLowerCase() == this.data[x - 1].countryName.trim().toLowerCase())?.id;
          // Region
          this.data[x - 1].laboratoryRegionId = regions.find(item => item.countryId == this.data[x - 1].laboratoryRegionCountryId && item.name.trim().toLowerCase() == this.data[x - 1].regionName.trim().toLowerCase())?.id;
          // Laboratory
          this.data[x - 1].laboratoryId = laboratories.find(item => item.regionId == this.data[x - 1].laboratoryRegionId && item.name.trim().toLowerCase() == this.data[x - 1].laboratoryName.trim().toLowerCase())?.id;
          // Test
          this.data[x - 1].testId = tests.find(item => item.name.trim().toLowerCase() == this.data[x - 1].testName.trim().toLowerCase())?.id;

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
  loadTests() {
    return this.httpService.GET(TestsController.GetAllAsDrp).pipe(map(res => res.data));
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
      laboratoryRegionCountryId: new FormControl(null, [Validators.required]),
      laboratoryRegionId: new FormControl(null, [Validators.required]),
      laboratoryId: new FormControl(null, [Validators.required]),
      testId: new FormControl(null, [Validators.required]),
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
    let objectDtos: LaboratoryTestServiceDto[] = [];

    this.formArry.forEach(element => {
      // Add durations
      this.durations?.forEach(x => {
        let val = new LaboratoryTestServiceDto();
        val.laboratoryId = Number(element.value.laboratoryId);
        val.testId = Number(element.value.testId);
        val.serviceDuration = moment(x, 'MMM-YY').add(1, 'd').toISOString();
        val.testPerformed = element.value[x] ? Number(element.value[x]) : 0;
        objectDtos.push(val);
      });
    });

    this.httpService.POST(LaboratoryTestServicesController.ImportLaboratoryTestServices, objectDtos)
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
    const rowNumber = (this.paginator.pageIndex * this.paginator.pageSize) + (i+1);
    return this.formatInt(rowNumber , false)
  }

}
