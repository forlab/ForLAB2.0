import { Component, ElementRef, OnInit, ViewChild, Injector } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { fromEvent, Observable } from 'rxjs';
import { SelectionModel } from '@angular/cdk/collections';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { ConfirmDeleteComponent } from 'src/@core/directives/confirm-delete/confirm-delete.component';
import { CountryDiseaseIncidentDto, CountryDiseaseIncidentFilterDto } from 'src/@core/models/disease/CountryDiseaseIncident';
import { TableColumn } from 'src/@core/models/common/response';
import { CountryDiseaseIncidentsController } from 'src/@core/APIs/CountryDiseaseIncidentsController';
import { ObjectDataSource } from 'src/@core/services/object.datasource';
import { takeUntil, tap, debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { ConfirmActiveComponent } from 'src/@core/directives/confirm-active/confirm-active.component';
import { BaseService } from 'src/@core/services/base.service';
import { AddEditCountryDiseaseIncidentComponent } from '../add-edit-country-disease-incident/add-edit-country-disease-incident.component';
import { ConfirmExportComponent } from 'src/@core/directives/confirm-export/confirm-export.component';
import { CountriesController } from 'src/@core/APIs/CountriesController';
import { DiseasesController } from 'src/@core/APIs/DiseasesController';
import { ConfirmActiveSelectedComponent } from 'src/@core/directives/confirm-active-selected/confirm-active-selected.component';

@Component({
  selector: 'app-country-disease-incidents',
  templateUrl: './country-disease-incidents.component.html',
  styleUrls: ['./country-disease-incidents.component.scss']
})
export class CountryDiseaseIncidentsComponent extends BaseService implements OnInit {

  filterDto: CountryDiseaseIncidentFilterDto = new CountryDiseaseIncidentFilterDto();
  // Drp
  countries$: Observable<any[]>;
  diseases$: Observable<any[]>;

  // Table
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort;
  @ViewChild('searchInput', { static: false }) searchInput: ElementRef;
  columns: TableColumn<CountryDiseaseIncidentDto>[] = [
    { label: 'Select', property: 'select', type: 'button', visible: true },
    { label: 'Actions', property: 'actions', type: 'button', visible: true },
    { label: 'Status', property: 'isActive', type: 'button', visible: true },
    { label: 'Country', property: 'countryName', type: 'text', visible: true },
    { label: 'Disease', property: 'diseaseName', type: 'text', visible: true },
    { label: 'Year', property: 'year', type: 'text', visible: true },
    { label: 'Incidence', property: 'incidence', type: 'number', visible: true },
    { label: 'Per 1k Population', property: 'incidencePer1kPopulation', type: 'number', visible: true },
    { label: 'Per 100k Population', property: 'incidencePer100kPopulation', type: 'number', visible: true },
    { label: 'Prevalence Rate', property: 'prevalenceRate', type: 'number', visible: true },
    { label: 'Prevalence Rate per 1k Population', property: 'prevalenceRatePer1kPopulation', type: 'number', visible: true },
    { label: 'Prevalence Rate per 100k Population', property: 'prevalenceRatePer100kPopulation', type: 'number', visible: true },
    { label: 'Creator', property: 'creator', type: 'text', visible: false },
    { label: 'Created On', property: 'createdOn', type: 'datetime', visible: false },
  ];
  dataSource: ObjectDataSource;
  selection = new SelectionModel<CountryDiseaseIncidentDto>(true, []);

  constructor(public injector: Injector) {
    super(injector);

    // Hide Actions and Select columns
    const isSuperAdmin = this.permissionsService.getPermission('SuperAdmin');
    if (!isSuperAdmin) {
      this.columns = this.columns.filter(x => x.property != 'select' && x.property != 'actions');
    }
  }


  ngOnInit() {
    // Load Drp
    this.loadCountries();
    this.loadDiseases();

    this.dataSource = new ObjectDataSource(this.httpService);
  }

  ngAfterViewInit() {

    this.loadData();
    this.sort.sortChange.subscribe((res) => {
      this.paginator.pageIndex = 0;
      this.loadData();
    });

    this.paginator.page.pipe(tap(() => this.loadData())).subscribe();

    fromEvent(this.searchInput.nativeElement, 'keyup')
      .pipe(
        debounceTime(150),
        distinctUntilChanged(),
        tap(() => {
          this.paginator.pageIndex = 0;
          this.loadData();
        })
      )
      .subscribe();

  }

  loadCountries() {
    this.countries$ = this.httpService.GET(CountriesController.GetAllAsDrp).pipe(map(res => res.data));
  }
  loadDiseases() {
    this.diseases$ = this.httpService.GET(DiseasesController.GetAllAsDrp).pipe(map(res => res.data));
  }

  toggleColumnVisibility(column, event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    column.visible = !column.visible;
  }

  trackByProperty<T>(index: number, column: TableColumn<T>) {
    return column.property;
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  loadData() {

    this.selection.clear();

    let params: QueryParamsDto[] = [
      { key: 'pageSize', value: this.paginator.pageSize || 10 },
      { key: 'pageIndex', value: this.paginator.pageIndex + 1 || 1 },
      { key: 'applySort', value: this.sort && this.sort.active ? true : false },
      { key: 'sortProperty', value: this.sort && this.sort.active ? this.sort.active : null },
      { key: 'isAscending', value: this.sort && this.sort.direction == 'asc' ? true : false },
      // Filter
      { key: 'year', value: this.searchInput.nativeElement.value || this.filterDto.year },
    ];

    // Append FilterDto values to the paramaters
    params = params.concat(this.getFilterParamsDtos(this.filterDto));

    this.dataSource.loadObjects(CountryDiseaseIncidentsController.GetAll, params);
  }
  /** Whether the number of selected elements matches the total number of rows. */
  isAllSelected() {
    const numSelected = this.selection.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }
  /** Selects all rows if they are not all selected; otherwise clear selection. */
  masterToggle() {
    if (this.isAllSelected()) {
      this.selection.clear();
    } else {
      this.selection.clear();
      this.dataSource.data.forEach(row => this.selection.select(row));
    }
  }

  reset() {
    let skipped: string[] = [];

    if (this.filterDto != null) {
      Object.keys(this.filterDto).forEach(key => {
        if (!skipped.includes(key)) {
          this.filterDto[key] = null;
        }
      });

      // Reset page
      this.paginator.pageIndex = 0;
      // Reload the data
      this.loadData();
    }
  }

  createObject() {
    this.dialog.open(AddEditCountryDiseaseIncidentComponent)
      .afterClosed()
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res) {
          this.loadData();
        }
      });
  }

  updateObject(countryDiseaseIncident: CountryDiseaseIncidentDto) {
    this.dialog.open(AddEditCountryDiseaseIncidentComponent, {
      data: countryDiseaseIncident
    })
      .afterClosed()
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res) {
          this.loadData();
        }
      });
  }

  updateIsActive(countryDiseaseIncident: CountryDiseaseIncidentDto) {
    this.dialog.open(ConfirmActiveComponent, {
      data: {
        url: CountryDiseaseIncidentsController.UpdateIsActive,
        objectInfo: [{ key: 'Country', value: countryDiseaseIncident.countryName }, { key: 'Disease', value: countryDiseaseIncident.diseaseName }],
        isActive: !countryDiseaseIncident.isActive,
        queryParamsDto: { key: 'countryDiseaseIncidentId', value: countryDiseaseIncident.id },
      }
    })
      .afterClosed()
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res) {
          this.loadData();
        }
      });
  }

  deleteObject(countryDiseaseIncident: CountryDiseaseIncidentDto) {
    this.dialog.open(ConfirmDeleteComponent, {
      data: {
        url: CountryDiseaseIncidentsController.RemoveCountryDiseaseIncident,
        objectInfo: [{ key: 'Country', value: countryDiseaseIncident.countryName }, { key: 'Disease', value: countryDiseaseIncident.diseaseName }],
        queryParamsDto: { key: 'countryDiseaseIncidentId', value: countryDiseaseIncident.id },
      }
    })
      .afterClosed()
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res) {
          this.loadData();
        }
      });
  }

  export() {
    let params: QueryParamsDto[] = [
      { key: 'pageSize', value: this.paginator.pageSize || 10 },
      { key: 'pageIndex', value: this.paginator.pageIndex + 1 || 1 },
      { key: 'applySort', value: this.sort && this.sort.active ? true : false },
      { key: 'sortProperty', value: this.sort && this.sort.active ? this.sort.active : null },
      { key: 'isAscending', value: this.sort && this.sort.direction == 'asc' ? true : false },
      // Filter
      { key: 'year', value: this.searchInput.nativeElement.value || this.filterDto.year },
    ];

    // Append FilterDto values to the paramaters
    params = params.concat(this.getFilterParamsDtos(this.filterDto));

    this.dialog.open(ConfirmExportComponent, {
      data: {
        url: CountryDiseaseIncidentsController.ExportCountryDiseaseIncidents,
        fileName: 'CountryDiseaseIncidents',
        queryParamsDtos: params,
      }
    });
  }

  changeIsActiveForSelected(isActive: boolean) {

    this.dialog.open(ConfirmActiveSelectedComponent, {
      data: {
        url: CountryDiseaseIncidentsController.UpdateIsActiveForSelected,
        ids: this.selection.selected.map(x => x.id),
        isActive: isActive,
      }
    })
      .afterClosed()
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res) {
          this.loadData();
        }
      });
  }

}
