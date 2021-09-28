import { Component, OnInit, ViewChild, Injector, ViewEncapsulation, Input, ChangeDetectionStrategy } from '@angular/core';
import { TableColumn } from 'src/@core/models/common/response';
import { SelectionModel } from '@angular/cdk/collections';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { BaseService } from 'src/@core/services/base.service';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { CountryProductPriceDto } from 'src/@core/models/product/CountryProductPrice';
import { CountryDto } from 'src/@core/models/lookup/Country';
import { CountryProductPricesController } from 'src/@core/APIs/CountryProductPricesController';
import { map, takeUntil } from 'rxjs/operators';
import { CountriesController } from 'src/@core/APIs/CountriesController';
import { ProductDto } from 'src/@core/models/product/Product';
import { ProductsController } from 'src/@core/APIs/ProductsController';
import * as moment from 'moment';
import { RxwebValidators } from '@rxweb/reactive-form-validators';
import { FormValidationError } from 'src/@core/models/common/SharedModel';
import { TableVirtualScrollDataSource } from 'ng-table-virtual-scroll';
import { combineLatest } from 'rxjs';

@Component({
  selector: 'import-country-product-prices',
  templateUrl: './import-country-product-prices.component.html',
  styleUrls: ['./import-country-product-prices.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ImportCountryProductPricesComponent extends BaseService implements OnInit {
  // Drp
  countries: CountryDto[] = [];
  products: ProductDto[] = [];
  // Table
  @Input('data') data: CountryProductPriceDto[] = [];
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort;
  columns: TableColumn<CountryProductPriceDto>[] = [
    { label: 'Actions', property: 'actions', type: 'button', visible: true },
    { label: '#', property: 'index', type: 'custom', visible: true },
    { label: 'Country', property: 'countryId', type: 'select', visible: true },
    { label: 'Product', property: 'productId', type: 'select', visible: true },
    { label: 'Price', property: 'price', type: 'number', visible: true },
    { label: 'Pack Size', property: 'packSize', type: 'number', visible: true },
    { label: 'From Date', property: 'fromDate', type: 'date', visible: true },
  ];
  dataSource = new TableVirtualScrollDataSource();
  selection = new SelectionModel<CountryProductPriceDto>(true, []);
  // Form
  formArry: FormGroup[];
  // vars
  loadingData: boolean = true;
  errors: FormValidationError[] = [];
  successRes: any;
  done: boolean = false;

  constructor(private fb: FormBuilder, public injector: Injector) {
    super(injector);
  }

  ngOnInit() {

    combineLatest([this.loadCountries(), this.loadProducts()])
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(([countries, products]) => {
        // Set Drp data
        this.countries = countries;
        this.products = products;

        // Patch the Table Data
        this.formArry = Array.from(Array(this.data.length), (_, i) => i + 1).map(x => {
          let form = this.appendObject();

          // Country
          this.data[x - 1].countryId = countries.find(item => item.name.trim().toLowerCase() == this.data[x - 1].countryName.trim().toLowerCase())?.id;
          // Product
          this.data[x - 1].productId = products.find(item => item.name.trim().toLowerCase() == this.data[x - 1].productName.trim().toLowerCase())?.id;

          // Patch FormGroup Value
          form.patchValue(this.data[x - 1]);
          return form;
        });

        this.loadingData = false;
        this.dataSource = new TableVirtualScrollDataSource(this.formArry);
        this.dataSource.paginator = this.paginator;
      });

  }

  loadCountries() {
    return this.httpService.GET(CountriesController.GetAllAsDrp).pipe(map(res => res.data));
  }
  loadProducts() {
    return this.httpService.GET(ProductsController.GetAllAsDrp).pipe(map(res => res.data));
  }

  get visibleColumns() {
    if (!this.columns) {
      return [];
    }
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  toggleColumnVisibility(column, event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    column.visible = !column.visible;
  }

  trackByProperty<T>(index: number, column: TableColumn<T>) {
    return column.property;
  }

  appendObject() {
    return this.fb.group({
      countryId: [null, Validators.compose([Validators.required])],
      productId: [null, Validators.compose([Validators.required])],
      price: [null, Validators.compose([Validators.required, RxwebValidators.minNumber({ value: 0 })])],
      packSize: [null, Validators.compose([Validators.required])],
      fromDate: [null, Validators.compose([Validators.required])],
    })
  }

  addObject() {
    this.formArry.unshift(this.appendObject());
    this.dataSource = new TableVirtualScrollDataSource(this.formArry);
    this.dataSource.paginator = this.paginator;
  }

  deleteObject(row) {
    const index = this.formArry.indexOf(row);
    this.formArry.splice(index, 1);
    this.dataSource = new TableVirtualScrollDataSource(this.formArry);
    this.dataSource.paginator = this.paginator;
  }

  submit() {
    const invalidFormGroups = this.formArry?.filter(x => x.invalid);
    /** check form */
    this.errors = [];
    if (invalidFormGroups?.length > 0) {
      invalidFormGroups.forEach(x => x.markAllAsTouched());
      this.errors = this.getFormGroupsErrors(invalidFormGroups, this.formArry);
      return;
    }

    this.loading = true;
    let objectDtos: CountryProductPriceDto[] = [];

    objectDtos = this.formArry.map(element => {
      let val = new CountryProductPriceDto();
      val.countryId = Number(element.value.countryId);
      val.productId = Number(element.value.productId);
      val.price = Number(element.value.price);
      val.packSize = Number(element.value.packSize);
      val.fromDate = moment(element.value.fromDate).add(1, 'd').toISOString();
      return val;
    });

    this.httpService.POST(CountryProductPricesController.ImportCountryProductPrices, objectDtos)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {

        if (res.isPassed) {
          this.successRes = res.data;
          this.done = true;
          this.alertService.success('Uploaded success');
          this._ref.detectChanges();
        } else {
          if (res.data) {
            this.errors = res.data;
          }
          this.loading = false;
          this.alertService.error(res.message)
          this._ref.detectChanges();
        }

      }, err => {
        this.alertService.exception();
        this.loading = false;
        this._ref.detectChanges();
      });
  }

  getRowNumber(i: number): number {
    const rowNumber = (this.paginator.pageIndex * this.paginator.pageSize) + (i + 1);
    return this.formatInt(rowNumber, false)
  }

}
