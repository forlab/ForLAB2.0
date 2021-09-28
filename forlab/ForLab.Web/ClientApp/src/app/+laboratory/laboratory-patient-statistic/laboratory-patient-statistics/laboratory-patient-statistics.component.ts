import { Component, ElementRef, OnInit, ViewChild, Injector, Input } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { fromEvent, Observable } from 'rxjs';
import { SelectionModel } from '@angular/cdk/collections';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { ConfirmDeleteComponent } from 'src/@core/directives/confirm-delete/confirm-delete.component';
import { LaboratoryPatientStatisticDto, LaboratoryPatientStatisticFilterDto } from 'src/@core/models/laboratory/LaboratoryPatientStatistic';
import { TableColumn } from 'src/@core/models/common/response';
import { LaboratoryPatientStatisticsController } from 'src/@core/APIs/LaboratoryPatientStatisticsController';
import { ObjectDataSource } from 'src/@core/services/object.datasource';
import { takeUntil, tap, debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { ConfirmActiveComponent } from 'src/@core/directives/confirm-active/confirm-active.component';
import { BaseService } from 'src/@core/services/base.service';
import { AddEditLaboratoryPatientStatisticComponent } from '../add-edit-laboratory-patient-statistic/add-edit-laboratory-patient-statistic.component';
import { LaboratoriesController } from 'src/@core/APIs/LaboratoriesController';
import { ConfirmExportComponent } from 'src/@core/directives/confirm-export/confirm-export.component';

@Component({
  selector: 'app-laboratory-patient-statistics',
  templateUrl: './laboratory-patient-statistics.component.html',
  styleUrls: ['./laboratory-patient-statistics.component.scss']
})
export class LaboratoryPatientStatisticsComponent extends BaseService implements OnInit {
  // Inputs to reuse the component in other components
  @Input('fromLaboratoryDetails') fromLaboratoryDetails: boolean = false;
  @Input('laboratoryId') laboratoryId: number;
  @Input('laboratoryCreatedBy') laboratoryCreatedBy: number;

  // Drp
  laboratories$: Observable<any[]>;

  filterDto: LaboratoryPatientStatisticFilterDto = new LaboratoryPatientStatisticFilterDto();

  // Table
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort;
  @ViewChild('searchInput', { static: false }) searchInput: ElementRef;
  columns: TableColumn<LaboratoryPatientStatisticDto>[] = [
    { label: 'Actions', property: 'actions', type: 'button', visible: true },
    // { label: 'Select', property: 'select', type: 'button', visible: false },
    { label: 'Status', property: 'isActive', type: 'button', visible: true },
    { label: 'Region', property: 'laboratoryRegionName', type: 'text', visible: true },
    { label: 'Laboratory', property: 'laboratoryName', type: 'text', visible: true },
    { label: 'Period', property: 'period', type: 'date', visible: true },
    { label: 'Count', property: 'count', type: 'int', visible: true },
    { label: 'Creator', property: 'creator', type: 'text', visible: false },
    { label: 'Created On', property: 'createdOn', type: 'datetime', visible: false },
  ];
  dataSource: ObjectDataSource;
  selection = new SelectionModel<LaboratoryPatientStatisticDto>(true, []);
  skippedOnReset: string[] = [];

  constructor(public injector: Injector) {
    super(injector);

  }

  ngOnInit() {

    // Check
    if (this.fromLaboratoryDetails) {
      this.columns = this.columns.filter(x => x.property != 'laboratoryRegionName' && x.property != 'laboratoryName');
      this.filterDto.laboratoryId = this.laboratoryId;
      this.skippedOnReset.push('laboratoryId');
    }

    // Load Drp
    if (!this.fromLaboratoryDetails) {
      this.loadLaboratories();
    }

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

  loadLaboratories() {
    this.laboratories$ = this.httpService.GET(LaboratoriesController.GetAllAsDrp).pipe(map(res => res.data));
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
      { key: 'count', value: this.searchInput.nativeElement.value || this.filterDto.count },
    ];

    // Append FilterDto values to the paramaters
    params = params.concat(this.getFilterParamsDtos(this.filterDto));

    this.dataSource.loadObjects(LaboratoryPatientStatisticsController.GetAll, params);
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
    if (this.filterDto != null) {
      Object.keys(this.filterDto).forEach(key => {
        if (!this.skippedOnReset.includes(key)) {
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
    this.dialog.open(AddEditLaboratoryPatientStatisticComponent, {
      data: {
        laboratoryId: this.laboratoryId,
        fromLaboratoryDetails: this.fromLaboratoryDetails,
        laboratoryPatientStatisticDto: new LaboratoryPatientStatisticDto()
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

  updateObject(laboratoryPatientStatistic: LaboratoryPatientStatisticDto) {
    this.dialog.open(AddEditLaboratoryPatientStatisticComponent, {
      data: {
        laboratoryId: this.laboratoryId,
        fromLaboratoryDetails: this.fromLaboratoryDetails,
        laboratoryPatientStatisticDto: laboratoryPatientStatistic
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

  updateIsActive(laboratoryPatientStatistic: LaboratoryPatientStatisticDto) {
    this.dialog.open(ConfirmActiveComponent, {
      data: {
        url: LaboratoryPatientStatisticsController.UpdateIsActive,
        objectInfo: [{ key: 'Laboratory', value: laboratoryPatientStatistic.laboratoryName }, { key: 'Period', value: laboratoryPatientStatistic.period }],
        isActive: !laboratoryPatientStatistic.isActive,
        queryParamsDto: { key: 'laboratoryPatientStatisticId', value: laboratoryPatientStatistic.id },
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

  deleteObject(laboratoryPatientStatistic: LaboratoryPatientStatisticDto) {
    this.dialog.open(ConfirmDeleteComponent, {
      data: {
        url: LaboratoryPatientStatisticsController.RemoveLaboratoryPatientStatistic,
        objectInfo: [{ key: 'Laboratory', value: laboratoryPatientStatistic.laboratoryName }, { key: 'Period', value: laboratoryPatientStatistic.period }],
        queryParamsDto: { key: 'laboratoryPatientStatisticId', value: laboratoryPatientStatistic.id },
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
      { key: 'count', value: this.searchInput.nativeElement.value || this.filterDto.count },
    ];

    // Append FilterDto values to the paramaters
    params = params.concat(this.getFilterParamsDtos(this.filterDto));

    this.dialog.open(ConfirmExportComponent, {
      data: {
        url: LaboratoryPatientStatisticsController.ExportLaboratoryPatientStatistics,
        fileName: 'LaboratoryPatientStatistics',
        queryParamsDtos: params,
      }
    });
  }

}
