import { Component, OnInit, ViewChild, Injector, ViewEncapsulation, Input, ChangeDetectionStrategy } from '@angular/core';
import { TableColumn } from 'src/@core/models/common/response';
import { SelectionModel } from '@angular/cdk/collections';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { BaseService } from 'src/@core/services/base.service';
import { MatPaginator } from '@angular/material/paginator';
import { LaboratoryDto } from 'src/@core/models/lookup/Laboratory';
import { CountryDto } from 'src/@core/models/lookup/Country';
import { LaboratoriesController } from 'src/@core/APIs/LaboratoriesController';
import { map, takeUntil } from 'rxjs/operators';
import { LaboratoryCategoryDto } from 'src/@core/models/lookup/LaboratoryCategory';
import { LaboratoryLevelDto } from 'src/@core/models/lookup/LaboratoryLevel';
import { RegionsController } from 'src/@core/APIs/RegionsController';
import { LaboratoryCategoriesController } from 'src/@core/APIs/LaboratoryCategoriesController';
import { LaboratoryLevelsController } from 'src/@core/APIs/LaboratoryLevelsController';
import { RegionDto } from 'src/@core/models/lookup/Region';
import { CountriesController } from 'src/@core/APIs/CountriesController';
import { FormValidationError } from 'src/@core/models/common/SharedModel';
import { TableVirtualScrollDataSource } from 'ng-table-virtual-scroll';
import { combineLatest } from 'rxjs';

@Component({
  selector: 'import-laboratories',
  templateUrl: './import-laboratories.component.html',
  styleUrls: ['./import-laboratories.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ImportLaboratoriesComponent extends BaseService implements OnInit {
  // Drp
  countries: CountryDto[] = [];
  regions: RegionDto[] = [];
  laboratoryCategories: LaboratoryCategoryDto[] = [];
  laboratoryLevels: LaboratoryLevelDto[] = [];
  // Table
  @Input('data') data: LaboratoryDto[] = [];
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  columns: TableColumn<LaboratoryDto>[] = [
    { label: 'Actions', property: 'actions', type: 'button', visible: true },
    { label: '#', property: 'index', type: 'custom', visible: true },
    { label: 'Name', property: 'name', type: 'text', visible: true },
    { label: 'Country', property: 'regionCountryId', type: 'select', visible: true },
    { label: 'Region', property: 'regionId', type: 'select', visible: true },
    { label: 'Category', property: 'laboratoryCategoryId', type: 'select', visible: true },
    { label: 'Level', property: 'laboratoryLevelId', type: 'select', visible: true },
    { label: 'Latitude', property: 'latitude', type: 'text', visible: true },
    { label: 'Longitude', property: 'longitude', type: 'text', visible: true },
  ];
  dataSource = new TableVirtualScrollDataSource();
  selection = new SelectionModel<LaboratoryDto>(true, []);
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

    combineLatest([this.loadCountries(), this.loadRegions(), this.loadLaboratoryCategories(), this.loadLaboratoryLevels()])
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(([countries, regions, laboratoryCategories, laboratoryLevels]) => {
        // Set Drp data
        this.countries = countries;
        this.regions = regions;
        this.laboratoryCategories = laboratoryCategories;
        this.laboratoryLevels = laboratoryLevels;

        // Patch the Table Data
        this.formArry = Array.from(Array(this.data.length), (_, i) => i + 1).map(x => {
          let form = this.appendObject();

          // Country
          this.data[x - 1].regionCountryId = countries.find(item => item.name.trim().toLowerCase() == this.data[x - 1].regionCountryName.trim().toLowerCase())?.id;
          // Region
          this.data[x - 1].regionId = regions.find(item => item.countryId == this.data[x - 1].regionCountryId && item.name.trim().toLowerCase() == this.data[x - 1].regionName.trim().toLowerCase())?.id;
          // LaboratoryCategory
          this.data[x - 1].laboratoryCategoryId = laboratoryCategories.find(item => item.name.trim().toLowerCase() == this.data[x - 1].laboratoryCategoryName.trim().toLowerCase())?.id;
          // LaboratoryLevel
          this.data[x - 1].laboratoryLevelId = laboratoryLevels.find(item => item.name.trim().toLowerCase() == this.data[x - 1].laboratoryLevelName.trim().toLowerCase())?.id;

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
  loadLaboratoryCategories() {
    return this.httpService.GET(LaboratoryCategoriesController.GetAllAsDrp).pipe(map(res => res.data));
  }
  loadLaboratoryLevels() {
    return this.httpService.GET(LaboratoryLevelsController.GetAllAsDrp).pipe(map(res => res.data));
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
      name: [null, Validators.compose([Validators.required])],
      regionCountryId: [null, Validators.compose([Validators.required])],
      regionId: [null, Validators.compose([Validators.required])],
      laboratoryCategoryId: [null, Validators.compose([Validators.required])],
      laboratoryLevelId: [null, Validators.compose([Validators.required])],
      latitude: [null, Validators.compose([Validators.required])],
      longitude: [null, Validators.compose([Validators.required])],
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
    let objectDtos: LaboratoryDto[] = [];

    objectDtos = this.formArry.map(element => {
      let val = new LaboratoryDto();
      val.name = String(element.value.name);
      val.regionId = Number(element.value.regionId);
      val.laboratoryCategoryId = Number(element.value.laboratoryCategoryId);
      val.laboratoryLevelId = Number(element.value.laboratoryLevelId);
      val.latitude = String(element.value.latitude);
      val.longitude = String(element.value.longitude);
      return val;
    });

    this.httpService.POST(LaboratoriesController.ImportLaboratories, objectDtos)
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
  
  getRowNumber(i: number): number {
    const rowNumber = (this.paginator.pageIndex * this.paginator.pageSize) + (i+1);
    return this.formatInt(rowNumber , false)
  }

}
