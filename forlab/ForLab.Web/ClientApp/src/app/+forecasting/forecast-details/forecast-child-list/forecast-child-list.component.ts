import { Component, ElementRef, OnInit, ViewChild, Injector, Input } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { fromEvent } from 'rxjs';
import { SelectionModel } from '@angular/cdk/collections';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { TableColumn } from 'src/@core/models/common/response';
import { ForecastCategoriesController } from 'src/@core/APIs/ForecastCategoriesController';
import { ForecastInstrumentsController } from 'src/@core/APIs/ForecastInstrumentsController';
import { ForecastLaboratoriesController } from 'src/@core/APIs/ForecastLaboratoriesController';
import { ForecastLaboratoryConsumptionsController } from 'src/@core/APIs/ForecastLaboratoryConsumptionsController';
import { ForecastLaboratoryTestServicesController } from 'src/@core/APIs/ForecastLaboratoryTestServicesController';
import { ForecastMorbidityProgramsController } from 'src/@core/APIs/ForecastMorbidityProgramsController';
import { ForecastMorbidityTargetBasesController } from 'src/@core/APIs/ForecastMorbidityTargetBasesController';
import { ForecastMorbidityTestingProtocolMonthsController } from 'src/@core/APIs/ForecastMorbidityTestingProtocolMonthsController';
import { ForecastMorbidityWhoBasesController } from 'src/@core/APIs/ForecastMorbidityWhoBasesController';
import { ForecastPatientAssumptionValuesController } from 'src/@core/APIs/ForecastPatientAssumptionValuesController';
import { ForecastPatientGroupsController } from 'src/@core/APIs/ForecastPatientGroupsController';
import { ForecastResultsController } from 'src/@core/APIs/ForecastResultsController';
import { ForecastProductAssumptionValuesController } from 'src/@core/APIs/ForecastProductAssumptionValuesController';
import { ForecastTestingAssumptionValuesController } from 'src/@core/APIs/ForecastTestingAssumptionValuesController';
import { ForecastTestsController } from 'src/@core/APIs/ForecastTestsController';
import { ObjectDataSource } from 'src/@core/services/object.datasource';
import { takeUntil, tap, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { BaseService } from 'src/@core/services/base.service';
import { ForecastMethodologyEnum } from 'src/@core/models/enum/Enums';

@Component({
  selector: 'forecast-child-list',
  templateUrl: './forecast-child-list.component.html',
  styleUrls: ['./forecast-child-list.component.scss']
})
export class ForecastChildListComponent extends BaseService implements OnInit {

  // Input
  @Input('forecastInfoId') forecastInfoId: number;
  @Input('isAggregate') isAggregate: boolean = false;
  @Input('objectName') objectName: string;
  @Input('forecastMethodologyId') forecastMethodologyId: number;
  // Table
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort;
  @ViewChild('searchInput', { static: false }) searchInput: ElementRef;
  columns: TableColumn<any>[] = [];
  dataSource: ObjectDataSource;
  selection = new SelectionModel<any>(true, []);
  // Vars
  searchBy: string = '';
  // Drp
  forecastMethodologyEnum = ForecastMethodologyEnum;
  
  constructor(public injector: Injector) {
    super(injector);
  }


  ngOnInit() {
    this.columns = this.getTableColumns;
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
      { key: 'forecastInfoId', value: this.forecastInfoId },
      { key: 'name', value: this.searchInput.nativeElement.value },
    ];

    this.dataSource.loadObjects(this.getCorrectAPI, params);
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

  get getCorrectAPI(): string {
    let result = null;
    this.searchBy = '';
    switch (this.objectName) {
      case 'ForecastCategory':
        this.searchBy = 'Category';
        result = ForecastCategoriesController.GetAll;
        break;
      case 'ForecastInstrument':
        this.searchBy = 'Instrument';
        result = ForecastInstrumentsController.GetAll;
        break;
      case 'ForecastLaboratory':
        this.searchBy = 'Laboratory';
        result = ForecastLaboratoriesController.GetAll;
        break;
      case 'ForecastLaboratoryConsumption':
        this.searchBy = 'Product';
        result = ForecastLaboratoryConsumptionsController.GetAll;
        break;
      case 'ForecastLaboratoryTestService':
        this.searchBy = 'Test';
        result = ForecastLaboratoryTestServicesController.GetAll;
        break;
      case 'ForecastMorbidityProgram':
        this.searchBy = 'Program';
        result = ForecastMorbidityProgramsController.GetAll;
        break;
      case 'ForecastMorbidityTargetBase':
        this.searchBy = 'Program';
        result = ForecastMorbidityTargetBasesController.GetAll;
        break;
      case 'ForecastMorbidityTestingProtocolMonth':
        this.searchBy = 'Patient Group';
        result = ForecastMorbidityTestingProtocolMonthsController.GetAll;
        break;
      case 'ForecastMorbidityWhoBase':
        this.searchBy = 'Disease';
        result = ForecastMorbidityWhoBasesController.GetAll;
        break;
      case 'ForecastPatientAssumptionValue':
        this.searchBy = 'Assumption Name';
        result = ForecastPatientAssumptionValuesController.GetAll;
        break;
      case 'ForecastPatientGroup':
        this.searchBy = 'Patient Group';
        result = ForecastPatientGroupsController.GetAll;
        break;
      case 'ForecastProductAssumptionValue':
        this.searchBy = 'Assumption Name';
        result = ForecastProductAssumptionValuesController.GetAll;
        break;
      case 'ForecastTestingAssumptionValue':
        this.searchBy = 'Assumption Name';
        result = ForecastTestingAssumptionValuesController.GetAll;
        break;
      case 'ForecastTest':
        this.searchBy = 'Test';
        result = ForecastTestsController.GetAll;
        break;
      case 'ForecastResult':
        this.searchBy = 'Period';
        result = ForecastResultsController.GetAll;
        break;
      default:
        this.searchBy = '';
        result = null;
        break;
    }

    return result;
  }

  get getTableColumns(): TableColumn<any>[] {
    let result: TableColumn<any>[] = null;
    switch (this.objectName) {
      case 'ForecastCategory':
        result = [
          { label: 'Status', property: 'isActive', type: 'button', visible: true },
          { label: 'Name', property: 'name', type: 'text', visible: true },
          { label: 'Creator', property: 'creator', type: 'text', visible: true },
          { label: 'Created On', property: 'createdOn', type: 'datetime', visible: true },
        ];
        break;
      case 'ForecastInstrument':
        result = [
          { label: 'Status', property: 'isActive', type: 'button', visible: true },
          { label: 'Instrument', property: 'instrumentName', type: 'text', visible: true },
          { label: 'Creator', property: 'creator', type: 'text', visible: true },
          { label: 'Created On', property: 'createdOn', type: 'datetime', visible: true },
        ];
        break;
      case 'ForecastLaboratory':
        if(this.isAggregate) {
          result = [
            { label: 'Status', property: 'isActive', type: 'button', visible: true },
            { label: 'Forecast Category', property: 'forecastCategoryName', type: 'text', visible: true },
            { label: 'Laboratory', property: 'laboratoryName', type: 'text', visible: true },
            { label: 'Creator', property: 'creator', type: 'text', visible: true },
            { label: 'Created On', property: 'createdOn', type: 'datetime', visible: true },
          ];
        } else {
            result = [
              { label: 'Status', property: 'isActive', type: 'button', visible: true },
              { label: 'Laboratory', property: 'laboratoryName', type: 'text', visible: true },
              { label: 'Creator', property: 'creator', type: 'text', visible: true },
              { label: 'Created On', property: 'createdOn', type: 'datetime', visible: true },
            ];
        }
        break;
      case 'ForecastLaboratoryConsumption':
        result = [
          { label: 'Status', property: 'isActive', type: 'button', visible: true },
          { label: 'Laboratory', property: 'laboratoryName', type: 'text', visible: true },
          { label: 'Product', property: 'productName', type: 'text', visible: true },
          { label: 'Period', property: 'period', type: 'text', visible: true },
          { label: 'Amount Forecasted', property: 'amountForecasted', type: 'number', visible: true },
          { label: 'Creator', property: 'creator', type: 'text', visible: false },
          { label: 'Created On', property: 'createdOn', type: 'datetime', visible: false },
        ];
        break;
      case 'ForecastLaboratoryTestService':
        result = [
          { label: 'Status', property: 'isActive', type: 'button', visible: true },
          { label: 'Laboratory', property: 'laboratoryName', type: 'text', visible: true },
          { label: 'Test', property: 'testName', type: 'text', visible: true },
          { label: 'Period', property: 'period', type: 'text', visible: true },
          { label: 'Amount Forecasted', property: 'amountForecasted', type: 'number', visible: true },
          { label: 'Creator', property: 'creator', type: 'text', visible: false },
          { label: 'Created On', property: 'createdOn', type: 'datetime', visible: false },
        ];
        break;
      case 'ForecastMorbidityProgram':
        result = [
          { label: 'Status', property: 'isActive', type: 'button', visible: true },
          { label: 'Program', property: 'programName', type: 'text', visible: true },
          { label: 'Creator', property: 'creator', type: 'text', visible: true },
          { label: 'Created On', property: 'createdOn', type: 'datetime', visible: true },
        ];
        break;
      case 'ForecastMorbidityTargetBase':
        result = [
          { label: 'Status', property: 'isActive', type: 'button', visible: true },
          { label: 'Laboratory', property: 'forecastLaboratoryLaboratoryName', type: 'text', visible: true },
          { label: 'Program', property: 'forecastMorbidityProgramProgramName', type: 'text', visible: true },
          { label: 'Current Patient', property: 'currentPatient', type: 'number', visible: true },
          { label: 'Target Patient', property: 'targetPatient', type: 'number', visible: true },
          { label: 'Creator', property: 'creator', type: 'text', visible: false },
          { label: 'Created On', property: 'createdOn', type: 'datetime', visible: false },
        ];
        break;
      case 'ForecastMorbidityTestingProtocolMonth':
        result = [
          { label: 'Status', property: 'isActive', type: 'button', visible: true },
          { label: 'Program', property: 'programName', type: 'text', visible: true },
          { label: 'Test', property: 'testName', type: 'text', visible: true },
          { label: 'Patient Group', property: 'patientGroupName', type: 'text', visible: true },
          { label: 'Month', property: 'calculationPeriodMonthName', type: 'text', visible: true },
          { label: 'Value', property: 'value', type: 'number', visible: true },
          { label: 'Creator', property: 'creator', type: 'text', visible: false },
          { label: 'Created On', property: 'createdOn', type: 'datetime', visible: false },
        ];
        break;
      case 'ForecastMorbidityWhoBase':
        result = [
          { label: 'Status', property: 'isActive', type: 'button', visible: true },
          { label: 'Country', property: 'countryName', type: 'text', visible: true },
          { label: 'Disease', property: 'diseaseName', type: 'text', visible: true },
          { label: 'Period', property: 'period', type: 'text', visible: true },
          { label: 'Count', property: 'count', type: 'number', visible: true },
          { label: 'Creator', property: 'creator', type: 'text', visible: false },
          { label: 'Created On', property: 'createdOn', type: 'datetime', visible: false },
        ];
        break;
      case 'ForecastPatientAssumptionValue':
        result = [
          { label: 'Status', property: 'isActive', type: 'button', visible: true },
          { label: 'Assumption Parameter', property: 'patientAssumptionParameterName', type: 'text', visible: true },
          { label: 'Value', property: 'value', type: 'number', visible: true },
          { label: 'Creator', property: 'creator', type: 'text', visible: true },
          { label: 'Created On', property: 'createdOn', type: 'datetime', visible: true },
        ];
        break;
      case 'ForecastPatientGroup':
        result = [
          { label: 'Status', property: 'isActive', type: 'button', visible: true },
          { label: 'Patient Group', property: 'patientGroupName', type: 'text', visible: true },
          { label: 'Percentage', property: 'percentage', type: 'percent', visible: true },
          { label: 'Creator', property: 'creator', type: 'text', visible: true },
          { label: 'Created On', property: 'createdOn', type: 'datetime', visible: true },
        ];
        break;
      case 'ForecastProductAssumptionValue':
        result = [
          { label: 'Status', property: 'isActive', type: 'button', visible: true },
          { label: 'Assumption Parameter', property: 'productAssumptionParameterName', type: 'text', visible: true },
          { label: 'Value', property: 'value', type: 'number', visible: true },
          { label: 'Creator', property: 'creator', type: 'text', visible: true },
          { label: 'Created On', property: 'createdOn', type: 'datetime', visible: true },
        ];
        break;
      case 'ForecastTestingAssumptionValue':
        result = [
          { label: 'Status', property: 'isActive', type: 'button', visible: true },
          { label: 'Assumption Parameter', property: 'testingAssumptionParameterName', type: 'text', visible: true },
          { label: 'Value', property: 'value', type: 'number', visible: true },
          { label: 'Creator', property: 'creator', type: 'text', visible: true },
          { label: 'Created On', property: 'createdOn', type: 'datetime', visible: true },
        ];
        break;
      case 'ForecastTest':
        result = [
          { label: 'Status', property: 'isActive', type: 'button', visible: true },
          { label: 'Test', property: 'testName', type: 'text', visible: true },
          { label: 'Creator', property: 'creator', type: 'text', visible: true },
          { label: 'Created On', property: 'createdOn', type: 'datetime', visible: true },
        ];
        break;
      case 'ForecastResult':
        if(this.forecastMethodologyId == this.forecastMethodologyEnum.Consumption) {
          result = [
            { label: 'Laboratory', property: 'laboratoryName', type: 'text', visible: true },
            { label: 'Product', property: 'productName', type: 'text', visible: true },
            { label: 'Product Type', property: 'productTypeName', type: 'text', visible: false },
            { label: 'Amount Forecasted', property: 'amountForecasted', type: 'number', visible: true },
            { label: 'Total Value', property: 'totalValue', type: 'number', visible: true },
            { label: 'Duration', property: 'durationDateTime', type: 'date', visible: false },
            { label: 'Period', property: 'period', type: 'text', visible: true },
            { label: 'Pack Size', property: 'packSize', type: 'int', visible: true },
            { label: 'Pack Qty', property: 'packQty', type: 'int', visible: true },
            { label: 'Pack Price', property: 'packPrice', type: 'price', visible: false },
            { label: 'Total Price', property: 'totalPrice', type: 'price', visible: true },
            { label: 'Creator', property: 'creator', type: 'text', visible: false },
            { label: 'Created On', property: 'createdOn', type: 'datetime', visible: false },
          ];
        } else {
          result = [
            { label: 'Laboratory', property: 'laboratoryName', type: 'text', visible: true },
            { label: 'Test', property: 'testName', type: 'text', visible: true },
            { label: 'Product', property: 'productName', type: 'text', visible: true },
            { label: 'Product Type', property: 'productTypeName', type: 'text', visible: false },
            { label: 'Amount Forecasted', property: 'amountForecasted', type: 'number', visible: true },
            { label: 'Total Value', property: 'totalValue', type: 'number', visible: true },
            { label: 'Duration', property: 'durationDateTime', type: 'date', visible: false },
            { label: 'Period', property: 'period', type: 'text', visible: true },
            { label: 'Pack Size', property: 'packSize', type: 'int', visible: true },
            { label: 'Pack Qty', property: 'packQty', type: 'int', visible: true },
            { label: 'Pack Price', property: 'packPrice', type: 'price', visible: false },
            { label: 'Total Price', property: 'totalPrice', type: 'price', visible: true },
            { label: 'Creator', property: 'creator', type: 'text', visible: false },
            { label: 'Created On', property: 'createdOn', type: 'datetime', visible: false },
          ];
        }
        break;
      default:
        result = null;
        break;
    }

    return result;
  }

}
