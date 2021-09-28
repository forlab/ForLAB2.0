import { Component, ElementRef, OnInit, ViewChild, Injector, Input } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { fromEvent, Observable } from 'rxjs';
import { SelectionModel } from '@angular/cdk/collections';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { ConfirmDeleteComponent } from 'src/@core/directives/confirm-delete/confirm-delete.component';
import { CountryProductPriceDto, CountryProductPriceFilterDto } from 'src/@core/models/product/CountryProductPrice';
import { TableColumn } from 'src/@core/models/common/response';
import { CountryProductPricesController } from 'src/@core/APIs/CountryProductPricesController';
import { ObjectDataSource } from 'src/@core/services/object.datasource';
import { takeUntil, tap, debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { ConfirmActiveComponent } from 'src/@core/directives/confirm-active/confirm-active.component';
import { BaseService } from 'src/@core/services/base.service';
import { AddEditCountryProductPriceComponent } from '../add-edit-country-product-price/add-edit-country-product-price.component';
import { ProductsController } from 'src/@core/APIs/ProductsController';
import { ConfirmExportComponent } from 'src/@core/directives/confirm-export/confirm-export.component';
import { UserSubscriptionsController } from 'src/@core/APIs/UserSubscriptionsController';


@Component({
  selector: 'app-country-product-prices',
  templateUrl: './country-product-prices.component.html',
  styleUrls: ['./country-product-prices.component.sass']
})
export class CountryProductPricesComponent extends BaseService implements OnInit {
  // Inputs to reuse the component in other components
  @Input('fromProductDetails') fromProductDetails: boolean = false;
  @Input('productId') productId: number;
  @Input('productCreatedBy') productCreatedBy: number;

  // Drp
  countries$: Observable<any[]>;
  products$: Observable<any[]>;

  filterDto: CountryProductPriceFilterDto = new CountryProductPriceFilterDto();

  // Table
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort;
  @ViewChild('searchInput', { static: false }) searchInput: ElementRef;
  columns: TableColumn<CountryProductPriceDto>[] = [
    { label: 'Actions', property: 'actions', type: 'button', visible: true },
    // { label: 'Select', property: 'select', type: 'button', visible: false },
    { label: 'Status', property: 'isActive', type: 'button', visible: true },
    { label: 'Product', property: 'productName', type: 'text', visible: true },
    { label: 'Country', property: 'countryName', type: 'text', visible: true },
    { label: 'Price', property: 'price', type: 'price', visible: true },
    { label: 'Pack Size', property: 'packSize', type: 'int', visible: true },
    { label: 'From Date', property: 'fromDate', type: 'date', visible: true },
    { label: 'Creator', property: 'creator', type: 'text', visible: false },
    { label: 'Created On', property: 'createdOn', type: 'datetime', visible: false },
  ];
  dataSource: ObjectDataSource;
  selection = new SelectionModel<CountryProductPriceDto>(true, []);

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
    let params: QueryParamsDto[] = [
      { key: 'applicationUserId', value: this.loggedInUser.id },
    ];
    this.countries$ = this.httpService.GET(UserSubscriptionsController.GetUserCountriesAsDrp, params).pipe(map(res => res.data));
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

    this.dataSource.loadObjects(CountryProductPricesController.GetAll, params);
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
    this.dialog.open(AddEditCountryProductPriceComponent, {
      data: {
        productId: this.productId,
        fromProductDetails: this.fromProductDetails,
        countryProductPriceDto: new CountryProductPriceDto()
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

  updateObject(countryProductPrice: CountryProductPriceDto) {
    this.dialog.open(AddEditCountryProductPriceComponent, {
      data: {
        productId: this.productId,
        fromProductDetails: this.fromProductDetails,
        countryProductPriceDto: countryProductPrice
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

  updateIsActive(countryProductPrice: CountryProductPriceDto) {
    this.dialog.open(ConfirmActiveComponent, {
      data: {
        url: CountryProductPricesController.UpdateIsActive,
        objectInfo: [{ key: 'Product', value: countryProductPrice.productName }, { key: 'Country', value: countryProductPrice.countryName }, { key: 'Price', value: countryProductPrice.price }],
        isActive: !countryProductPrice.isActive,
        queryParamsDto: { key: 'countryProductPriceId', value: countryProductPrice.id },
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

  deleteObject(countryProductPrice: CountryProductPriceDto) {
    this.dialog.open(ConfirmDeleteComponent, {
      data: {
        url: CountryProductPricesController.RemoveCountryProductPrice,
        objectInfo: [{ key: 'Product', value: countryProductPrice.productName }, { key: 'Country', value: countryProductPrice.countryName }, { key: 'Price', value: countryProductPrice.price }],
        queryParamsDto: { key: 'countryProductPriceId', value: countryProductPrice.id },
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
      { key: 'productName', value: this.searchInput.nativeElement.value },
    ];

    // Append FilterDto values to the paramaters
    params = params.concat(this.getFilterParamsDtos(this.filterDto));

    this.dialog.open(ConfirmExportComponent, {
      data: {
        url: CountryProductPricesController.ExportCountryProductPrices,
        fileName: 'CountryProductPrices',
        queryParamsDtos: params,
      }
    });
  }

}
