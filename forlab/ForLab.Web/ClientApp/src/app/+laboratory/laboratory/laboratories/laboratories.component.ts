import { Component, ElementRef, OnInit, ViewChild, Injector } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { fromEvent } from 'rxjs';
import { SelectionModel } from '@angular/cdk/collections';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { ConfirmDeleteComponent } from 'src/@core/directives/confirm-delete/confirm-delete.component';
import { LaboratoryDto, LaboratoryFilterDto } from 'src/@core/models/lookup/Laboratory';
import { TableColumn } from 'src/@core/models/common/response';
import { LaboratoriesController } from 'src/@core/APIs/LaboratoriesController';
import { ObjectDataSource } from 'src/@core/services/object.datasource';
import { takeUntil, tap, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ConfirmActiveComponent } from 'src/@core/directives/confirm-active/confirm-active.component';
import { BaseService } from 'src/@core/services/base.service';
import { ConfirmExportComponent } from 'src/@core/directives/confirm-export/confirm-export.component';
import { ConfirmActiveSelectedComponent } from 'src/@core/directives/confirm-active-selected/confirm-active-selected.component';

@Component({
  selector: 'app-laboratories',
  templateUrl: './laboratories.component.html',
  styleUrls: ['./laboratories.component.sass']
})
export class LaboratoriesComponent extends BaseService implements OnInit {

  filterDto: LaboratoryFilterDto = new LaboratoryFilterDto();

  // Table
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort;
  @ViewChild('searchInput', { static: false }) searchInput: ElementRef;
  columns: TableColumn<LaboratoryDto>[] = [
    { label: 'Select', property: 'select', type: 'button', visible: true },
    { label: 'Actions', property: 'actions', type: 'button', visible: true },
    { label: 'Status', property: 'isActive', type: 'button', visible: true },
    { label: 'Shared', property: 'shared', type: 'bool', visible: true },
    { label: 'Name', property: 'name', type: 'text', visible: true },
    { label: 'Region', property: 'regionName', type: 'text', visible: true },
    { label: 'Category', property: 'laboratoryCategoryName', type: 'text', visible: true },
    { label: 'Level', property: 'laboratoryLevelName', type: 'text', visible: true },
    { label: 'Latitude', property: 'latitude', type: 'text', visible: true },
    { label: 'Longitude', property: 'longitude', type: 'text', visible: true },
    { label: 'Creator', property: 'creator', type: 'text', visible: false },
    { label: 'Created On', property: 'createdOn', type: 'datetime', visible: false },
  ];
  dataSource: ObjectDataSource;
  selection = new SelectionModel<LaboratoryDto>(true, []);

  constructor(public injector: Injector) {
    super(injector);

    // Hide Actions and Select columns
    const isSuperAdmin = this.permissionsService.getPermission('SuperAdmin');
    if (!isSuperAdmin) {
      this.columns = this.columns.filter(x => x.property != 'shared');
    }
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

    this.dataSource.loadObjects(LaboratoriesController.GetAll, params);
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

  updateIsActive(laboratory: LaboratoryDto) {
    this.dialog.open(ConfirmActiveComponent, {
      data: {
        url: LaboratoriesController.UpdateIsActive,
        objectInfo: [{ key: 'Name', value: laboratory.name }, { key: 'Region', value: laboratory.regionName }],
        isActive: !laboratory.isActive,
        queryParamsDto: { key: 'laboratoryId', value: laboratory.id },
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

  deleteObject(laboratory: LaboratoryDto) {
    this.dialog.open(ConfirmDeleteComponent, {
      data: {
        url: LaboratoriesController.RemoveLaboratory,
        objectInfo: [{ key: 'Name', value: laboratory.name }, { key: 'Region', value: laboratory.regionName }],
        queryParamsDto: { key: 'laboratoryId', value: laboratory.id },
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
        url: LaboratoriesController.ExportLaboratories,
        fileName: 'Laboratories',
        queryParamsDtos: params,
      }
    });
  }

  changeIsActiveForSelected(isActive: boolean) {

    this.dialog.open(ConfirmActiveSelectedComponent, {
      data: {
        url: LaboratoriesController.UpdateIsActiveForSelected,
        ids: this.selection.selected.filter(x => x.createdBy == this.loggedInUser?.id || this.isSuperAdmin).map(x => x.id),
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

  changeSharedForSelected(shared: boolean) {

    this.dialog.open(ConfirmActiveSelectedComponent, {
      data: {
        url: LaboratoriesController.UpdateSharedForSelected,
        ids: this.selection.selected.filter(x => x.createdBy == this.loggedInUser?.id || this.isSuperAdmin).map(x => x.id),
        shared: shared,
        type: 'shared'
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
