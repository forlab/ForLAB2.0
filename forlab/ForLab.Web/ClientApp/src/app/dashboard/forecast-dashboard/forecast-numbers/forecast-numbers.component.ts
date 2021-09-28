import { Component, ElementRef, OnInit, ViewChild, Injector } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { fromEvent, Observable } from 'rxjs';
import { SelectionModel } from '@angular/cdk/collections';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { TableColumn } from 'src/@core/models/common/response';
import { DashboardController } from 'src/@core/APIs/DashboardController';
import { ObjectDataSource } from 'src/@core/services/object.datasource';
import { takeUntil, tap, debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { BaseService } from 'src/@core/services/base.service';
import { CountriesController } from 'src/@core/APIs/CountriesController';
import { RegionsController } from 'src/@core/APIs/RegionsController';

@Component({
  selector: 'app-forecast-numbers',
  templateUrl: './forecast-numbers.component.html',
  styleUrls: ['./forecast-numbers.component.scss']
})
export class ForecastNumbersComponent extends BaseService implements OnInit {

  // Table
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort;
  @ViewChild('searchInput', { static: false }) searchInput: ElementRef;
  columns: TableColumn<any>[] = [
    { label: 'Region Name', property: 'name', type: 'text', visible: true },
    { label: 'Number of Forecasts', property: 'numberOfForecasts', type: 'int', visible: true },
  ];
  dataSource: ObjectDataSource;
  selection = new SelectionModel<any>(true, []);
  // Drp
  countries$: Observable<any[]>;
  regions$: Observable<any[]>;
  // Vars
  countryId: number;
  regionId: number;


  constructor(public injector: Injector) {
    super(injector);
  }


  ngOnInit() {
    // Load Drp
    this.loadCountries();

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

  loadRegions() {
    let params: QueryParamsDto[] = [
      { key: 'countryId', value: this.countryId },
    ];
    this.regions$ = this.httpService.GET(RegionsController.GetAllAsDrp, params).pipe(map(res => res.data));
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
      { key: 'name', value: this.searchInput.nativeElement.value },
      { key: 'countryId', value: this.countryId },
      { key: 'regionId', value: this.regionId },
    ];

    this.dataSource.loadObjects(DashboardController.NumberOfForecasts, params);
    // Update lable
    this.columns[0].label = this.regionId ? 'Laboratory Name' : 'Region Name';
    this._ref.detectChanges()
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

}
