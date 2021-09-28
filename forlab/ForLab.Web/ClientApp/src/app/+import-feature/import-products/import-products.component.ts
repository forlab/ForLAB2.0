import { Component, OnInit, ViewChild, Injector, ViewEncapsulation, Input, ChangeDetectionStrategy } from '@angular/core';
import { TableColumn } from 'src/@core/models/common/response';
import { SelectionModel } from '@angular/cdk/collections';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { BaseService } from 'src/@core/services/base.service';
import { MatPaginator } from '@angular/material/paginator';
import { ProductDto } from 'src/@core/models/product/Product';
import { ProductsController } from 'src/@core/APIs/ProductsController';
import { map, takeUntil } from 'rxjs/operators';
import { VendorDto } from 'src/@core/models/vendor/Vendor';
import { ProductTypeEnum, ProductBasicUnitEnum } from 'src/@core/models/enum/Enums';
import { VendorsController } from 'src/@core/APIs/VendorsController';
import { RxwebValidators } from '@rxweb/reactive-form-validators';
import { FormValidationError } from 'src/@core/models/common/SharedModel';
import { TableVirtualScrollDataSource } from 'ng-table-virtual-scroll';
import { combineLatest } from 'rxjs';

@Component({
  selector: 'import-products',
  templateUrl: './import-products.component.html',
  styleUrls: ['./import-products.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ImportProductsComponent extends BaseService implements OnInit {
  // Drp
  vendors: VendorDto[] = [];
  productTypeEnum = ProductTypeEnum;
  productBasicUnitEnum = ProductBasicUnitEnum;
  // Table
  @Input('data') data: ProductDto[] = [];
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  columns: TableColumn<ProductDto>[] = [
    { label: 'Actions', property: 'actions', type: 'button', visible: true },
    { label: '#', property: 'index', type: 'custom', visible: true },
    { label: 'Name', property: 'name', type: 'text', visible: true },
    { label: 'Vendor', property: 'vendorId', type: 'select', visible: true },
    { label: 'Product Type', property: 'productTypeId', type: 'select', visible: true },
    { label: 'Catalog Number', property: 'catalogNo', type: 'text', visible: true },
    { label: 'Basic Unit', property: 'productBasicUnitId', type: 'select', visible: true },
    { label: 'Manufacturer Price', property: 'manufacturerPrice', type: 'number', visible: true },
    { label: 'Pack Size', property: 'packSize', type: 'number', visible: true },
  ];
  dataSource = new TableVirtualScrollDataSource();
  selection = new SelectionModel<ProductDto>(true, []);
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

    combineLatest([this.loadVendors()])
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(([vendors]) => {
        // Set Drp data
        this.vendors = vendors;
        // Convert Enum to List
        const productTypes = this.convertEnumToList(this.productTypeEnum);
        const productBasicUnits = this.convertEnumToList(this.productBasicUnitEnum);

        // Patch the Table Data
        this.formArry = Array.from(Array(this.data.length), (_, i) => i + 1).map(x => {
          let form = this.appendObject();

          // Vendor
          this.data[x - 1].vendorId = vendors.find(item => item.name.trim().toLowerCase() == this.data[x - 1].vendorName.trim().toLowerCase())?.id;
          // ProductType
          this.data[x - 1].productTypeId = productTypes.find(item => item.name.trim().toLowerCase() == this.data[x - 1].productTypeName.trim().toLowerCase())?.id;
          // ProductBasicUnit
          this.data[x - 1].productBasicUnitId = productBasicUnits.find(item => item.name.trim().toLowerCase() == this.data[x - 1].productBasicUnitName.trim().toLowerCase())?.id;

          // Patch FormGroup Value
          form.patchValue(this.data[x - 1]);
          return form;
        });

        this.loadingData = false;
        this.dataSource = new TableVirtualScrollDataSource(this.formArry);
        this.dataSource.paginator = this.paginator;
      });

  }

  loadVendors() {
    return this.httpService.GET(VendorsController.GetAllAsDrp).pipe(map(res => res.data));
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
      name: new FormControl(null, [Validators.required, RxwebValidators.unique()]),
      vendorId: new FormControl(null, [Validators.required]),
      manufacturerPrice: new FormControl(null, [Validators.required]),
      productTypeId: new FormControl(null, [Validators.required]),
      catalogNo: new FormControl(null, [Validators.required]),
      productBasicUnitId: new FormControl(null, [Validators.required]),
      packSize: new FormControl(null, [Validators.required]),
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
    let objectDtos: ProductDto[] = [];

    objectDtos = this.formArry.map(element => {
      let val = new ProductDto();
      val.name = String(element.value.name);
      val.vendorId = Number(element.value.vendorId);
      val.productTypeId = Number(element.value.productTypeId);
      val.productBasicUnitId = Number(element.value.productBasicUnitId);
      val.manufacturerPrice = Number(element.value.manufacturerPrice);
      val.catalogNo = String(element.value.catalogNo);
      val.packSize = Number(element.value.packSize);
      return val;
    });

    this.httpService.POST(ProductsController.ImportProducts, objectDtos)
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
    const rowNumber = (this.paginator.pageIndex * this.paginator.pageSize) + (i+1);
    return this.formatInt(rowNumber , false)
  }
  
}
