import { Component, OnInit, ViewChild, Injector, ViewEncapsulation, Input, ChangeDetectionStrategy } from '@angular/core';
import { QueryParamsDto, TableColumn } from 'src/@core/models/common/response';
import { SelectionModel } from '@angular/cdk/collections';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { BaseService } from 'src/@core/services/base.service';
import { MatPaginator } from '@angular/material/paginator';
import { ProductUsagesController } from 'src/@core/APIs/ProductUsagesController';
import { map, takeUntil } from 'rxjs/operators';
import { combineLatest } from 'rxjs';
import { FormValidationError } from 'src/@core/models/common/SharedModel';
import { TableVirtualScrollDataSource } from 'ng-table-virtual-scroll';
import { ProductDto } from 'src/@core/models/product/Product';
import { TestDto } from 'src/@core/models/testing/Test';
import { InstrumentDto } from 'src/@core/models/product/Instrument';
import { CountryPeriodEnum, ProductTypeEnum } from 'src/@core/models/enum/Enums';
import { ProductUsageDto } from 'src/@core/models/product/ProductUsage';
import { ProductsController } from 'src/@core/APIs/ProductsController';
import { TestsController } from 'src/@core/APIs/TestsController';
import { InstrumentsController } from 'src/@core/APIs/InstrumentsController';
import { RxwebValidators } from '@rxweb/reactive-form-validators';

@Component({
  selector: 'import-product-usage',
  templateUrl: './import-product-usage.component.html',
  styleUrls: ['./import-product-usage.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ImportProductUsageComponent extends BaseService implements OnInit {
  // Input
  @Input('fromProductDetails') fromProductDetails: boolean = false;
  @Input('fromTestDetails') fromTestDetails: boolean = false;
  // Drp
  products: ProductDto[];
  tests: TestDto[];
  instruments: InstrumentDto[];
  countryPeriodEnum = CountryPeriodEnum;
  productTypeEnum = ProductTypeEnum;
  // Table
  @Input('data') data: ProductUsageDto[] = [];
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  columns: TableColumn<ProductUsageDto>[] = [];
  dataSource = new TableVirtualScrollDataSource();
  selection = new SelectionModel<ProductUsageDto>(true, []);
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

    // Check
    if (this.fromProductDetails) {
      this.columns = [
        { label: 'Actions', property: 'actions', type: 'button', visible: true },
        { label: '#', property: 'index', type: 'custom', visible: true },
        { label: 'Product', property: 'productId', type: 'select', visible: true },
        { label: 'Per Period', property: 'perPeriod', type: 'select', visible: true },
        { label: 'Country Period', property: 'countryPeriodId', type: 'select', visible: true },
        { label: 'Per Period Per Instrument', property: 'perPeriodPerInstrument', type: 'select', visible: true },
        { label: 'Instrument', property: 'instrumentId', type: 'select', visible: true },
        { label: 'Amount', property: 'amount', type: 'number', visible: true },
      ];
    } else if (this.fromTestDetails) {
      this.columns = [
        { label: 'Actions', property: 'actions', type: 'button', visible: true },
        { label: '#', property: 'index', type: 'custom', visible: true },
        { label: 'Product', property: 'productId', type: 'select', visible: true },
        { label: 'Instrument', property: 'instrumentId', type: 'select', visible: true },
        { label: 'Test', property: 'testId', type: 'select', visible: true },
        { label: 'For Control', property: 'isForControl', type: 'select', visible: true },
        { label: 'Amount', property: 'amount', type: 'number', visible: true },
      ];
    }

    combineLatest([this.loadProducts(), this.loadTests(), this.loadInstruments()])
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(([products, tests, instruments]) => {
        // Convert Enum to List
        const countryPeriods = this.convertEnumToList(this.countryPeriodEnum);
        // Set Drp data
        this.products = products;
        this.tests = tests;
        this.instruments = instruments;

        // Patch the Table Data
        this.formArry = Array.from(Array(this.data.length), (_, i) => i + 1).map(x => {
          let form = this.appendObject();

          // CountryPeriod
          this.data[x - 1].countryPeriodId = countryPeriods.find(item => item.name.trim().toLowerCase() == this.data[x - 1].countryPeriodName?.trim().toLowerCase())?.id;
          // Product
          this.data[x - 1].productId = products.find(item => item.name.trim().toLowerCase() == this.data[x - 1].productName?.trim().toLowerCase())?.id;
          // Test
          this.data[x - 1].testId = tests.find(item => item.name.trim().toLowerCase() == this.data[x - 1].testName?.trim().toLowerCase())?.id;
          // Product
          this.data[x - 1].instrumentId = instruments.find(item => item.name.trim().toLowerCase() == this.data[x - 1].instrumentName?.trim().toLowerCase())?.id;

          // Patch FormGroup Value
          form.patchValue(this.data[x - 1]);
          return form;
        });

        this.loadingData = false;
        this.dataSource = new TableVirtualScrollDataSource(this.formArry);
        this.dataSource.paginator = this.paginator;
      });

  }

  loadProducts() {
    let params: QueryParamsDto[] = [];
    if (this.fromTestDetails) {
      params.push({ key: 'productTypeIds', value: [this.productTypeEnum.QualityControl.toString(), this.productTypeEnum.Reagents.toString()].join(',') });
    } else if (this.fromProductDetails) {
      params.push({ key: 'productTypeIds', value: [this.productTypeEnum.Consumables.toString(), this.productTypeEnum.Calibrators.toString()].join(',') });
    }

    return this.httpService.GET(ProductsController.GetAllAsDrp, params).pipe(map(res => res.data));
  }
  loadTests() {
    return this.httpService.GET(TestsController.GetAllAsDrp).pipe(map(res => res.data));
  }
  loadInstruments() {
    return this.httpService.GET(InstrumentsController.GetAllAsDrp).pipe(map(res => res.data));
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
    if (this.fromProductDetails) {
      return this.fb.group({
        testId: [null],
        instrumentId: [0, Validators.compose([Validators.required])],
        productId: [null, Validators.compose([Validators.required])],
        amount: [null, Validators.compose([Validators.required, RxwebValidators.minNumber({ value: 0 })])],
        isForControl: [null],
        perPeriod: [false, Validators.compose([Validators.required])],
        perPeriodPerInstrument: [false, Validators.compose([Validators.required])],
        countryPeriodId: [0, Validators.compose([Validators.required])],
      });
    } else if (this.fromTestDetails) {
      return this.fb.group({
        testId: [null, Validators.compose([Validators.required])],
        instrumentId: [null, Validators.compose([Validators.required])],
        productId: [null, Validators.compose([Validators.required])],
        amount: [null, Validators.compose([Validators.required, RxwebValidators.minNumber({ value: 0 })])],
        isForControl: [false, Validators.compose([Validators.required])],
        perPeriod: [null],
        perPeriodPerInstrument: [null],
        countryPeriodId: [null],
      });
    }
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
    let objectDtos: ProductUsageDto[] = [];

    objectDtos = this.formArry.map(element => {
      let val = new ProductUsageDto();
      val.testId = this.fromTestDetails ? Number(element.value.testId) : null;
      val.instrumentId = Number(element.value.instrumentId);
      val.productId = Number(element.value.productId);
      val.amount = Number(element.value.amount);
      val.isForControl = this.fromTestDetails ? Boolean(element.value.isForControl) : false;
      val.perPeriod = this.fromProductDetails ? Boolean(element.value.perPeriod) : false;
      val.perPeriodPerInstrument = this.fromProductDetails ? Boolean(element.value.perPeriodPerInstrument) : false;
      val.countryPeriodId = this.fromProductDetails ? Number(element.value.countryPeriodId) : null;

      // Check
      if (val.perPeriod || val.instrumentId == 0) {
        val.instrumentId = null;
      }
      if (val.countryPeriodId == 0 || !val.perPeriod) {
        val.countryPeriodId = null;
      }

      return val;
    });


    let params: QueryParamsDto[] = [
      { key: 'isProduct', value: this.fromProductDetails }
    ];
    this.httpService.POST(ProductUsagesController.ImportProductUsages, objectDtos, params)
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
