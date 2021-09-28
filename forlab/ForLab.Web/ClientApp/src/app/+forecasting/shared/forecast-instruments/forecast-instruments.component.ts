import { Component, OnInit, ViewChild, Injector, ElementRef, Input } from '@angular/core';
import { TableColumn, QueryParamsDto } from 'src/@core/models/common/response';
import { MatTableDataSource } from '@angular/material/table';
import { SelectionModel } from '@angular/cdk/collections';
import { BaseService } from 'src/@core/services/base.service';
import { ForecastInstrumentDto } from 'src/@core/models/forecasting/ForecastInstrument';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { filter, map, debounceTime, distinctUntilChanged, tap, startWith } from 'rxjs/operators';
import { ReplaySubject, Observable, fromEvent, combineLatest } from 'rxjs';
import { TestingAreasController } from 'src/@core/APIs/TestingAreasController';
import { TestingAreaDto } from 'src/@core/models/lookup/TestingArea';
import { TestDto } from 'src/@core/models/testing/Test';
import { FormControl } from '@angular/forms';
import { InstrumentDto } from 'src/@core/models/product/Instrument';
import { InstrumentsController } from 'src/@core/APIs/InstrumentsController';

@Component({
  selector: 'forecast-instruments',
  templateUrl: './forecast-instruments.component.html',
  styleUrls: ['./forecast-instruments.component.scss']
})
export class ForecastInstrumentsComponent extends BaseService implements OnInit {

  @Input('forecastInstrumentDtos')forecastInstrumentDtos: ForecastInstrumentDto[] = [];
  // Drp
  testingAreas$: Observable<TestingAreaDto[]>;
  instruments$: Observable<TestDto[]>;
  selectedTestingArea: TestingAreaDto;
  // Filter testing areas
  filteredTestingAreas$: Observable<TestingAreaDto[]>;
  testingAreaFilter: FormControl = new FormControl('');
  testingAreaFilter$: Observable<string>;
  // Table
  subject$: ReplaySubject<ForecastInstrumentDto[]> = new ReplaySubject<ForecastInstrumentDto[]>(1);
  data$: Observable<ForecastInstrumentDto[]> = this.subject$.asObservable();
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort;
  // @ViewChild('searchInput', { static: false }) searchInput: ElementRef;
  columns: TableColumn<ForecastInstrumentDto>[] = [
    { label: 'Select', property: 'select', type: 'button', visible: true },
    { label: 'Testing Area', property: 'instrumentTestingAreaName', type: 'text', visible: true },
    { label: 'Instrument Name', property: 'instrumentName', type: 'text', visible: true },
    { label: 'Actions', property: 'actions', type: 'button', visible: true },
  ];
  dataSource: MatTableDataSource<ForecastInstrumentDto> | null = new MatTableDataSource();
  selection = new SelectionModel<ForecastInstrumentDto>(true, []);

  constructor(public injector: Injector) {
    super(injector);
  }

  ngOnInit() {

    // Loading Drp
    this.loadTestingAreas();

    this.data$.pipe(
      filter<ForecastInstrumentDto[]>(Boolean)
    ).subscribe(forecastInstrumentDtos => {
      this.forecastInstrumentDtos = forecastInstrumentDtos;
      this.dataSource.data = forecastInstrumentDtos;
    });
    this.subject$.next(this.forecastInstrumentDtos);
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
  loadInstruments(testingAreaId: number) {
    let params: QueryParamsDto[] = [
      { key: 'testingAreaId', value: testingAreaId },
    ];
    this.instruments$ = this.httpService.GET(InstrumentsController.GetAllAsDrp, params).pipe(map(res => res.data));
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
      this.deleteForecastInstrument(x, this.forecastInstrumentDtos.indexOf(x));
    });
    this.selection = new SelectionModel<ForecastInstrumentDto>(true, []);
  }

  deleteForecastInstrument(forecastInstrumentDto: ForecastInstrumentDto, index: number) {
    this.forecastInstrumentDtos.splice(index, 1);
    this.subject$.next(this.forecastInstrumentDtos);
  }

  isInstrumentSelected(instrument: InstrumentDto) {
    return this.forecastInstrumentDtos.findIndex(x => x.instrumentId == instrument.id) > -1;
  }
  toggleInstrumentSelect(e, instrument: InstrumentDto) {
    if (e.checked) {
      let newForecastInstrumentDto = new ForecastInstrumentDto;
      newForecastInstrumentDto.instrumentId = instrument.id;
      newForecastInstrumentDto.instrumentName = instrument.name;
      newForecastInstrumentDto.instrumentTestingAreaName = this.selectedTestingArea.name;
      this.forecastInstrumentDtos.push(newForecastInstrumentDto);
      this.subject$.next(this.forecastInstrumentDtos);
    } else {
      let forecastInstrumentDto = this.forecastInstrumentDtos.find(x => x.instrumentId == instrument.id);
      this.deleteForecastInstrument(forecastInstrumentDto, this.forecastInstrumentDtos.indexOf(forecastInstrumentDto));
    }
  }

  get isValid(): boolean {
    if (this.forecastInstrumentDtos && this.forecastInstrumentDtos.length == 0) return false;
    return true;
  }

}
