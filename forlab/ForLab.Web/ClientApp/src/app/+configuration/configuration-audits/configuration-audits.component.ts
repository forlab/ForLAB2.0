import { Component, ElementRef, OnInit, ViewChild, Injector } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { fromEvent } from 'rxjs';
import { SelectionModel } from '@angular/cdk/collections';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { ConfigurationDto } from 'src/@core/models/configuration/Configuration';
import { TableColumn } from 'src/@core/models/common/response';
import { ConfigurationsController } from 'src/@core/APIs/ConfigurationsController';
import { ObjectDataSource } from 'src/@core/services/object.datasource';
import { takeUntil, tap, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { BaseService } from 'src/@core/services/base.service';
import { ConfirmExportComponent } from 'src/@core/directives/confirm-export/confirm-export.component';

@Component({
  selector: 'app-configuration-audits',
  templateUrl: './configuration-audits.component.html',
  styleUrls: ['./configuration-audits.component.sass']
})
export class ConfigurationAuditsComponent extends BaseService implements OnInit {

  // Table
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort;
  @ViewChild('searchInput', { static: false }) searchInput: ElementRef;
  columns: TableColumn<ConfigurationDto>[] = [
    // { label: 'Select', property: 'select', type: 'button', visible: false },
    { label: 'Action', property: 'action', type: 'text', visible: true },
    { label: 'Action By', property: 'creator', type: 'text', visible: true },
    { label: 'Date Of Action', property: 'dateOfAction', type: 'datetime', visible: true },
    { label: 'Num of Days to Change Password', property: 'numOfDaysToChangePassword', type: 'custom', visible: true },
    { label: 'Account Login Attempts', property: 'accountLoginAttempts', type: 'custom', visible: true },
    { label: 'Password Expiry Time', property: 'passwordExpiryTime', type: 'custom', visible: true },
    { label: 'User Photo Size', property: 'userPhotosize', type: 'custom', visible: true },
    { label: 'Attachments Max Size', property: 'attachmentsMaxSize', type: 'custom', visible: true },
    { label: 'Times Count Before Password Reuse', property: 'timesCountBeforePasswordReuse', type: 'int', visible: true },
    { label: 'Time to Session TimeOut', property: 'timeToSessionTimeOut', type: 'int', visible: true },
  ];
  dataSource: ObjectDataSource;
  selection = new SelectionModel<ConfigurationDto>(true, []);

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
      { key: 'action', value: this.searchInput.nativeElement.value },
    ];

    this.dataSource.loadObjects(ConfigurationsController.GetAllConfigurationAudits, params);
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


  export() {
    let params: QueryParamsDto[] = [
      { key: 'pageSize', value: this.paginator.pageSize || 10 },
      { key: 'pageIndex', value: this.paginator.pageIndex + 1 || 1 },
      { key: 'applySort', value: this.sort && this.sort.active ? true : false },
      { key: 'sortProperty', value: this.sort && this.sort.active ? this.sort.active : null },
      { key: 'isAscending', value: this.sort && this.sort.direction == 'asc' ? true : false },
      // Filter
      { key: 'action', value: this.searchInput.nativeElement.value },
    ];

    this.dialog.open(ConfirmExportComponent, {
      data: {
        url: ConfigurationsController.ExportConfigurationAudits,
        fileName: 'ConfigurationAudits',
        queryParamsDtos: params,
      }
    });
  }
  
}
