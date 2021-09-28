import { Component, ElementRef, OnInit, ViewChild, Injector, Input } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { fromEvent, Observable } from 'rxjs';
import { SelectionModel } from '@angular/cdk/collections';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { ConfirmDeleteComponent } from 'src/@core/directives/confirm-delete/confirm-delete.component';
import { LaboratoryTestServiceDto, LaboratoryTestServiceFilterDto } from 'src/@core/models/laboratory/LaboratoryTestService';
import { TableColumn } from 'src/@core/models/common/response';
import { LaboratoryTestServicesController } from 'src/@core/APIs/LaboratoryTestServicesController';
import { ObjectDataSource } from 'src/@core/services/object.datasource';
import { takeUntil, tap, debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { ConfirmActiveComponent } from 'src/@core/directives/confirm-active/confirm-active.component';
import { BaseService } from 'src/@core/services/base.service';
import { AddEditLaboratoryTestServiceComponent } from '../add-edit-laboratory-test-service/add-edit-laboratory-test-service.component';
import { LaboratoriesController } from 'src/@core/APIs/LaboratoriesController';
import { TestsController } from 'src/@core/APIs/TestsController';
import { ConfirmExportComponent } from 'src/@core/directives/confirm-export/confirm-export.component';

@Component({
  selector: 'app-laboratory-test-services',
  templateUrl: './laboratory-test-services.component.html',
  styleUrls: ['./laboratory-test-services.component.scss']
})
export class LaboratoryTestServicesComponent extends BaseService implements OnInit {
  // Inputs to reuse the component in other components
  @Input('fromLaboratoryDetails') fromLaboratoryDetails: boolean = false;
  @Input('laboratoryId') laboratoryId: number;
  @Input('laboratoryCreatedBy') laboratoryCreatedBy: number;

  // Drp
  laboratories$: Observable<any[]>;
  tests$: Observable<any[]>;

  filterDto: LaboratoryTestServiceFilterDto = new LaboratoryTestServiceFilterDto();

  // Table
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort;
  @ViewChild('searchInput', { static: false }) searchInput: ElementRef;
  columns: TableColumn<LaboratoryTestServiceDto>[] = [
    { label: 'Actions', property: 'actions', type: 'button', visible: true },
    // { label: 'Select', property: 'select', type: 'button', visible: false },
    { label: 'Status', property: 'isActive', type: 'button', visible: true },
    { label: 'Region', property: 'laboratoryRegionName', type: 'text', visible: true },
    { label: 'Laboratory', property: 'laboratoryName', type: 'text', visible: true },
    { label: 'Test', property: 'testName', type: 'text', visible: true },
    { label: 'Duration', property: 'serviceDuration', type: 'date', visible: true },
    { label: 'Test Performed', property: 'testPerformed', type: 'number', visible: true },
    { label: 'Creator', property: 'creator', type: 'text', visible: false },
    { label: 'Created On', property: 'createdOn', type: 'datetime', visible: false },
  ];
  dataSource: ObjectDataSource;
  selection = new SelectionModel<LaboratoryTestServiceDto>(true, []);
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
    this.loadTests();

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
  loadTests() {
    this.tests$ = this.httpService.GET(TestsController.GetAllAsDrp).pipe(map(res => res.data));
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
      { key: 'testPerformed', value: this.searchInput.nativeElement.value || this.filterDto.testPerformed },
    ];

    // Append FilterDto values to the paramaters
    params = params.concat(this.getFilterParamsDtos(this.filterDto));

    this.dataSource.loadObjects(LaboratoryTestServicesController.GetAll, params);
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
    this.dialog.open(AddEditLaboratoryTestServiceComponent, {
      data: {
        laboratoryId: this.laboratoryId,
        fromLaboratoryDetails: this.fromLaboratoryDetails,
        laboratoryTestServiceDto: new LaboratoryTestServiceDto()
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

  updateObject(laboratoryTestService: LaboratoryTestServiceDto) {
    this.dialog.open(AddEditLaboratoryTestServiceComponent, {
      data: {
        laboratoryId: this.laboratoryId,
        fromLaboratoryDetails: this.fromLaboratoryDetails,
        laboratoryTestServiceDto: laboratoryTestService
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

  updateIsActive(laboratoryTestService: LaboratoryTestServiceDto) {
    this.dialog.open(ConfirmActiveComponent, {
      data: {
        url: LaboratoryTestServicesController.UpdateIsActive,
        objectInfo: [{ key: 'Laboratory', value: laboratoryTestService.laboratoryName }, { key: 'Test', value: laboratoryTestService.testName }],
        isActive: !laboratoryTestService.isActive,
        queryParamsDto: { key: 'laboratoryTestServiceId', value: laboratoryTestService.id },
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

  deleteObject(laboratoryTestService: LaboratoryTestServiceDto) {
    this.dialog.open(ConfirmDeleteComponent, {
      data: {
        url: LaboratoryTestServicesController.RemoveLaboratoryTestService,
        objectInfo: [{ key: 'Laboratory', value: laboratoryTestService.laboratoryName }, { key: 'Test', value: laboratoryTestService.testName }],
        queryParamsDto: { key: 'laboratoryTestServiceId', value: laboratoryTestService.id },
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
      { key: 'testPerformed', value: this.searchInput.nativeElement.value || this.filterDto.testPerformed },
    ];

    // Append FilterDto values to the paramaters
    params = params.concat(this.getFilterParamsDtos(this.filterDto));

    this.dialog.open(ConfirmExportComponent, {
      data: {
        url: LaboratoryTestServicesController.ExportLaboratoryTestServices,
        fileName: 'LaboratoryTestServices',
        queryParamsDtos: params,
      }
    });
  }

}
