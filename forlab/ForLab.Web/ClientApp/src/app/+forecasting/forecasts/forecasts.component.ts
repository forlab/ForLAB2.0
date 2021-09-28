import { Component, ElementRef, OnInit, ViewChild, Injector } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { fromEvent } from 'rxjs';
import { SelectionModel } from '@angular/cdk/collections';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { ConfirmDeleteComponent } from 'src/@core/directives/confirm-delete/confirm-delete.component';
import { ForecastInfoDto, ForecastInfoFilterDto } from 'src/@core/models/forecasting/ForecastInfo';
import { TableColumn } from 'src/@core/models/common/response';
import { ForecastInfosController } from 'src/@core/APIs/ForecastInfosController';
import { ObjectDataSource } from 'src/@core/services/object.datasource';
import { takeUntil, tap, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ConfirmActiveComponent } from 'src/@core/directives/confirm-active/confirm-active.component';
import { BaseService } from 'src/@core/services/base.service';
import { ConfirmExportComponent } from 'src/@core/directives/confirm-export/confirm-export.component';
import { ForecastInfoLevelEnum, ForecastMethodologyEnum, ScopeOfTheForecastEnum, ForecastInfoStatusEnum } from 'src/@core/models/enum/Enums';

@Component({
  selector: 'app-forecasts',
  templateUrl: './forecasts.component.html',
  styleUrls: ['./forecasts.component.scss']
})
export class ForecastsComponent extends BaseService implements OnInit {

  filterDto: ForecastInfoFilterDto = new ForecastInfoFilterDto();
  // Drp
  forecastInfoLevelEnum = ForecastInfoLevelEnum;
  forecastMethodologyEnum = ForecastMethodologyEnum;
  scopeOfTheForecastEnum = ScopeOfTheForecastEnum;
  forecastInfoStatusEnum = ForecastInfoStatusEnum;
  // Table
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort;
  @ViewChild('searchInput', { static: false }) searchInput: ElementRef;
  columns: TableColumn<ForecastInfoDto>[] = [
    { label: 'Actions', property: 'actions', type: 'button', visible: true },
    // { label: 'Select', property: 'select', type: 'button', visible: false },
    { label: 'Status', property: 'isActive', type: 'button', visible: true },
    { label: 'Name', property: 'name', type: 'text', visible: true },
    { label: 'Level', property: 'forecastInfoLevelName', type: 'text', visible: true },
    { label: 'Country', property: 'countryName', type: 'text', visible: false },
    { label: 'Methodology', property: 'forecastMethodologyName', type: 'custom', visible: true },
    { label: 'Scope', property: 'scopeOfTheForecastName', type: 'text', visible: true },
    { label: 'Start Date', property: 'startDate', type: 'date', visible: true },
    { label: 'End Date', property: 'endDate', type: 'date', visible: true },
    { label: 'Wastage Rate', property: 'wastageRate', type: 'percent', visible: true },
    { label: 'Creator', property: 'creator', type: 'text', visible: false },
    { label: 'Created On', property: 'createdOn', type: 'datetime', visible: false },
  ];
  dataSource: ObjectDataSource;
  selection = new SelectionModel<ForecastInfoDto>(true, []);

  constructor(public injector: Injector) {
    super(injector);
  }


  ngOnInit() {
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
      { key: 'name', value: this.searchInput.nativeElement.value || this.filterDto.name },
    ];

    // Append FilterDto values to the paramaters
    params = params.concat(this.getFilterParamsDtos(this.filterDto));

    this.dataSource.loadObjects(ForecastInfosController.GetAll, params);
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


  updateIsActive(forecastInfo: ForecastInfoDto) {
    this.dialog.open(ConfirmActiveComponent, {
      data: {
        url: ForecastInfosController.UpdateIsActive,
        objectInfo: [{ key: 'Name', value: forecastInfo.name }, { key: 'Level', value: forecastInfo.forecastInfoLevelName }, { key: 'Methodology', value: forecastInfo.forecastMethodologyName }],
        isActive: !forecastInfo.isActive,
        queryParamsDto: { key: 'forecastInfoId', value: forecastInfo.id },
        btnTitle: 'Close Forecast'
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

  deleteObject(forecastInfo: ForecastInfoDto) {
    this.dialog.open(ConfirmDeleteComponent, {
      data: {
        url: ForecastInfosController.RemoveForecastInfo,
        objectInfo: [{ key: 'Name', value: forecastInfo.name }, { key: 'Level', value: forecastInfo.forecastInfoLevelName }, { key: 'Methodology', value: forecastInfo.forecastMethodologyName }],
        queryParamsDto: { key: 'forecastInfoId', value: forecastInfo.id },
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
      { key: 'name', value: this.searchInput.nativeElement.value || this.filterDto.name },
    ];

    // Append FilterDto values to the paramaters
    params = params.concat(this.getFilterParamsDtos(this.filterDto));

    this.dialog.open(ConfirmExportComponent, {
      data: {
        url: ForecastInfosController.ExportForecastInfos,
        fileName: 'ForecastInfos',
        queryParamsDtos: params,
      }
    });
  }

}
