import { Component, OnInit, Injector, ViewEncapsulation } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { RegistredObjects } from './registred-objects';
import * as XLSX from 'xlsx';
import { MaxNumOfExcelRecords } from 'src/@core/config';

@Component({
  selector: 'app-import-root',
  templateUrl: './import-root.component.html',
  styleUrls: ['./import-root.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class ImportRootComponent extends BaseService implements OnInit {
  // registred objects
  registredObjects = RegistredObjects;
  // Excel File
  sheetNames: string[] = [];
  selectedSheetName: string;
  workbook: XLSX.WorkBook;
  sheetDataJson: any[] = [];
  selectedObjectName: string;
  // Flags
  maxNumOfExcelRecords: number = MaxNumOfExcelRecords;
  isNumOfRecordsValid: boolean = true;
  isSheetNameValid: boolean = true;
  isActive: boolean = false;
  // Query Params
  isSpecific: boolean = false;
  objectName: string = 'SampleImportData';

  constructor(public injector: Injector) {
    super(injector);
  }

  ngOnInit() {

    // Import from a specific object page
    this.activatedRoute.queryParams.subscribe(params => {
      if (this.registredObjects.findIndex(x => x.objectName == params['objectName']) > -1) {
        this.objectName = params['objectName'];
        this.isSpecific = true;
      } else {
        this.objectName = 'SampleImportData';
      }
    });

  }


  readFile(event: any) {

    // Reset vars
    this.sheetNames = [];
    this.selectedSheetName = null;
    this.workbook = null;
    this.sheetDataJson = [];
    this.selectedObjectName = null;

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
      this.isSheetNameValid = false;
      this.selectedObjectName = null;
      return;
    }

    this.isSheetNameValid = true;
    this.selectedSheetName = sheetName;

    // If sheet name is valid
    this.selectedObjectName = this.getRegistredObjectName(sheetName);
    if (!this.selectedObjectName) return;
    let sheetData = this.workbook.Sheets[sheetName];
    this.sheetDataJson = XLSX.utils.sheet_to_json(sheetData, { raw: true, dateNF: 'dd/mm/yyyy', defval: "" });
    // Check length validation
    if (this.sheetDataJson.length > this.maxNumOfExcelRecords) {
      this.isNumOfRecordsValid = false;
    } else {
      this.isNumOfRecordsValid = true;
    }
  }

  sheetNameIsValid(sheetName: string): boolean {
    if (!sheetName) {
      return false;
    }
    let allSheetNames: string[] = this.registredObjects.map(x => x.objectName);
    this.registredObjects.forEach(x => allSheetNames = allSheetNames.concat(x.acceptableSheetNames));
    // Remove whitespaces and convert to lowercase
    allSheetNames = allSheetNames.map(x => x.trim().toLowerCase());
    // Distinc names
    allSheetNames = allSheetNames.filter((n, i) => allSheetNames.indexOf(n) === i);
    // Check if the sheet name exist on registred names
    if (allSheetNames.indexOf(sheetName.trim().toLowerCase()) > -1) {
      return true;
    }
    return false;
  }

  getRegistredObjectName(sheetName: string): string {
    if (!sheetName) {
      return null;
    }

    sheetName = sheetName.trim().toLowerCase();
    let target = this.registredObjects.find(x =>
      x.objectName.trim().toLowerCase() == sheetName
      ||
      x.acceptableSheetNames.map(y => y.trim().toLowerCase()).includes(sheetName));

    if (!target) {
      return null;
    }

    return target.objectName;
  }

  formatList(list: string[]): string {
    return list.join(', ');
  }
}
