import { Component, ElementRef, OnInit, ViewChild, Injector, Input } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { fromEvent } from 'rxjs';
import { SelectionModel } from '@angular/cdk/collections';
import { BaseService } from 'src/@core/services/base.service';
import { UserSubscriptionLevelsEnum } from 'src/@core/models/enum/Enums';
import { TableColumn, QueryParamsDto } from 'src/@core/models/common/response';
import { UserSubscriptionsController } from 'src/@core/APIs/UserSubscriptionsController';
import { ObjectDataSource } from 'src/@core/services/object.datasource';
import { tap, debounceTime, distinctUntilChanged } from 'rxjs/operators';

@Component({
  selector: 'app-user-subscriptions',
  templateUrl: './user-subscriptions.component.html',
  styleUrls: ['./user-subscriptions.component.scss']
})
export class UserSubscriptionsComponent extends BaseService implements OnInit {

  @Input('userId') userId: number;
  @Input('userSubscriptionLevelId') userSubscriptionLevelId: number;
  // Drp
  userSubscriptionLevelsEnum = UserSubscriptionLevelsEnum;

  // Table
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort;
  @ViewChild('searchInput', { static: false }) searchInput: ElementRef;
  columns: TableColumn<any>[] = [];
  dataSource: ObjectDataSource;
  selection = new SelectionModel<any>(true, []);

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

    if(!this.userSubscriptionLevelId || !this.userId) {
      return;
    }

    this.selection.clear();

    let params: QueryParamsDto[] = [
      { key: 'pageSize', value: this.paginator.pageSize || 10 },
      { key: 'pageIndex', value: this.paginator.pageIndex + 1 || 1 },
      { key: 'applySort', value: this.sort && this.sort.active ? true : false },
      { key: 'sortProperty', value: this.sort && this.sort.active ? this.sort.active : null },
      { key: 'isAscending', value: this.sort && this.sort.direction == 'asc' ? true : false },
      // Filter
      { key: 'name', value: this.searchInput.nativeElement.value },
      { key: 'applicationUserId', value: this.userId },
    ];

    // Get correct URL
    let url = null;
    switch (this.userSubscriptionLevelId) {
      case this.userSubscriptionLevelsEnum.CountryLevel:
        url = UserSubscriptionsController.GetAllUserCountrySubscriptions;
        this.columns = [
          { label: 'Country', property: 'countryName', type: 'text', visible: true },
          { label: 'Created On', property: 'createdOn', type: 'datetime', visible: true },
        ];
        break;
      case this.userSubscriptionLevelsEnum.RegionLevel:
        url = UserSubscriptionsController.GetAllUserRegionSubscriptions;
        this.columns = [
          { label: 'Country', property: 'regionCountryName', type: 'text', visible: true },
          { label: 'Region', property: 'regionName', type: 'text', visible: true },
          { label: 'Created On', property: 'createdOn', type: 'datetime', visible: true },
        ];
        break;
      case this.userSubscriptionLevelsEnum.LaboratoryLevel:
        url = UserSubscriptionsController.GetAllUserLaboratorySubscriptions;
        this.columns = [
          { label: 'Country', property: 'laboratoryRegionCountryName', type: 'text', visible: true },
          { label: 'Region', property: 'laboratoryRegionName', type: 'text', visible: true },
          { label: 'Laboratory', property: 'laboratoryName', type: 'text', visible: true },
          { label: 'Created On', property: 'createdOn', type: 'datetime', visible: true },
        ];
        break;
    }
    this._ref.detectChanges();
    this.dataSource.loadObjects(url, params);
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
