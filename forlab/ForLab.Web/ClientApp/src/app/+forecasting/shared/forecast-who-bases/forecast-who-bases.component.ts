import { Component, OnInit, Injector, ViewEncapsulation, ViewChild, Input } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { TableColumn } from 'src/@core/models/common/response';
import * as XLSX from 'xlsx';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { SelectionModel } from '@angular/cdk/collections';
import { FormGroup, FormBuilder, FormArray, Validators } from '@angular/forms';
import { takeUntil } from 'rxjs/operators';
import { HistoicalWhoBaseDto } from 'src/@core/models/forecasting/ImportedFileTemplate';
import { DiseaseDto } from 'src/@core/models/disease/Disease';
import { CountryDto } from 'src/@core/models/lookup/Country';
import { CountriesController } from 'src/@core/APIs/CountriesController';
import { DiseasesController } from 'src/@core/APIs/DiseasesController';
import { FormValidationError } from 'src/@core/models/common/SharedModel';

@Component({
  selector: 'forecast-who-bases',
  templateUrl: './forecast-who-bases.component.html',
  styleUrls: ['./forecast-who-bases.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class ForecastWhoBasesComponent extends BaseService implements OnInit {

  // Excel File
  sheetNames: string[] = [];
  selectedSheetName: string;
  workbook: XLSX.WorkBook;
  @Input('data') sheetDataJson: any[] = [];
  // Flags
  isValidSheetName: boolean = true;
  isActive: boolean = true;
  // Table
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort;
  // @ViewChild('searchInput', { static: false }) searchInput: ElementRef;
  columns: TableColumn<HistoicalWhoBaseDto>[] = [
    { label: 'Actions', property: 'actions', type: 'button', visible: true },
    { label: '#', property: 'index', type: 'custom', visible: true },
    { label: 'Country', property: 'countryId', type: 'select', visible: true },
    { label: 'Disease', property: 'diseaseId', type: 'select', visible: true },
  ];
  dataSource: MatTableDataSource<any> | null = new MatTableDataSource([]);
  selection = new SelectionModel<any>(true, []);
  // Form
  searchForm: FormGroup;
  formItem = this.fb.group({});
  // vars
  errors: FormValidationError[] = [];
  arrayBuffer: any;
  file: File;
  // Drp
  countries: CountryDto[] = [];
  diseases: DiseaseDto[] = [];

  constructor(private fb: FormBuilder, public injector: Injector) {
    super(injector);
    this.searchForm = this.createFormItem();
  }

  ngOnInit() {
    // Load Drp
    this.loadCountries();
    this.loadDiseases();

    if (this.columns.length > 0) {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    }
    setTimeout(() => this.dataSource.paginator = this.paginator);
  }

  loadCountries() {
    this.httpService.GET(CountriesController.GetAllAsDrp)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.countries = res.data;
            // Refresh data on the table
            this.getObjects().clear();
            this.dataSource.paginator = this.paginator;
            this.patch(this.sheetDataJson);
            this._ref.detectChanges();
          } else {
            this.alertService.error(res.message);
            this.loading = false;
            this._ref.detectChanges();
          }
        }, err => {
          this.alertService.exception();
          this.loading = false;
          this._ref.detectChanges();
        });
  }
  loadDiseases() {
    this.httpService.GET(DiseasesController.GetAllAsDrp)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.diseases = res.data;
            // Refresh data on the table
            this.getObjects().clear();
            this.dataSource.paginator = this.paginator;
            this.patch(this.sheetDataJson);
            this._ref.detectChanges();
          } else {
            this.alertService.error(res.message);
            this.loading = false;
            this._ref.detectChanges();
          }
        }, err => {
          this.alertService.exception();
          this.loading = false;
          this._ref.detectChanges();
        });
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
    this.sheetDataJson = [];
    this.getObjects().clear();
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
    this.sheetDataJson = XLSX.utils.sheet_to_json(sheetData, { raw: true, dateNF: 'dd/mm/yyyy' });
    this.patch(this.sheetDataJson);
  }

  sheetNameIsValid(sheetName: string): boolean {
    if (!sheetName) {
      return false;
    }
    let allSheetNames: string[] = ['ForecastMorbidityWhoBase', 'ForecastMorbidityWhoBases', 'Forecast Morbidity Who Base', 'Forecast-Morbidity-Who-Base'];
    allSheetNames = allSheetNames.map(x => x.trim().toLowerCase());
    // Distinc names
    allSheetNames = allSheetNames.filter((n, i) => allSheetNames.indexOf(n) === i);
    // Check if the sheet name exist on registred names
    if (allSheetNames.indexOf(sheetName.trim().toLowerCase()) > -1) {
      return true;
    }
    return false;
  }

  formatList(list: string[]): string {
    return list.join(', ');
  }


  // Table 
  onFilterChange(filterValue: string) {
    filterValue = filterValue.trim();
    filterValue = filterValue.toLowerCase();
    this.dataSource.filter = filterValue;
  }

  toggleColumnVisibility(column, event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    column.visible = !column.visible;
  }

  trackByProperty<T>(index: number, column: TableColumn<T>) {
    return column.property;
  }

  patch(data: HistoicalWhoBaseDto[]) {
    for (let index = 0; index < data.length; index++) {
      this.addObject();
    }
    this.searchForm.get('objects').patchValue(data);

    // fill dropdown
    if (data.length > 0) {
      data.forEach((x, i) => {
        // Country
        if (this.countries && this.countries.length > 0 && data[i].countryName) {
          const country = this.countries.find(x => x.name.trim().toLowerCase() == data[i].countryName.trim().toLowerCase());
          data[i].countryId = country ? country.id : null;
        }
        // Disease
        if (this.diseases && this.diseases.length > 0 && data[i].diseaseName) {
          const disease = this.diseases.find(x => x.name.trim().toLowerCase() == data[i].diseaseName.trim().toLowerCase());
          data[i].diseaseId = disease ? disease.id : null;
        }
      });

      this.searchForm.get('objects').patchValue(data);
      this._ref.detectChanges();
    }
  }

  createFormItem(): FormGroup {
    return this.formItem = this.fb.group({
      objects: this.fb.array([])
    });
  }

  appendObject() {
    return this.formItem = this.fb.group({
      countryId: [null, Validators.compose([Validators.required])],
      diseaseId: [null, Validators.compose([Validators.required])],
    })
  }

  getObjects(): FormArray {
    return this.searchForm.get('objects') as FormArray;
  }

  addObject() {
    this.getObjects().insert(0, this.appendObject());
    this.dataSource = new MatTableDataSource((this.searchForm.get('objects') as FormArray).controls);
    this.dataSource.paginator = this.paginator;
  }

  deleteObject(objectIndex: number) {
    this.getObjects().removeAt(objectIndex);
    this.dataSource = new MatTableDataSource((this.searchForm.get('objects') as FormArray).controls);
    this.dataSource.paginator = this.paginator;
  }

  get isValid(): boolean {
    if (this.getObjects().controls.length == 0) return false;
    /** check form */
    if (this.searchForm.invalid) {
      this.searchForm.markAllAsTouched();
      return false;
    }
    return true;
  }
  setErrors() {
    this.errors = this.getFormValidationErrors(this.getObjects());
  }

  getRowNumber(i: number): number {
    const rowNumber = (this.paginator.pageIndex * this.paginator.pageSize) + (i + 1);
    return this.formatInt(rowNumber, false)
  }

}
