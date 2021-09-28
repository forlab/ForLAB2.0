import { Component, ElementRef, OnInit, ViewChild, Injector, Input } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { fromEvent, Observable } from 'rxjs';
import { SelectionModel } from '@angular/cdk/collections';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { ConfirmDeleteComponent } from 'src/@core/directives/confirm-delete/confirm-delete.component';
import { RegionProductPriceDto, RegionProductPriceFilterDto } from 'src/@core/models/product/RegionProductPrice';
import { TableColumn } from 'src/@core/models/common/response';
import { RegionProductPricesController } from 'src/@core/APIs/RegionProductPricesController';
import { ObjectDataSource } from 'src/@core/services/object.datasource';
import { takeUntil, tap, debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { ConfirmActiveComponent } from 'src/@core/directives/confirm-active/confirm-active.component';
import { BaseService } from 'src/@core/services/base.service';
import { AddEditRegionProductPriceComponent } from '../add-edit-region-product-price/add-edit-region-product-price.component';
import { CountriesController } from 'src/@core/APIs/CountriesController';
import { ProductsController } from 'src/@core/APIs/ProductsController';
import { RegionsController } from 'src/@core/APIs/RegionsController';
import { ConfirmExportComponent } from 'src/@core/directives/confirm-export/confirm-export.component';


@Component({
  selector: 'app-region-product-prices',
  templateUrl: './region-product-prices.component.html',
  styleUrls: ['./region-product-prices.component.sass']
})
export class RegionProductPricesComponent extends BaseService implements OnInit {
  // Inputs to reuse the component in other components
  @Input('fromProductDetails') fromProductDetails: boolean = false;
  @Input('productId') productId: number;
  @Input('productCreatedBy') productCreatedBy: number;

  // Drp
  countries$: Observable<any[]>;
  regions$: Observable<any[]>;
  products$: Observable<any[]>;

  filterDto: RegionProductPriceFilterDto = new RegionProductPriceFilterDto();

  // Table
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort;
  @ViewChild('searchInput', { static: false }) searchInput: ElementRef;
  columns: TableColumn<RegionProductPriceDto>[] = [
    { label: 'Actions', property: 'actions', type: 'button', visible: true },
    // { label: 'Select', property: 'select', type: 'button', visible: false },
    { label: 'Status', property: 'isActive', type: 'button', visible: true },
    { label: 'Product', property: 'productName', type: 'text', visible: true },
    { label: 'Country', property: 'regionCountryName', type: 'text', visible: true },
    { label: 'Region', property: 'regionName', type: 'text', visible: true },
    { label: 'Price', property: 'price', type: 'price', visible: true },
    { label: 'Pack Size', property: 'packSize', type: 'int', visible: true },
    { label: 'From Date', property: 'fromDate', type: 'date', visible: true },
    { label: 'Creator', property: 'creator', type: 'text', visible: false },
    { label: 'Created On', property: 'createdOn', type: 'datetime', visible: false },
  ];
  dataSource: ObjectDataSource;
  selection = new SelectionModel<RegionProductPriceDto>(true, []);

  constructor(public injector: Injector) {
    super(injector);
  }

  ngOnInit() {

    // Check
    if (this.fromProductDetails) {
      const index = this.columns.findIndex(x => x.property == 'productName');
      this.columns.splice(index, 1);
      this.filterDto.productId = this.productId;
    }

    // Load Drp
    if (!this.fromProductDetails) {
      this.loadProducts();
    }
    this.loadCountries();

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

  loadCountries() {
    this.countries$ = this.httpService.GET(CountriesController.GetAllAsDrp).pipe(map(res => res.data));
  }
  loadRegions(countryId: number) {
    const url = RegionsController.GetAllAsDrp;
    let params: QueryParamsDto[] = [
      { key: 'countryId', value: countryId }
    ];
    this.regions$ = this.httpService.GET(url, params).pipe(map(res => res.data));
  }
  loadProducts() {
    this.products$ = this.httpService.GET(ProductsController.GetAllAsDrp).pipe(map(res => res.data));
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
      { key: 'productName', value: this.searchInput.nativeElement.value },
    ];

    // Append FilterDto values to the paramaters
    params = params.concat(this.getFilterParamsDtos(this.filterDto));

    this.dataSource.loadObjects(RegionProductPricesController.GetAll, params);
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
    this.dialog.open(AddEditRegionProductPriceComponent, {
      data: {
        productId: this.productId,
        fromProductDetails: this.fromProductDetails,
        regionProductPriceDto: new RegionProductPriceDto()
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

  updateObject(regionProductPrice: RegionProductPriceDto) {
    this.dialog.open(AddEditRegionProductPriceComponent, {
      data: {
        productId: this.productId,
        fromProductDetails: this.fromProductDetails,
        regionProductPriceDto: regionProductPrice
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

  updateIsActive(regionProductPrice: RegionProductPriceDto) {
    this.dialog.open(ConfirmActiveComponent, {
      data: {
        url: RegionProductPricesController.UpdateIsActive,
        objectInfo: [{ key: 'Product', value: regionProductPrice.productName }, { key: 'Region', value: regionProductPrice.regionName }, { key: 'Price', value: regionProductPrice.price }],
        isActive: !regionProductPrice.isActive,
        queryParamsDto: { key: 'regionProductPriceId', value: regionProductPrice.id },
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

  deleteObject(regionProductPrice: RegionProductPriceDto) {
    this.dialog.open(ConfirmDeleteComponent, {
      data: {
        url: RegionProductPricesController.RemoveRegionProductPrice,
        objectInfo: [{ key: 'Product', value: regionProductPrice.productName }, { key: 'Region', value: regionProductPrice.regionName }, { key: 'Price', value: regionProductPrice.price }],
        queryParamsDto: { key: 'regionProductPriceId', value: regionProductPrice.id },
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
      { key: 'name', value: this.searchInput.nativeElement.value },
    ];

    // Append FilterDto values to the paramaters
    params = params.concat(this.getFilterParamsDtos(this.filterDto));

    this.dialog.open(ConfirmExportComponent, {
      data: {
        url: RegionProductPricesController.ExportRegionProductPrices,
        fileName: 'RegionProductPrices',
        queryParamsDtos: params,
      }
    });
  }

}
