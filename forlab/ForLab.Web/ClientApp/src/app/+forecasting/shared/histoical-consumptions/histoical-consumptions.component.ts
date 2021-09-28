import { Component, OnInit, Injector, ViewEncapsulation, ViewChild, Input } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { TableColumn, QueryParamsDto } from 'src/@core/models/common/response';
import * as XLSX from 'xlsx';
import { MatPaginator } from '@angular/material/paginator';
import { SelectionModel } from '@angular/cdk/collections';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { RxwebValidators } from '@rxweb/reactive-form-validators';
import { map, takeUntil } from 'rxjs/operators';
import { UserSubscriptionsController } from 'src/@core/APIs/UserSubscriptionsController';
import { LaboratoryDto } from 'src/@core/models/lookup/Laboratory';
import { HistoicalConsumptionDto } from 'src/@core/models/forecasting/ImportedFileTemplate';
import { RegionDto } from 'src/@core/models/lookup/Region';
import { ProductDto } from 'src/@core/models/product/Product';
import { ProductsController } from 'src/@core/APIs/ProductsController';
import { FormValidationError } from 'src/@core/models/common/SharedModel';
import { TableVirtualScrollDataSource } from 'ng-table-virtual-scroll';
import { combineLatest } from 'rxjs';
import { MaxNumOfExcelRecords } from 'src/@core/config';

@Component({
  selector: 'histoical-consumptions',
  templateUrl: './histoical-consumptions.component.html',
  styleUrls: ['./histoical-consumptions.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class HistoicalConsumptionsComponent extends BaseService implements OnInit {

  // Inputs
  @Input('countryId') countryId: number = 0;
  @Input('data') data: any[] = [];
  @Input('isAggregate') isAggregate: boolean = false;
  // Drp
  products: ProductDto[] = [];
  userRegions: RegionDto[] = [];
  userLaboratories: LaboratoryDto[] = [];
  // Excel File
  sheetNames: string[] = [];
  selectedSheetName: string;
  workbook: XLSX.WorkBook;
  // Flags
  maxNumOfExcelRecords: number = MaxNumOfExcelRecords;
  isValidSheetName: boolean = true;
  isActive: boolean = true;
  // Table
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  columns: TableColumn<HistoicalConsumptionDto>[] = [
    { label: 'Actions', property: 'actions', type: 'button', visible: true },
    { label: '#', property: 'index', type: 'custom', visible: true },
    { label: 'Region', property: 'regionId', type: 'select', visible: true },
    { label: 'Laboratory', property: 'laboratoryId', type: 'select', visible: true },
    { label: 'Forecast Category', property: 'forecastCategoryName', type: 'text', visible: true },
    { label: 'Product', property: 'productId', type: 'select', visible: true },
  ];
  dataSource = new TableVirtualScrollDataSource();
  selection = new SelectionModel<HistoicalConsumptionDto>(true, []);
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
    this.patch();
  }

  ngOnChanges() {
    // Refresh isAggregate
    this.onIsAggregateChnage(this.isAggregate);
    // Load Drp
    this.patch();
  }

  patch() {
    combineLatest([this.loadProducts(), this.loadUserRegions(), this.loadUserLaboratories()])
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(([products, userRegions, userLaboratories]) => {
        // Set Drp data
        this.products = products;
        this.userRegions = userRegions;
        this.userLaboratories = userLaboratories;

        // Patch the Table Data
        this.formArry = Array.from(Array(this.data.length), (_, i) => i + 1).map(x => {
          let form = this.appendObject();

          // Region
          if (!this.data[x - 1].regionId) {
            this.data[x - 1].regionId = userRegions.find(item => item.name.trim().toLowerCase() == this.data[x - 1].regionName.trim().toLowerCase())?.id;
          }
          // Laboratory
          if (!this.data[x - 1].laboratoryId) {
            this.data[x - 1].laboratoryId = userLaboratories.find(item => item.regionId == this.data[x - 1].regionId && item.name.trim().toLowerCase() == this.data[x - 1].laboratoryName.trim().toLowerCase())?.id;
          }
          // Product
          if (!this.data[x - 1].productId) {
            this.data[x - 1].productId = products.find(item => item.name.trim().toLowerCase() == this.data[x - 1].productName.trim().toLowerCase())?.id;
          }

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
    return this.httpService.GET(ProductsController.GetAllAsDrp).pipe(map(res => res.data));
  }
  loadUserRegions() {
    let params: QueryParamsDto[] = [
      { key: 'applicationUserId', value: this.loggedInUser.id },
      { key: 'countryId', value: this.countryId },
    ];
    return this.httpService.GET(UserSubscriptionsController.GetUserRegionsAsDrp, params).pipe(map(res => res.data));
  }
  loadUserLaboratories() {
    let params: QueryParamsDto[] = [
      { key: 'applicationUserId', value: this.loggedInUser.id },
      { key: 'countryId', value: this.countryId },
    ];
    return this.httpService.GET(UserSubscriptionsController.GetUserLaboratoriesAsDrp, params).pipe(map(res => res.data));
  }

  get visibleColumns() {
    if (!this.columns) {
      return [];
    }
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  readFile(event: any) {

    // Reset vars
    this.sheetNames = [];
    this.selectedSheetName = null;
    this.workbook = null;
    this.data = [];
    this.formArry = [];
    this.dataSource = new TableVirtualScrollDataSource();
    this.dataSource.paginator = this.paginator;

    let file = event.target.files[0];
    let fileReader = new FileReader();
    fileReader.onload = (e) => {
      let arrayBuffer: any = fileReader.result;
      var data = new Uint8Array(arrayBuffer);
      var arr = new Array();
      for (var i = 0; i != data.length; ++i) arr[i] = String.fromCharCode(data[i]);
      var bstr = arr.join("");
      // Set the workbook
      this.workbook = XLSX.read(bstr, { type: "binary", cellDates: true });
      // Get sheet names
      this.sheetNames = this.workbook.SheetNames.map(x => x.trim());
      // Auto Select the Correct Object
      if (this.sheetNames && this.sheetNames.length == 1 && this.sheetNameIsValid(this.sheetNames[0])) {
        this.onSheetChange(this.sheetNames[0]);
      }
    }

    fileReader.readAsArrayBuffer(file);
    this.isActive = true;
    this._ref.detectChanges();
  }

  onSheetChange(sheetName: string) {
    // Validate sheet name
    if (!this.sheetNameIsValid(sheetName)) {
      this.isValidSheetName = false;
      return;
    }

    this.isValidSheetName = true;
    this.selectedSheetName = sheetName;

    // If sheet name is valid
    let sheetData = this.workbook.Sheets[sheetName];
    this.data = XLSX.utils.sheet_to_json(sheetData, { raw: true, dateNF: 'dd/mm/yyyy' });
    this.patch();
  }

  sheetNameIsValid(sheetName: string): boolean {
    if (!sheetName) {
      return false;
    }
    let allSheetNames: string[] = ['HistoicalConsumption', 'HistoicalConsumptions', 'Histoical Consumption', 'Histoical Consumptions', 'Histoical-Consumption'];
    allSheetNames = allSheetNames.map(x => x.trim().toLowerCase());
    // Distinc names
    allSheetNames = allSheetNames.filter((n, i) => allSheetNames.indexOf(n) === i);
    // Check if the sheet name exist on registred names
    if (allSheetNames.indexOf(sheetName.trim().toLowerCase()) > -1) {
      return true;
    }
    return false;
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
      regionId: [null, Validators.compose([Validators.required])],
      laboratoryId: [null, Validators.compose([Validators.required, RxwebValidators.unique()])],
      forecastCategoryName: [null, Validators.compose([])],
      productId: [null, Validators.compose([Validators.required])],
    });
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

  get isValid(): boolean {
    if (this.formArry?.length == 0) return false;
    /** check form */
    const invalidFormGroups = this.formArry?.filter(x => x.invalid);
    if (invalidFormGroups?.length > 0) {
      invalidFormGroups.forEach(x => x.markAllAsTouched());
      return false;
    }

    return true;
  }
  setErrors() {
    const invalidFormGroups = this.formArry?.filter(x => x.invalid);
    this.errors = this.getFormGroupsErrors(invalidFormGroups, this.formArry);
  }

  onIsAggregateChnage(isAggregate: boolean) {
    if (!isAggregate) {
      this.columns = this.columns.filter(x => x.property != 'forecastCategoryName');
      this.dataSource.paginator = this.paginator;
    } else {
      this.columns = this.columns.filter(x => x.property != 'forecastCategoryName' && x.property != 'productId');
      this.columns.push({ label: 'Forecast Category', property: 'forecastCategoryName', type: 'text', visible: true });
      this.columns.push({ label: 'Product', property: 'productId', type: 'select', visible: true });
    }
  }

  getFilteredLabs(regionId: number): LaboratoryDto[] {
    return this.userLaboratories.filter(x => x.regionId == regionId);
  }


  getRowNumber(i: number): number {
    const rowNumber = (this.paginator.pageIndex * this.paginator.pageSize) + (i + 1);
    return this.formatInt(rowNumber, false)
  }
}
