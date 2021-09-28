import { Component, ElementRef, OnInit, ViewChild, Injector } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { fromEvent, Observable } from 'rxjs';
import { SelectionModel } from '@angular/cdk/collections';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { TestingProtocolDto, TestingProtocolFilterDto } from 'src/@core/models/testing/TestingProtocol';
import { TableColumn } from 'src/@core/models/common/response';
import { ObjectDataSource } from 'src/@core/services/object.datasource';
import { tap, debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { BaseService } from 'src/@core/services/base.service';
import { TestingProtocolsController } from 'src/@core/APIs/TestingProtocolsController';
import { TestsController } from 'src/@core/APIs/TestsController';
import { ConfirmExportComponent } from 'src/@core/directives/confirm-export/confirm-export.component';
import { CalculationPeriodEnum } from 'src/@core/models/enum/Enums';
import { PatientGroupsController } from 'src/@core/APIs/PatientGroupsController';

@Component({
  selector: 'app-testing-protocols',
  templateUrl: './testing-protocols.component.html',
  styleUrls: ['./testing-protocols.component.scss']
})
export class TestingProtocolsComponent extends BaseService implements OnInit {

  filterDto: TestingProtocolFilterDto = new TestingProtocolFilterDto();
  // Drp
  calculationPeriodEnum = CalculationPeriodEnum;
  tests$: Observable<any[]>;
  patientGroups$: Observable<any[]>;

  // Table
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort;
  @ViewChild('searchInput', { static: false }) searchInput: ElementRef;
  columns: TableColumn<TestingProtocolDto>[] = [
    { label: 'Status', property: 'isActive', type: 'button', visible: true },
    { label: 'Patient Group', property: 'patientGroupName', type: 'text', visible: true },
    { label: 'Test', property: 'testName', type: 'text', visible: true },
    { label: 'Calculation Period', property: 'calculationPeriodName', type: 'text', visible: true },
    { label: 'Base Line', property: 'baseLine', type: 'int', visible: true },
    { label: 'Test After First Year', property: 'testAfterFirstYear', type: 'int', visible: true },
    { label: 'Creator', property: 'creator', type: 'text', visible: false },
    { label: 'Created On', property: 'createdOn', type: 'datetime', visible: false },
  ];
  dataSource: ObjectDataSource;
  selection = new SelectionModel<TestingProtocolDto>(true, []);

  constructor(public injector: Injector) {
    super(injector);
  }


  ngOnInit() {
    // Load Drp
    this.loadTests();
    this.loadPatientGroups();

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

  loadTests() {
    this.tests$ = this.httpService.GET(TestsController.GetAllAsDrp).pipe(map(res => res.data));
  }
  loadPatientGroups() {
    this.patientGroups$ = this.httpService.GET(PatientGroupsController.GetAllAsDrp).pipe(map(res => res.data));
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
      { key: 'name', value: this.searchInput.nativeElement.value || this.filterDto.testId },
    ];

    // Append FilterDto values to the paramaters
    params = params.concat(this.getFilterParamsDtos(this.filterDto));

    this.dataSource.loadObjects(TestingProtocolsController.GetAll, params);
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

  export() {
    let params: QueryParamsDto[] = [
      { key: 'pageSize', value: this.paginator.pageSize || 10 },
      { key: 'pageIndex', value: this.paginator.pageIndex + 1 || 1 },
      { key: 'applySort', value: this.sort && this.sort.active ? true : false },
      { key: 'sortProperty', value: this.sort && this.sort.active ? this.sort.active : null },
      { key: 'isAscending', value: this.sort && this.sort.direction == 'asc' ? true : false },
      // Filter
      { key: 'name', value: this.searchInput.nativeElement.value || this.filterDto.testId },
    ];

    // Append FilterDto values to the paramaters
    params = params.concat(this.getFilterParamsDtos(this.filterDto));

    this.dialog.open(ConfirmExportComponent, {
      data: {
        url: TestingProtocolsController.ExportTestingProtocols,
        fileName: 'TestingProtocols',
        queryParamsDtos: params,
      }
    });
  }

}
