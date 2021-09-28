import { Component, OnInit, ViewChild, Injector, ElementRef, Input } from '@angular/core';
import { TableColumn, QueryParamsDto } from 'src/@core/models/common/response';
import { MatTableDataSource } from '@angular/material/table';
import { SelectionModel } from '@angular/cdk/collections';
import { BaseService } from 'src/@core/services/base.service';
import { ForecastTestDto } from 'src/@core/models/forecasting/ForecastTest';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { filter, map, debounceTime, distinctUntilChanged, tap, startWith } from 'rxjs/operators';
import { ReplaySubject, Observable, fromEvent, combineLatest } from 'rxjs';
import { TestingAreasController } from 'src/@core/APIs/TestingAreasController';
import { TestsController } from 'src/@core/APIs/TestsController';
import { TestingAreaDto } from 'src/@core/models/lookup/TestingArea';
import { TestDto } from 'src/@core/models/testing/Test';
import { FormControl } from '@angular/forms';

@Component({
  selector: 'forecast-tests',
  templateUrl: './forecast-tests.component.html',
  styleUrls: ['./forecast-tests.component.scss']
})
export class ForecastTestsComponent extends BaseService implements OnInit {

  @Input('forecastTestDtos') forecastTestDtos: ForecastTestDto[] = [];
  // Drp
  testingAreas$: Observable<TestingAreaDto[]>;
  tests$: Observable<TestDto[]>;
  selectedTestingArea: TestingAreaDto;
  // Filter testing areas
  filteredTestingAreas$: Observable<TestingAreaDto[]>;
  testingAreaFilter: FormControl = new FormControl('');
  testingAreaFilter$: Observable<string>;
  // Table
  subject$: ReplaySubject<ForecastTestDto[]> = new ReplaySubject<ForecastTestDto[]>(1);
  data$: Observable<ForecastTestDto[]> = this.subject$.asObservable();
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort;
  // @ViewChild('searchInput', { static: false }) searchInput: ElementRef;
  columns: TableColumn<ForecastTestDto>[] = [
    { label: 'Select', property: 'select', type: 'button', visible: true },
    { label: 'Testing Area', property: 'testTestingAreaName', type: 'text', visible: true },
    { label: 'Test Name', property: 'testName', type: 'text', visible: true },
    { label: 'Actions', property: 'actions', type: 'button', visible: true },
  ];
  dataSource: MatTableDataSource<ForecastTestDto> | null = new MatTableDataSource();
  selection = new SelectionModel<ForecastTestDto>(true, []);

  constructor(public injector: Injector) {
    super(injector);
  }

  ngOnInit() {

    // Loading Drp
    this.loadTestingAreas();

    this.data$.pipe(
      filter<ForecastTestDto[]>(Boolean)
    ).subscribe(forecastTestDtos => {
      this.forecastTestDtos = forecastTestDtos;
      this.dataSource.data = forecastTestDtos;
    });
    this.subject$.next(this.forecastTestDtos);
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;

    // Filter Testing Area
    this.testingAreaFilter$ = this.testingAreaFilter.valueChanges.pipe(startWith(''));
    this.filteredTestingAreas$ = combineLatest(this.testingAreas$, this.testingAreaFilter$).pipe(
      map(([testingAreas, filterString]) => testingAreas.filter(area => area.name.trim().toLowerCase().indexOf(filterString.trim().toLowerCase()) !== -1))
    );

  }

  loadTestingAreas() {
    this.testingAreas$ = this.httpService.GET(TestingAreasController.GetAllAsDrp).pipe(map(res => res.data));
  }
  loadTests(testingAreaId: number) {
    let params: QueryParamsDto[] = [
      { key: 'testingAreaId', value: testingAreaId },
    ];
    this.tests$ = this.httpService.GET(TestsController.GetAllAsDrp, params).pipe(map(res => res.data));
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  onFilterChange(value: string) {
    if (!this.dataSource) {
      return;
    }
    value = value.trim();
    value = value.toLowerCase();
    this.dataSource.filter = value;
  }

  toggleColumnVisibility(column, event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    column.visible = !column.visible;
  }

  /** Whether the number of selected elements matches the total number of rows. */
  isAllSelected() {
    const numSelected = this.selection.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  /** Selects all rows if they are not all selected; otherwise clear selection. */
  masterToggle() {
    this.isAllSelected() ?
      this.selection.clear() :
      this.dataSource.data.forEach(row => this.selection.select(row));
  }

  trackByProperty<T>(index: number, column: TableColumn<T>) {
    return column.property;
  }

  removeSelectedRows() {
    if (!this.selection.selected && this.selection.selected.length == 0) {
      return;
    }
    this.selection.selected.forEach(x => {
      this.deleteForecastTest(x, this.forecastTestDtos.indexOf(x));
    });
    this.selection = new SelectionModel<ForecastTestDto>(true, []);
  }

  deleteForecastTest(forecastTestDto: ForecastTestDto, index: number) {
    this.forecastTestDtos.splice(index, 1);
    this.subject$.next(this.forecastTestDtos);
  }

  isTestSelected(test: TestDto) {
    return this.forecastTestDtos.findIndex(x => x.testId == test.id) > -1;
  }
  toggleTestSelect(e, test: TestDto) {
    if (e.checked) {
      let newForecastTestDto = new ForecastTestDto;
      newForecastTestDto.testId = test.id;
      newForecastTestDto.testName = test.name;
      newForecastTestDto.testTestingAreaName = this.selectedTestingArea.name;
      this.forecastTestDtos.push(newForecastTestDto);
      this.subject$.next(this.forecastTestDtos);
    } else {
      let forecastTestDto = this.forecastTestDtos.find(x => x.testId == test.id);
      this.deleteForecastTest(forecastTestDto, this.forecastTestDtos.indexOf(forecastTestDto));
    }
  }

  get isValid(): boolean {
    if (this.forecastTestDtos && this.forecastTestDtos.length == 0) return false;
    return true;
  }

}
