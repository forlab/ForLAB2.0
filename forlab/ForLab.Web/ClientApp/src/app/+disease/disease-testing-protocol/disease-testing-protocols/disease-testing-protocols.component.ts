import { Component, ElementRef, OnInit, ViewChild, Injector } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { fromEvent, Observable } from 'rxjs';
import { SelectionModel } from '@angular/cdk/collections';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { DiseaseTestingProtocolDto, DiseaseTestingProtocolFilterDto } from 'src/@core/models/disease/DiseaseTestingProtocol';
import { TableColumn } from 'src/@core/models/common/response';
import { DiseaseTestingProtocolsController } from 'src/@core/APIs/DiseaseTestingProtocolsController';
import { ObjectDataSource } from 'src/@core/services/object.datasource';
import { takeUntil, tap, debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { BaseService } from 'src/@core/services/base.service';
import { ConfirmExportComponent } from 'src/@core/directives/confirm-export/confirm-export.component';
import { AddEditDiseaseTestingProtocolComponent } from '../add-edit-disease-testing-protocol/add-edit-disease-testing-protocol.component';
import { ConfirmActiveComponent } from 'src/@core/directives/confirm-active/confirm-active.component';
import { ConfirmDeleteComponent } from 'src/@core/directives/confirm-delete/confirm-delete.component';
import { TestingProtocolsController } from 'src/@core/APIs/TestingProtocolsController';
import { DiseasesController } from 'src/@core/APIs/DiseasesController';

@Component({
  selector: 'app-disease-testing-protocols',
  templateUrl: './disease-testing-protocols.component.html',
  styleUrls: ['./disease-testing-protocols.component.scss']
})
export class DiseaseTestingProtocolsComponent extends BaseService implements OnInit {

  filterDto: DiseaseTestingProtocolFilterDto = new DiseaseTestingProtocolFilterDto();
  // Drp
  diseases$: Observable<any[]>;
  testingProtocols$: Observable<any[]>;
  // Table
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort;
  @ViewChild('searchInput', { static: false }) searchInput: ElementRef;
  columns: TableColumn<DiseaseTestingProtocolDto>[] = [
    { label: 'Actions', property: 'actions', type: 'button', visible: true },
    // { label: 'Select', property: 'select', type: 'button', visible: false },
    { label: 'Status', property: 'isActive', type: 'button', visible: true },
    { label: 'Disease', property: 'diseaseName', type: 'text', visible: true },
    { label: 'Testing Protocol', property: 'testingProtocolName', type: 'text', visible: false },
    { label: 'Creator', property: 'creator', type: 'text', visible: true },
    { label: 'Created On', property: 'createdOn', type: 'datetime', visible: true },
  ];
  dataSource: ObjectDataSource;
  selection = new SelectionModel<DiseaseTestingProtocolDto>(true, []);

  constructor(public injector: Injector) {
    super(injector);
  }


  ngOnInit() {
    // Load Drp
    this.loadDiseases();
    this.loadTestingProtocols();

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

  loadDiseases() {
    this.diseases$ = this.httpService.GET(DiseasesController.GetAllAsDrp).pipe(map(res => res.data));
  }
  loadTestingProtocols() {
    this.testingProtocols$ = this.httpService.GET(TestingProtocolsController.GetAllAsDrp).pipe(map(res => res.data));
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
      { key: 'diseaseId', value: this.searchInput.nativeElement.value || this.filterDto.diseaseId },
    ];

    // Append FilterDto values to the paramaters
    params = params.concat(this.getFilterParamsDtos(this.filterDto));

    this.dataSource.loadObjects(DiseaseTestingProtocolsController.GetAll, params);
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
    this.dialog.open(AddEditDiseaseTestingProtocolComponent)
      .afterClosed()
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res) {
          this.loadData();
        }
      });
  }

  updateObject(diseaseTestingProtocol: DiseaseTestingProtocolDto) {
    this.dialog.open(AddEditDiseaseTestingProtocolComponent, {
      data: diseaseTestingProtocol
    })
      .afterClosed()
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res) {
          this.loadData();
        }
      });
  }

  updateIsActive(diseaseTestingProtocol: DiseaseTestingProtocolDto) {
    this.dialog.open(ConfirmActiveComponent, {
      data: {
        url: DiseaseTestingProtocolsController.UpdateIsActive,
        objectInfo: [{ key: 'Disease', value: diseaseTestingProtocol.diseaseName }, { key: 'Testing Protocol', value: diseaseTestingProtocol.testingProtocolName }],
        isActive: !diseaseTestingProtocol.isActive,
        queryParamsDto: { key: 'diseaseTestingProtocolId', value: diseaseTestingProtocol.id },
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

  deleteObject(diseaseTestingProtocol: DiseaseTestingProtocolDto) {
    this.dialog.open(ConfirmDeleteComponent, {
      data: {
        url: DiseaseTestingProtocolsController.RemoveDiseaseTestingProtocol,
        objectInfo: [{ key: 'Disease', value: diseaseTestingProtocol.diseaseName }, { key: 'Testing Protocol', value: diseaseTestingProtocol.testingProtocolName }],
        queryParamsDto: { key: 'diseaseTestingProtocolId', value: diseaseTestingProtocol.id },
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
      { key: 'diseaseId', value: this.searchInput.nativeElement.value || this.filterDto.diseaseId },
    ];

    // Append FilterDto values to the paramaters
    params = params.concat(this.getFilterParamsDtos(this.filterDto));

    this.dialog.open(ConfirmExportComponent, {
      data: {
        url: DiseaseTestingProtocolsController.ExportDiseaseTestingProtocols,
        fileName: 'DiseaseTestingProtocols',
        queryParamsDtos: params,
      }
    });
  }

}
