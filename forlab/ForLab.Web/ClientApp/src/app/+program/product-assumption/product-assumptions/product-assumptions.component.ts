import { Component, ElementRef, OnInit, ViewChild, Injector, Input } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { fromEvent, Observable } from 'rxjs';
import { SelectionModel } from '@angular/cdk/collections';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { ConfirmDeleteComponent } from 'src/@core/directives/confirm-delete/confirm-delete.component';
import { TableColumn } from 'src/@core/models/common/response';
import { ObjectDataSource } from 'src/@core/services/object.datasource';
import { takeUntil, tap, debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { ConfirmActiveComponent } from 'src/@core/directives/confirm-active/confirm-active.component';
import { BaseService } from 'src/@core/services/base.service';
import { AddEditProductAssumptionComponent } from '../add-edit-product-assumption/add-edit-product-assumption.component';
import { ConfirmExportComponent } from 'src/@core/directives/confirm-export/confirm-export.component';
import { ProgramsController } from 'src/@core/APIs/ProgramsController';
import { ProductAssumptionParameterFilterDto, ProductAssumptionParameterDto } from 'src/@core/models/program/ProductAssumptionParameter';
import { ProductAssumptionParametersController } from 'src/@core/APIs/ProductAssumptionParametersController';

@Component({
  selector: 'product-assumptions',
  templateUrl: './product-assumptions.component.html',
  styleUrls: ['./product-assumptions.component.scss']
})
export class ProductAssumptionsComponent extends BaseService implements OnInit {
  // Inputs to reuse the component in other components
  @Input('fromProgramDetails') fromProgramDetails: boolean = false;
  @Input('programId') programId: number;
  @Input('programCreatedBy') programCreatedBy: number;

  // Drp
  programs$: Observable<any[]>;

  filterDto: ProductAssumptionParameterFilterDto = new ProductAssumptionParameterFilterDto();

  // Table
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort;
  @ViewChild('searchInput', { static: false }) searchInput: ElementRef;
  columns: TableColumn<ProductAssumptionParameterDto>[] = [
    { label: 'Actions', property: 'actions', type: 'button', visible: true },
    // { label: 'Select', property: 'select', type: 'button', visible: false },
    { label: 'Status', property: 'isActive', type: 'button', visible: true },
    { label: 'Program', property: 'programName', type: 'text', visible: true },
    { label: 'Name', property: 'name', type: 'text', visible: true },
    { label: 'Type', property: 'isPercentage', type: 'custom', visible: true },
    { label: 'Sign', property: 'isPositive', type: 'custom', visible: true },
    { label: 'Creator', property: 'creator', type: 'text', visible: true },
    { label: 'Created On', property: 'createdOn', type: 'datetime', visible: true },
  ];
  dataSource: ObjectDataSource;
  selection = new SelectionModel<ProductAssumptionParameterDto>(true, []);
  skippedOnReset: string[] = [];

  constructor(public injector: Injector) {
    super(injector);

  }

  ngOnInit() {

    // Check
    if (this.fromProgramDetails) {
      const index = this.columns.findIndex(x => x.property == 'programName');
      this.columns.splice(index, 1);
      this.filterDto.programId = this.programId;
      this.skippedOnReset.push('programId');
    }

    // Load Drp
    if (!this.fromProgramDetails) {
      this.loadPrograms();
    }

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

  loadPrograms() {
    this.programs$ = this.httpService.GET(ProgramsController.GetAllAsDrp).pipe(map(res => res.data));
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

    this.dataSource.loadObjects(ProductAssumptionParametersController.GetAll, params);
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
    if (this.filterDto != null) {
      Object.keys(this.filterDto).forEach(key => {
        if (!this.skippedOnReset.includes(key)) {
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
    this.dialog.open(AddEditProductAssumptionComponent, {
      data: {
        programId: this.programId,
        fromProgramDetails: this.fromProgramDetails,
        productAssumptionParameterDto: new ProductAssumptionParameterDto()
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

  updateObject(productAssumptionParameter: ProductAssumptionParameterDto) {
    this.dialog.open(AddEditProductAssumptionComponent, {
      data: {
        programId: this.programId,
        fromProgramDetails: this.fromProgramDetails,
        productAssumptionParameterDto: productAssumptionParameter
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

  updateIsActive(productAssumptionParameter: ProductAssumptionParameterDto) {
    this.dialog.open(ConfirmActiveComponent, {
      data: {
        url: ProductAssumptionParametersController.UpdateIsActive,
        objectInfo: [{ key: 'Program', value: productAssumptionParameter.programName }, { key: 'Name', value: productAssumptionParameter.name }],
        isActive: !productAssumptionParameter.isActive,
        queryParamsDto: { key: 'productAssumptionParameterId', value: productAssumptionParameter.id },
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

  deleteObject(productAssumptionParameter: ProductAssumptionParameterDto) {
    this.dialog.open(ConfirmDeleteComponent, {
      data: {
        url: ProductAssumptionParametersController.RemoveProductAssumptionParameter,
        objectInfo: [{ key: 'Program', value: productAssumptionParameter.programName }, { key: 'Name', value: productAssumptionParameter.name }],
        queryParamsDto: { key: 'productAssumptionParameterId', value: productAssumptionParameter.id },
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
        url: ProductAssumptionParametersController.ExportProductAssumptionParameters,
        fileName: 'ProductAssumptionParameters',
        queryParamsDtos: params,
      }
    });
  }

}
