import { Component, ElementRef, OnInit, ViewChild, Injector } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { fromEvent } from 'rxjs';
import { SelectionModel } from '@angular/cdk/collections';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { ConfirmDeleteComponent } from 'src/@core/directives/confirm-delete/confirm-delete.component';
import { LaboratoryCategoryDto, LaboratoryCategoryFilterDto } from 'src/@core/models/lookup/LaboratoryCategory';
import { TableColumn } from 'src/@core/models/common/response';
import { LaboratoryCategoriesController } from 'src/@core/APIs/LaboratoryCategoriesController';
import { ObjectDataSource } from 'src/@core/services/object.datasource';
import { takeUntil, tap, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ConfirmActiveComponent } from 'src/@core/directives/confirm-active/confirm-active.component';
import { BaseService } from 'src/@core/services/base.service';
import { AddEditLaboratoryCategoryComponent } from '../add-edit-laboratory-category/add-edit-laboratory-category.component';
import { ConfirmExportComponent } from 'src/@core/directives/confirm-export/confirm-export.component';
import { ConfirmActiveSelectedComponent } from 'src/@core/directives/confirm-active-selected/confirm-active-selected.component';

@Component({
  selector: 'app-laboratory-categories',
  templateUrl: './laboratory-categories.component.html',
  styleUrls: ['./laboratory-categories.component.sass']
})
export class LaboratoryCategoriesComponent extends BaseService implements OnInit {

  filterDto: LaboratoryCategoryFilterDto = new LaboratoryCategoryFilterDto();

  // Table
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort;
  @ViewChild('searchInput', { static: false }) searchInput: ElementRef;
  columns: TableColumn<LaboratoryCategoryDto>[] = [
    { label: 'Select', property: 'select', type: 'button', visible: true },
    { label: 'Actions', property: 'actions', type: 'button', visible: true },
    { label: 'Status', property: 'isActive', type: 'button', visible: true },
    { label: 'Shared', property: 'shared', type: 'bool', visible: true },
    { label: 'Name', property: 'name', type: 'text', visible: true },
    { label: 'Creator', property: 'creator', type: 'text', visible: true },
    { label: 'Created On', property: 'createdOn', type: 'datetime', visible: true },
  ];
  dataSource: ObjectDataSource;
  selection = new SelectionModel<LaboratoryCategoryDto>(true, []);

  constructor(public injector: Injector) {
    super(injector);

    // Hide Actions and Select columns
    const isSuperAdmin = this.permissionsService.getPermission('SuperAdmin');
    if (!isSuperAdmin) {
      this.columns = this.columns.filter(x => x.property != 'select' && x.property != 'shared');
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

    this.dataSource.loadObjects(LaboratoryCategoriesController.GetAll, params);
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
    this.dialog.open(AddEditLaboratoryCategoryComponent)
      .afterClosed()
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res) {
          this.loadData();
        }
      });
  }

  updateObject(laboratoryCategory: LaboratoryCategoryDto) {
    this.dialog.open(AddEditLaboratoryCategoryComponent, {
      data: laboratoryCategory
    })
      .afterClosed()
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res) {
          this.loadData();
        }
      });
  }

  updateIsActive(laboratoryCategory: LaboratoryCategoryDto) {
    this.dialog.open(ConfirmActiveComponent, {
      data: {
        url: LaboratoryCategoriesController.UpdateIsActive,
        objectInfo: [{ key: 'Name', value: laboratoryCategory.name }],
        isActive: !laboratoryCategory.isActive,
        queryParamsDto: { key: 'laboratoryCategoryId', value: laboratoryCategory.id },
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

  deleteObject(laboratoryCategory: LaboratoryCategoryDto) {
    this.dialog.open(ConfirmDeleteComponent, {
      data: {
        url: LaboratoryCategoriesController.RemoveLaboratoryCategory,
        objectInfo: [{ key: 'Name', value: laboratoryCategory.name }],
        queryParamsDto: { key: 'laboratoryCategoryId', value: laboratoryCategory.id },
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
        url: LaboratoryCategoriesController.ExportLaboratoryCategories,
        fileName: 'LaboratoryCategories',
        queryParamsDtos: params,
      }
    });
  }


  changeIsActiveForSelected(isActive: boolean) {
    this.dialog.open(ConfirmActiveSelectedComponent, {
      data: {
        url: LaboratoryCategoriesController.UpdateIsActiveForSelected,
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
        url: LaboratoryCategoriesController.UpdateSharedForSelected,
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
