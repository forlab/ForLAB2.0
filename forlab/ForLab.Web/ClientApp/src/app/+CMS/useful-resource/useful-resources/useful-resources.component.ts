import { Component, ElementRef, OnInit, ViewChild, Injector } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { fromEvent } from 'rxjs';
import { SelectionModel } from '@angular/cdk/collections';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { ConfirmDeleteComponent } from 'src/@core/directives/confirm-delete/confirm-delete.component';
import { UsefulResourceDto, UsefulResourceFilterDto } from 'src/@core/models/CMS/UsefulResource';
import { TableColumn } from 'src/@core/models/common/response';
import { UsefulResourcesController } from 'src/@core/APIs/UsefulResourcesController';
import { ObjectDataSource } from 'src/@core/services/object.datasource';
import { takeUntil, tap, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ConfirmActiveComponent } from 'src/@core/directives/confirm-active/confirm-active.component';
import { BaseService } from 'src/@core/services/base.service';
import { AddEditUsefulResourceComponent } from '../add-edit-useful-resource/add-edit-useful-resource.component';

@Component({
  selector: 'app-useful-resources',
  templateUrl: './useful-resources.component.html',
  styleUrls: ['./useful-resources.component.scss']
})
export class UsefulResourcesComponent extends BaseService implements OnInit {

  filterDto: UsefulResourceFilterDto = new UsefulResourceFilterDto();

  // Table
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort;
  @ViewChild('searchInput', { static: false }) searchInput: ElementRef;
  columns: TableColumn<UsefulResourceDto>[] = [
    { label: 'Actions', property: 'actions', type: 'button', visible: true },
    // { label: 'Select', property: 'select', type: 'button', visible: false },
    { label: 'Status', property: 'isActive', type: 'button', visible: true },
    { label: 'Title', property: 'title', type: 'text', visible: true },
    { label: 'Attachment Name', property: 'attachmentName', type: 'text', visible: true },
    { label: 'Extension Format', property: 'extensionFormat', type: 'text', visible: true },
    { label: 'Download Count', property: 'downloadCount', type: 'int', visible: true },
    { label: 'Is External Resource', property: 'isExternalResource', type: 'bool', visible: true },
    { label: 'Creator', property: 'creator', type: 'text', visible: false },
    { label: 'Created On', property: 'createdOn', type: 'datetime', visible: false },
  ];
  dataSource: ObjectDataSource;
  selection = new SelectionModel<UsefulResourceDto>(true, []);

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
      { key: 'title', value: this.searchInput.nativeElement.value || this.filterDto.title },
    ];

    // Append FilterDto values to the paramaters
    params = params.concat(this.getFilterParamsDtos(this.filterDto, ['title']));

    this.dataSource.loadObjects(UsefulResourcesController.GetAll, params);
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
    this.dialog.open(AddEditUsefulResourceComponent)
      .afterClosed()
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res) {
          this.loadData();
        }
      });
  }

  updateObject(usefulResource: UsefulResourceDto) {
    this.dialog.open(AddEditUsefulResourceComponent, {
      data: usefulResource
    })
      .afterClosed()
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res) {
          this.loadData();
        }
      });
  }

  updateIsActive(usefulResource: UsefulResourceDto) {
    this.dialog.open(ConfirmActiveComponent, {
      data: {
        url: UsefulResourcesController.UpdateIsActive,
        objectInfo: [{ key: 'Title', value: usefulResource.title }, { key: 'Extension Format', value: usefulResource.extensionFormat }, { key: 'Download Count', value: usefulResource.downloadCount }],
        isActive: !usefulResource.isActive,
        queryParamsDto: { key: 'usefulResourceId', value: usefulResource.id },
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

  deleteObject(usefulResource: UsefulResourceDto) {
    this.dialog.open(ConfirmDeleteComponent, {
      data: {
        url: UsefulResourcesController.RemoveUsefulResource,
        objectInfo: [{ key: 'Title', value: usefulResource.title }, { key: 'Extension Format', value: usefulResource.extensionFormat }, { key: 'Download Count', value: usefulResource.downloadCount }],
        queryParamsDto: { key: 'usefulResourceId', value: usefulResource.id },
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
