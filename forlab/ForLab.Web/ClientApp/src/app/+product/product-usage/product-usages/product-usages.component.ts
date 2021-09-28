import { Component, ElementRef, OnInit, ViewChild, Injector, Input } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { fromEvent, Observable } from 'rxjs';
import { SelectionModel } from '@angular/cdk/collections';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { ConfirmDeleteComponent } from 'src/@core/directives/confirm-delete/confirm-delete.component';
import { ProductUsageDto, ProductUsageFilterDto } from 'src/@core/models/product/ProductUsage';
import { TableColumn } from 'src/@core/models/common/response';
import { ProductUsagesController } from 'src/@core/APIs/ProductUsagesController';
import { ObjectDataSource } from 'src/@core/services/object.datasource';
import { takeUntil, tap, debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { ConfirmActiveComponent } from 'src/@core/directives/confirm-active/confirm-active.component';
import { BaseService } from 'src/@core/services/base.service';
import { AddEditProductUsageComponent } from '../add-edit-product-usage/add-edit-product-usage.component';
import { CountryPeriodEnum, ProductTypeEnum } from 'src/@core/models/enum/Enums';
import { ConfirmExportComponent } from 'src/@core/directives/confirm-export/confirm-export.component';
import { ProductsController } from 'src/@core/APIs/ProductsController';
import { InstrumentsController } from 'src/@core/APIs/InstrumentsController';

@Component({
  selector: 'app-product-usages',
  templateUrl: './product-usages.component.html',
  styleUrls: ['./product-usages.component.scss']
})
export class ProductUsagesComponent extends BaseService implements OnInit {
  // Inputs to reuse the component in other components
  @Input('fromProductDetails') fromProductDetails: boolean = false;
  @Input('productId') productId: number;
  @Input('productCreatedBy') productCreatedBy: number;
  @Input('fromTestDetails') fromTestDetails: boolean = false;
  @Input('testId') testId: number;
  @Input('testCreatedBy') testCreatedBy: number;
  // Drp
  countryPeriodEnum = CountryPeriodEnum;
  productTypeEnum = ProductTypeEnum;
  products$: Observable<any[]>;
  instruments$: Observable<any[]>;

  filterDto: ProductUsageFilterDto = new ProductUsageFilterDto();

  // Table
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort;
  @ViewChild('searchInput', { static: false }) searchInput: ElementRef;
  columns: TableColumn<ProductUsageDto>[] = [];
  dataSource: ObjectDataSource;
  selection = new SelectionModel<ProductUsageDto>(true, []);

  constructor(public injector: Injector) {
    super(injector);
  }


  ngOnInit() {
    // Check
    if (this.fromProductDetails) {
      this.columns = [
        { label: 'Actions', property: 'actions', type: 'button', visible: true },
        { label: 'Status', property: 'isActive', type: 'button', visible: true },
        { label: 'Product', property: 'productName', type: 'text', visible: true },
        { label: 'Per Period', property: 'perPeriod', type: 'bool', visible: false },
        { label: 'Country Period', property: 'countryPeriodName', type: 'text', visible: true },
        { label: 'Per Period Per Instrument', property: 'perPeriodPerInstrument', type: 'bool', visible: false },
        { label: 'Instrument', property: 'instrumentName', type: 'text', visible: true },
        { label: 'Amount', property: 'amount', type: 'text', visible: true },
        { label: 'Creator', property: 'creator', type: 'text', visible: false },
        { label: 'Created On', property: 'createdOn', type: 'datetime', visible: false },
      ];
      this.filterDto.productId = this.productId;
      this.filterDto.productTypeIds = [this.productTypeEnum.Consumables.toString(), this.productTypeEnum.Calibrators.toString()].join(',');
    } else if (this.fromTestDetails) {
      this.columns = [
        { label: 'Actions', property: 'actions', type: 'button', visible: true },
        { label: 'Status', property: 'isActive', type: 'button', visible: true },
        { label: 'Product', property: 'productName', type: 'text', visible: true },
        { label: 'Test', property: 'testName', type: 'text', visible: true },
        { label: 'Instrument', property: 'instrumentName', type: 'text', visible: true },
        { label: 'Is for Control', property: 'isForControl', type: 'bool', visible: true },
        { label: 'Amount', property: 'amount', type: 'text', visible: true },
        { label: 'Creator', property: 'creator', type: 'text', visible: false },
        { label: 'Created On', property: 'createdOn', type: 'datetime', visible: false },
      ];
      this.filterDto.testId = this.testId;
      this.filterDto.productTypeIds = [this.productTypeEnum.QualityControl.toString(), this.productTypeEnum.Reagents.toString()].join(',');
    }

    // Load Drp
    if (!this.fromProductDetails) {
      this.loadProducts();
    }
    this.loadInstruments();

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

  loadProducts() {
    let params: QueryParamsDto[] = [];
    if (this.fromTestDetails) {
      params.push({ key: 'productTypeIds', value: [this.productTypeEnum.QualityControl.toString(), this.productTypeEnum.Reagents.toString()].join(',') });
    } else if (this.fromProductDetails) {
      params.push({ key: 'productTypeIds', value: [this.productTypeEnum.Consumables.toString(), this.productTypeEnum.Calibrators.toString()].join(',') });
    }

    this.products$ = this.httpService.GET(ProductsController.GetAllAsDrp, params).pipe(map(res => res.data));
  }
  loadInstruments() {
    this.instruments$ = this.httpService.GET(InstrumentsController.GetAllAsDrp).pipe(map(res => res.data));
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
      { key: 'amount', value: this.searchInput.nativeElement.value || this.filterDto.amount },
    ];

    // Append FilterDto values to the paramaters
    params = params.concat(this.getFilterParamsDtos(this.filterDto));

    this.dataSource.loadObjects(ProductUsagesController.GetAll, params);
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
    let skipped: string[] = ['testId', 'productId'];

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
    this.dialog.open(AddEditProductUsageComponent, {
      data: {
        productId: this.productId,
        fromProductDetails: this.fromProductDetails,
        testId: this.testId,
        fromTestDetails: this.fromTestDetails,
        productUsageDto: new ProductUsageDto()
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

  updateObject(productUsage: ProductUsageDto) {
    this.dialog.open(AddEditProductUsageComponent, {
      data: {
        productId: this.productId,
        fromProductDetails: this.fromProductDetails,
        testId: this.testId,
        fromTestDetails: this.fromTestDetails,
        productUsageDto: productUsage
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

  updateIsActive(productUsage: ProductUsageDto) {
    this.dialog.open(ConfirmActiveComponent, {
      data: {
        url: ProductUsagesController.UpdateIsActive,
        objectInfo: [{ key: 'Product', value: productUsage.productName }, { key: 'Test', value: productUsage.testName }, { key: 'Instrument', value: productUsage.instrumentName }],
        isActive: !productUsage.isActive,
        queryParamsDto: { key: 'productUsageId', value: productUsage.id },
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

  deleteObject(productUsage: ProductUsageDto) {
    this.dialog.open(ConfirmDeleteComponent, {
      data: {
        url: ProductUsagesController.RemoveProductUsage,
        objectInfo: [{ key: 'Product', value: productUsage.productName }, { key: 'Test', value: productUsage.testName }, { key: 'Instrument', value: productUsage.instrumentName }],
        queryParamsDto: { key: 'productUsageId', value: productUsage.id },
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
      { key: 'amount', value: this.searchInput.nativeElement.value || this.filterDto.amount },
      { key: 'exportProductUsage', value: this.fromProductDetails ? true : false },
    ];

    // Append FilterDto values to the paramaters
    params = params.concat(this.getFilterParamsDtos(this.filterDto));

    this.dialog.open(ConfirmExportComponent, {
      data: {
        url: ProductUsagesController.ExportProductUsages,
        fileName: 'ProductUsages',
        queryParamsDtos: params,
      }
    });
  }

  navigateToImport() {
    if (this.fromProductDetails) {
      this.router.navigate(['/import-feature/index'], { queryParams: { objectName: 'ProductUsage' } });
    } else if (this.fromTestDetails) {
      this.router.navigate(['/import-feature/index'], { queryParams: { objectName: 'TestUsage' } });
    }
  }
}
