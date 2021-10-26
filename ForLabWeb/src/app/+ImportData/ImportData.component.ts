import { Component, ViewChild, TemplateRef, Renderer, AfterViewInit, OnInit } from "@angular/core";
import * as XLSX from "xlsx";
import { APIwithActionService } from "../shared/APIwithAction.service";
import { GlobalAPIService } from "../shared/GlobalAPI.service";
import { HttpClient, HttpRequest, HttpEventType, HttpResponse, HttpHeaders, HttpParams } from "@angular/common/http";
import { FormGroup, Validators, FormBuilder } from "@angular/forms";

import { ngxLoadingAnimationTypes, NgxLoadingComponent } from "ngx-loading";
import { ModalDirective } from "ngx-bootstrap";
import { element } from "protractor";
import * as FileSaver from "file-saver";
import { columnsByPin } from "@swimlane/ngx-datatable/release/utils";
import { Router } from "@angular/router";
const PrimaryWhite = "#ffffff";
const SecondaryGrey = "#ccc";
const PrimaryRed = "#dd0031";
const SecondaryBlue = "#006ddd";
const fileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8";
const fileExtension = ".xlsx";
@Component({
  selector: "sa-importdata",
  templateUrl: "./ImportData.component.html",
  styleUrls: ["ImportData.component.css"],
})
export class ImportDataComponent implements OnInit {
  arrayBuffer: any;
  file: File;
  ImportedFileName: string;
  Importform: FormGroup;
  btnUpload: boolean = true;
  Shoediv: boolean = false;
  public progress: number;
  public message: string = "";
  public messagearr = new Array();
  public Show: boolean = false;
  regularSheets = new Array();
  masterData = {};
  documentArr = new Array();
  selectedsheet = new Array();
  wb: XLSX.WorkBook;
  @ViewChild("lgModal4") public lgModal4: ModalDirective;
  @ViewChild("lgModalError") public lgModalError: ModalDirective;
  @ViewChild("ngxLoading") ngxLoadingComponent: NgxLoadingComponent;
  @ViewChild("customLoadingTemplate") customLoadingTemplate: TemplateRef<any>;
  public ngxLoadingAnimationTypes = ngxLoadingAnimationTypes;
  isAllChecked: boolean;
  formData = new FormData();
  public loading = false;

  openedSheetIndex = 0;
  openedDocSheetIndex = 0;
  constructor(
    private router: Router,
    private _fb: FormBuilder,
    private http: HttpClient,
    private _APIwithActionService: APIwithActionService,
    private _GlobalAPIService: GlobalAPIService
  ) {
    this.Importform = this._fb.group({
      file: [null, Validators.required],
    });
  }
  ngOnInit() {}
  onInitRegularData() {
    this.openedSheetIndex = 0;
    this.openedDocSheetIndex = 0;
    this.regularSheets = [
      {
        sheet: "Region",
        sheetT: "",
        columns: [
          { type: "string", nullable: false, value: "Region", valueT: "", foreign: "" },
          { type: "string", nullable: true, value: "Short Name", valueT: "", foreign: "" },
        ],
        success: false,
        error: [],
      },
      {
        sheet: "Site",
        sheetT: "",
        columns: [
          { type: "string", nullable: false, value: "Region", valueT: "", foreign: "Region-Region" },
          { type: "string", nullable: false, value: "Site Category", valueT: "", foreign: "" },
          { type: "string", nullable: false, value: "Site Name", valueT: "", foreign: "" },
          { type: "number", nullable: false, value: "Working Days", valueT: "", foreign: "" },
        ],
        success: false,
        error: [],
      },
      {
        sheet: "Site Instrument",
        sheetT: "",
        columns: [
          { type: "string", nullable: false, value: "Region", valueT: "", foreign: "Region-Region" },
          { type: "string", nullable: false, value: "Site Name", valueT: "", foreign: "Site-Site Name" },
          { type: "string", nullable: false, value: "Testing Area", valueT: "", foreign: "" },
          { type: "string", nullable: false, value: "Instrument Name", valueT: "", foreign: "Instrument-Instrument Name" },
          { type: "number", nullable: false, value: "Quantity", valueT: "", foreign: "" },
          { type: "number", nullable: false, value: "%Run", valueT: "", foreign: "" },
        ],
        success: false,
        error: [],
      },
      {
        sheet: "Product",
        sheetT: "",
        columns: [
          { type: "string", nullable: false, value: "Product Name", valueT: "", foreign: "" },
          { type: "string", nullable: false, value: "Product Type", valueT: "", foreign: "" },
          { type: "string", nullable: false, value: "Basic Unit", valueT: "", foreign: "" },
          { type: "number", nullable: true, value: "Min Packs Per Site", valueT: "", foreign: "" },
          { type: "number", nullable: false, value: "Price", valueT: "", foreign: "" },
          { type: "number", nullable: false, value: "Packaging Size", valueT: "", foreign: "" },
          { type: "string", nullable: false, value: "Price As of Date", valueT: "", foreign: "" },
        ],
        success: false,
        error: [],
      },
      {
        sheet: "Instrument",
        sheetT: "",
        columns: [
          { type: "string", nullable: false, value: "Testing Area", valueT: "", foreign: "" },
          { type: "string", nullable: false, value: "Instrument Name", valueT: "", foreign: "" },
          { type: "number", nullable: false, value: "Max Through Put", valueT: "", foreign: "" },
          { type: "number", nullable: false, value: "Per Test Control", valueT: "", foreign: "" },
          { type: "number", nullable: true, value: "Daily Control Test", valueT: "", foreign: "" },
          { type: "number", nullable: true, value: "Weekly Control Test", valueT: "", foreign: "" },
          { type: "number", nullable: true, value: "Monthly Control Test", valueT: "", foreign: "" },
          { type: "number", nullable: true, value: "Quarterly control Test", valueT: "", foreign: "" },
        ],
        success: false,
        error: [],
      },
      {
        sheet: "Test",
        sheetT: "",
        columns: [
          { type: "string", nullable: false, value: "Test Name", valueT: "", foreign: "" },
          { type: "string", nullable: false, value: "Area Name", valueT: "", foreign: "" },
        ],
        success: false,
        error: [],
      },
      {
        sheet: "Test Product Usage Rate",
        sheetT: "",
        columns: [
          { type: "string", nullable: false, value: "Test Name", valueT: "", foreign: "" },
          { type: "string", nullable: false, value: "Instrument", valueT: "", foreign: "Instrument-Instrument Name" },
          { type: "string", nullable: false, value: "Product", valueT: "", foreign: "Product-Product Name" },
          { type: "number", nullable: false, value: "Rate", valueT: "", foreign: "" },
          { type: "number", nullable: true, value: "Is For Control", valueT: "", foreign: "" },
        ],
        success: false,
        error: [],
      },
      {
        sheet: "Consumables",
        sheetT: "",
        columns: [
          { type: "string", nullable: false, value: "Test Name", valueT: "", foreign: "" },
          { type: "string", nullable: false, value: "Instrument Name", valueT: "", foreign: "Instrument-Instrument Name" },
          { type: "string", nullable: false, value: "Product Name", valueT: "", foreign: "Product-Product Name" },
          { type: "string", nullable: true, value: "Period", valueT: "", foreign: "" },
          { type: "string", nullable: true, value: "Number Of Test", valueT: "", foreign: "" },
          { type: "number", nullable: false, value: "Rate", valueT: "", foreign: "" },
          { type: "string", nullable: true, value: "Per Test", valueT: "", foreign: "" },
          { type: "string", nullable: true, value: "Per Period", valueT: "", foreign: "" },
          { type: "string", nullable: true, value: "Per Instrument", valueT: "", foreign: "" },
        ],
        success: false,
        error: [],
      },
    ];
    this.masterData = {
      "Region-Region": new Array(),
      "Site-Site Name": new Array(),
      "Instrument-Instrument Name": new Array(),
      "Product-Product Name": new Array(),
    };
  }

  onUpdateMasterData(index: number, idxCol: number, docIndex: number, docColIndex: number) {
    var tempSheet = this.regularSheets[index];
    var sheetColName = tempSheet.sheet + "-" + tempSheet.columns[idxCol].value;
    if (this.masterData.hasOwnProperty(sheetColName)) {
      this.masterData[sheetColName] = new Array();
      if (docColIndex > -1) {
        var col = this.documentArr[docIndex].columns[docColIndex].col;
        var maxNum = this.documentArr[docIndex].max;
        var workSheet: any = this.wb.Sheets[tempSheet.sheetT];
        for (var c = 2; c < +maxNum + 1; c++) {
          if (workSheet[col + c]) {
            if (this.masterData[sheetColName].indexOf(workSheet[col + c].v) == -1) this.masterData[sheetColName].push(workSheet[col + c].v);
          }
        }
      }
      //Check realted Sheets in Document
      for (var idxSheet = 0; idxSheet < this.regularSheets.length; idxSheet++) {
        for (var idxColumn = 0; idxColumn < this.regularSheets[idxSheet].columns.length; idxColumn++) {
          if (this.regularSheets[idxSheet].columns[idxColumn].foreign === sheetColName) {
            this.checkExcelFormat(idxSheet);
          }
        }
      }
    }
  }

  checkExcelFormat(index: number) {
    var tempSheet = this.regularSheets[index];
    tempSheet.success = false;
    tempSheet.error = new Array();
    if (!tempSheet.sheetT) {
      tempSheet.error.push("Sheet Name is not selected");
      return;
    }
    for (var idxCol = 0; idxCol < tempSheet.columns.length; idxCol++) {
      if (!tempSheet.columns[idxCol].valueT) {
        tempSheet.error.push("'" + tempSheet.columns[idxCol].value + "' Column is not selected. Tip: Make sure it using 'Select Columns' button");
      } else {
        // 1. Check nullable
        var col = this.documentArr.find((item) => item.sheet === tempSheet.sheetT).columns.find((column) => column.value === tempSheet.columns[idxCol].valueT).col;
        var maxNum = this.documentArr.find((item) => item.sheet === tempSheet.sheetT).max;
        var workSheet: any = this.wb.Sheets[tempSheet.sheetT];
        for (var c = 2; c < +maxNum + 1; c++) {
          if (!tempSheet.columns[idxCol].nullable && !workSheet[col + c]) {
            tempSheet.error.push(col + c + " cell should not be blank");
          } else {
            //Check data type
            if (workSheet[col + c] && tempSheet.columns[idxCol].type !== typeof workSheet[col + c].v) {
              tempSheet.error.push(col + c + " cell's format should be " + tempSheet.columns[idxCol].type + "");
            }
            //Check Foreign columns
            if (workSheet[col + c] && tempSheet.columns[idxCol].foreign) {
              if (this.masterData[tempSheet.columns[idxCol].foreign].indexOf(workSheet[col + c].v) == -1) {
                tempSheet.error.push(col + c + " cell's value should be defined in " + tempSheet.columns[idxCol].foreign + " column");
              }
            }
            //Check %Run in Site Instrument
            if (tempSheet.sheet === "Site Instrument") {
            }
          }
        }
      }
    }
    if (!tempSheet.error.length) tempSheet.success = true;
  }

  onChangeDocSheet(index: number, docIndex: number) {
    if (docIndex > -1) {
      this.regularSheets[index].sheetT = this.documentArr[docIndex].sheet;
      // Need to recharge Columns based on new sheet selection
      var tempSheet = this.regularSheets[index];
      for (var idxCol = 0; idxCol < tempSheet.columns.length; idxCol++) {
        tempSheet.columns[idxCol].valueT = ""; //Reset the columns once sheet is changed
        this.onUpdateMasterData(index, idxCol, docIndex, -1);
        var docColIndex = this.documentArr[docIndex].columns.findIndex((element) => tempSheet.columns[idxCol].value === element.value);
        if (docColIndex > -1) {
          //Update regularSheets ValueT
          tempSheet.columns[idxCol].valueT = tempSheet.columns[idxCol].value;
          this.onUpdateMasterData(index, idxCol, docIndex, docColIndex);
        }
      }
      this.checkExcelFormat(index);
    } else {
      this.regularSheets[index].sheetT = "";
      this.checkExcelFormat(index);
    }
  }

  onChangeDocColumn(colIndex: number, docColIndex: number) {
    if (docColIndex > -1) {
      this.regularSheets[this.openedSheetIndex].columns[colIndex].valueT = this.documentArr[this.openedDocSheetIndex].columns[docColIndex].value;
    } else {
      this.regularSheets[this.openedSheetIndex].columns[docColIndex].valueT = "";
    }
    //Update masterData
    this.onUpdateMasterData(this.openedSheetIndex, colIndex, this.openedDocSheetIndex, docColIndex);
  }
  onApplyColumns() {
    this.lgModal4.hide();
    this.checkExcelFormat(this.openedSheetIndex);
  }

  openColumns(index: number) {
    this.openedSheetIndex = index;
    if (this.regularSheets[index].sheetT) {
      this.openedDocSheetIndex = this.documentArr.findIndex((item) => item.sheet === this.regularSheets[index].sheetT);
      this.lgModal4.show();
    } else {
      this._GlobalAPIService.FailureMessage("You must select Document Sheet firstly");
    }
  }
  onErrorReport(index: number) {
    this.openedSheetIndex = index;
    if (this.regularSheets[index].sheetT) {
      this.openedDocSheetIndex = this.documentArr.findIndex((item) => item.sheet === this.regularSheets[index].sheetT);
      this.lgModalError.show();
    } else {
      this._GlobalAPIService.FailureMessage("You must select Document Sheet firstly");
    }
  }

  Downloadsample() {
    window.location.href = "https://storage.cloud.google.com/forlabaj.appspot.com/ImportBlankTemplate.xls";
  }

  incomingfile(event) {
    this.onInitRegularData();
    const reader = new FileReader();
    this.file = event.target.files[0];
    this.ImportedFileName = this.file.name;
    this.formData.append(this.file.name, this.file);

    const target: DataTransfer = <DataTransfer>event.target;
    if (target.files.length !== 1) throw new Error("Cannot use multiple files");

    reader.onload = (e: any) => {
      this.documentArr = new Array();
      /* read workbook */
      const bstr: string = e.target.result;
      this.wb = XLSX.read(bstr, { type: "binary" });
      /* Make documentArr */
      for (var idx = 0; idx < this.wb.SheetNames.length; idx++) {
        var workSheet: any = this.wb.Sheets[this.wb.SheetNames[idx]];
        var columns = new Array();
        var maxNum = 0;
        for (let z in workSheet) {
          if (z[0] === "!") continue;
          //parse out the column, row, and value
          var tt = 0;
          for (var i = 0; i < z.length; i++) {
            if (+z[i]) {
              tt = i;
              break;
            }
          }
          var col = z.substring(0, tt);
          var row = +z.substring(tt);
          if (maxNum < row) maxNum = row;
          var value = workSheet[z].v;
          //store header names
          if (row == 1 && value) {
            columns.push({ col: col, value: value });
            continue;
          }
        }
        this.documentArr.push({ sheet: this.wb.SheetNames[idx], columns: columns, max: maxNum });
      }
      // console.log("this.file", this.file);
      console.log("this.wb", this.wb);
      // console.log("this.sheetarr", this.documentArr);
      // Find the same name sheet & columns on Document based on Regular format
      for (var idx = 0; idx < this.regularSheets.length; idx++) {
        var tempSheet = this.regularSheets[idx];
        var docIndex = this.documentArr.findIndex((element) => tempSheet.sheet === element.sheet);
        if (docIndex > -1) {
          tempSheet.sheetT = this.documentArr[docIndex].sheet;
          for (var idxCol = 0; idxCol < tempSheet.columns.length; idxCol++) {
            var docColIndex = this.documentArr[docIndex].columns.findIndex((element) => tempSheet.columns[idxCol].value === element.value);
            if (docColIndex > -1) {
              tempSheet.columns[idxCol].valueT = tempSheet.columns[idxCol].value;
              //Update masterData
              this.onUpdateMasterData(idx, idxCol, docIndex, docColIndex);
            }
          }
        }
      }
      //TODO: Update masterData

      // Check sheet by sheet
      for (var idx = 0; idx < this.regularSheets.length; idx++) {
        this.checkExcelFormat(idx);
      }
    };
    reader.readAsBinaryString(target.files[0]);
    // this.documentArr =["Region","Site","Product","Test","Instrument","Test Product Usage Rate","Consumables","Site Instrument"]
  }

  Upload() {
    if (!this.selectedsheet.length) {
      this._GlobalAPIService.FailureMessage("There is no data to upload. Please check sheets what you want to upload");
      return;
    }
    let selectedsheetstr: string = "";
    let selectedsheetArr = new Array();
    for (var idx = 0; idx < this.selectedsheet.length; idx++) {
      if (!this.selectedsheet[idx].success) {
        this._GlobalAPIService.FailureMessage("There are some wrong sheets, Please solve that issues. To see details Click Show button");
        return;
      }
      selectedsheetstr = selectedsheetstr + this.selectedsheet[idx].sheet + ",";
      let tempCols = new Array();
      for (var col = 0; col < this.selectedsheet[idx].columns.length; col++) {
        tempCols.push({
          regCol: this.selectedsheet[idx].columns[col].value,
          docCol: this.selectedsheet[idx].columns[col].valueT,
        });
      }
      selectedsheetArr.push({
        regSheet: this.selectedsheet[idx].sheet,
        docSheet: this.selectedsheet[idx].sheetT,
        columns: tempCols,
      });
    }
    // console.log("matchRule:", selectedsheetArr);
    this.formData.append("matchRule", JSON.stringify(selectedsheetArr));
    var token = localStorage.getItem("jwt");

    let params1 = new HttpParams();
    params1.append("sheets", selectedsheetstr);
    //"http://localhost:53234/api/Import/Uploadfile"
    //http://forlab.dataman.net.in/webapi/api/Import/Uploadfile   http://forlab.aspwork.co.in
    //http://forlabplus.com/webapi/api/Import/Uploadfile/
    //https://forlab-174007.appspot.com/api/Import/Uploadfile/
    const uploadReq = new HttpRequest('Post', `http://localhost:53234/api/Import/Uploadfile/` + selectedsheetstr, this.formData, {
      headers: new HttpHeaders({ "Authorization": "Bearer " + token, 'userid': localStorage.getItem("userid"), 'countryid': localStorage.getItem("countryid") }),
      reportProgress: true


    });
    this.loading = true;
    this.http.request(uploadReq).subscribe((event) => {
      if (event.type === HttpEventType.UploadProgress) this.progress = 50;
      else if (event.type === HttpEventType.Response) {
        this.progress = 100;
        this.message = event.body["msg"];
        if (this.message != "") {
          this.loading = false;
          this.btnUpload = false;
          this._GlobalAPIService.SuccessMessage("Uploaded Successfully");
          this.messagearr = this.message.split("#");
          this.Show = true;
        }
      }
    });
    if (this.messagearr.length > 0) {
    }
  }
  SelectSheet(ischecked: boolean, Item: any) {
    if (ischecked == true) {
      this.selectedsheet.push(Item);
    } else {
      this.selectedsheet.splice(
        this.selectedsheet.findIndex((x) => x.sheet === Item.sheet),
        1
      );
    }
  }
  Selectall(ischecked: boolean) {
    if (ischecked == true) {
      this.isAllChecked = true;
      this.selectedsheet = [...this.regularSheets];
    } else {
      this.isAllChecked = false;
      this.selectedsheet = [];
    }
  }
  onManageDataList() {
    this.router.navigate(["Managedata"]);
  }
}
