import { Component, OnInit, EventEmitter, ViewChild } from "@angular/core";
import { Router, ActivatedRoute } from "@angular/router";
import { BsModalRef, BsModalService, ModalDirective } from "ngx-bootstrap";
import { NgForm } from "@angular/forms";

import * as XLSX from "xlsx";
import { APIwithActionService } from "../../shared/APIwithAction.service";
import { HttpRequest, HttpHeaders, HttpClient } from "@angular/common/http";
import { GlobalAPIService } from "app/shared/GlobalAPI.service";

@Component({
  selector: "app-forecast-import-service",
  templateUrl: "./ForecastImportService.component.html",
  styleUrls: ["ForecastImportService.component.css"],
})
export class ForecastImportServiceComponent implements OnInit {
  @ViewChild("lgModalError") public lgModalError: ModalDirective;
  public event: EventEmitter<any> = new EventEmitter();
  forecastId: number;
   Messagelable:string="";
  wb: XLSX.WorkBook;
  Sheetarr = new Array();
  file: File;
  importForm: NgForm;
  Filenamenew: string = "Include some File";
  headerlist: any = new Array();
  excelarray = new Array();
  importedlist = new Array();
  uploadimportedlist = new Array();
  loadedHeadlist = new Array();
  loadedDatalist = new Array();
  stringarr: string = "S";
  loading: boolean = false;
  methodology: string = "";
  formData = new FormData();
  title: string = "Import your patient data";
  SheetList = new Array();
  selectedSheet = "";
  masterData = new Array();
  regularData = new Array();
  combinationData = new Array();
  hasUploadedData = false;
  months: any[] = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
  errorMsg = new Array();

  constructor(
    private http: HttpClient,
    private _router: Router,
    private _APIwithActionService: APIwithActionService,
    public bsModalRef: BsModalRef,
    private _GlobalAPIService: GlobalAPIService
  ) {}

  ngOnInit() {
    this.loading = true;
    this._APIwithActionService.getDatabyID(this.forecastId, "Forecsatinfo", "GetbyId").subscribe((resp) => {
      this.stringarr = resp["forecastType"];
      this.methodology = resp["methodology"];
      switch (this.methodology) {
        case "CONSUMPTION":
          this._APIwithActionService.getDatabyID(this.forecastId, "Import", "getimporteddata").subscribe((resp) => {
            if (resp.length) this.hasUploadedData = true;
            this.loadedHeadlist = ["Region", "Site", "ProductName"];
            var helper = {};
            var result = resp.reduce((r, o) => {
              if (this.loadedHeadlist.indexOf(o.duration) == -1) this.loadedHeadlist.push(o.duration);
              var key = o.regionName + "-" + o.siteID + "-" + o.proID;
              if (!helper[key]) {
                helper[key] = key;
                r.push({
                  rowKey: key,
                  regionName: o.regionName,
                  siteID: o.siteID,
                  siteName: o.siteName,
                  proID: o.proID,
                  proName: o.proName,
                  row: new Array(Object.assign({}, o)),
                });
              } else {
                r.find((item) => item.rowKey == key).row.push(Object.assign({}, o));
              }
              return r;
            }, []);
            this.loadedDatalist = result;
            this.loading = false;
          });
          this.title = "IMPORT YOUR HISTORICAL CONSUMPTION DATA HERE";
          this.regularData.push({ column: "Region", isnullable: false }, { column: "Site", isnullable: false }, { column: "Product", isnullable: false });
          break;
        case "SERVICE STATSTICS":
          this._APIwithActionService.getDatabyID(this.forecastId, "Import", "getimportedservicedata").subscribe((resp) => {
            console.log("resp", resp);
            if (resp.length) this.hasUploadedData = true;
            this.loadedHeadlist = ["Region", "Site", "TestName"];
            var helper = {};
            var result = resp.reduce((r, o) => {
              if (this.loadedHeadlist.indexOf(o.duration) == -1) this.loadedHeadlist.push(o.duration);
              var key = o.regionName + "-" + o.siteID + "-" + o.testID;
              if (!helper[key]) {
                helper[key] = key;
                r.push({
                  rowKey: key,
                  regionName: o.regionName,
                  siteID: o.siteID,
                  siteName: o.siteName,
                  testID: o.testID,
                  testName: o.testName,
                  row: new Array(Object.assign({}, o)),
                });
              } else {
                r.find((item) => item.rowKey == key).row.push(Object.assign({}, o));
              }
              return r;
            }, []);
            this.loadedDatalist = result;
            this.loading = false;
          });
          this.title = "IMPORT YOUR HISTORICAL SERVICE DATA HERE";
          this.regularData.push({ column: "Region", isnullable: false }, { column: "Site", isnullable: false }, { column: "Test", isnullable: false });
          break;
        default:
          this.loading = false;
          break;
      }
    });
  }

  onChangeSheet(sheetName: string) {
    this.selectedSheet = sheetName;
    this.onUpdateTable(sheetName);
  }
  onInitMasterData() {
    this.masterData = [
      { sheet: "Region", column: "Region", data: new Array() },
      { sheet: "Site", column: "Site Name", data: new Array() },
      { sheet: "Instrument", column: "Instrument Name", data: new Array() },
      { sheet: "Product", column: "Product Name", data: new Array() },
    ];
  }

  onUpdateTable(sheetName: string) {
    //Update masterData
    this.onInitMasterData();
    this.combinationData = new Array();
    for (var idx = 0; idx < this.masterData.length; idx++) {
      if (this.SheetList.indexOf(this.masterData[idx].sheet) > -1) {
        let masterWorkSheet = this.wb.Sheets[this.masterData[idx].sheet];
        var masterSheetData = XLSX.utils.sheet_to_json(masterWorkSheet, { header: 1 });
        var masterSheetHeader: any = masterSheetData[0];
        var masterColIndex = masterSheetHeader.indexOf(this.masterData[idx].column);
        if (masterColIndex > -1) {
          for (let colIdx = 1; colIdx < masterSheetData.length; colIdx++) {
            if (this.masterData[idx].data.indexOf(masterSheetData[colIdx][masterColIndex]) == -1) {
              this.masterData[idx].data.push(masterSheetData[colIdx][masterColIndex].toLowerCase());
            }
            if (this.masterData[idx].sheet == "Site") {
              this.combinationData.push((masterSheetData[colIdx][0] + "-" + masterSheetData[colIdx][2]).toLowerCase());
            }
          }
        }
      }
    }
    // console.log("this.masterData", this.masterData);
    //Update Table Data
    let workSheet = this.wb.Sheets[sheetName];
    var data1: any = XLSX.utils.sheet_to_json(workSheet, { header: 1 });
    console.log("data1", data1);
    this.headerlist = data1[0];
    let arr = new Array();
    for (let index = 1; index < data1.length; index++) {
      if (data1[index].length) {
        arr.push(data1[index]);
      }
    }
    this.excelarray = [];
    //if (sheetName=="Historical Consumption" || sheetName=="Historical Consumption")
    for (let index = 0; index < data1.length; index++) {
      if (data1[index].length) {
        this.excelarray.push(data1[index]);
      }
    }
    this.importedlist = arr;
    console.log("this.importdlist", this.importedlist);
    //Check Doc format
    this.errorMsg = new Array();
    for (var idx = 0; idx < this.regularData.length; idx++) {
      var idxRegularData = this.headerlist.findIndex((item) => item.toLowerCase().indexOf(this.regularData[idx].column.toLowerCase()) > -1);
      if (idxRegularData == -1) {
        this.errorMsg.push(this.regularData[idx].column + " Column must be exist");
      } else {
        for (var col = 0; col < this.importedlist.length; col++) {
          if (!this.importedlist[col][idxRegularData]) {
            this.errorMsg.push(this.headerlist[idxRegularData] + " Column's " + col + "th cell should not be empty");
          }
        }
      }
    }
    if (this.errorMsg.length) return;
    for (var idx = 0; idx < this.headerlist.length; idx++) {
      var idxMasterData = this.masterData.findIndex((item) => this.headerlist[idx].toLowerCase().indexOf(item.sheet.toLowerCase()) > -1);
      if (idxMasterData > -1) {
        for (var colIdx = 0; colIdx < this.importedlist.length; colIdx++) {
          if (this.masterData[idxMasterData].data.indexOf(this.importedlist[colIdx][idx].toLowerCase()) == -1) {
            this.errorMsg.push(
              this.headerlist[idx] +
                " Column's " +
                (colIdx + 2) +
                "th Cell Value is not defined in " +
                this.masterData[idxMasterData].sheet +
                "-" +
                this.masterData[idxMasterData].column +
                " Master Column."
            );
          }
        }
      }
    }
    //Check duplicate validate
    var helper = {};
    var key = "";
    for (var idx = 0; idx < this.importedlist.length; idx++) {
      key = this.importedlist[idx][0] + "-" + this.importedlist[idx][1] + "-" + this.importedlist[idx][2];
      if (!helper[key]) {
        helper[key] = idx;
      } else {
        this.errorMsg.push(idx + 2 + "th row is duplicated with " + (helper[key] + 2) + " th one");
      }
      var combinationKey = (this.importedlist[idx][0] + "-" + this.importedlist[idx][1]).toLowerCase();
      if (this.combinationData.indexOf(combinationKey) == -1) {
        this.errorMsg.push("'" + this.importedlist[idx][1] + "' Site does not belongs to '" + this.importedlist[idx][0] + "' Region in Site Sheet");
      }
    }
    //Check Combination validate
  }

  onErrorDetail() {
    this.lgModalError.show();
  }

  incomingfile(event) {
    this.Messagelable="";
    this.hasUploadedData = false;
    this.headerlist = new Array();
    this.importedlist = new Array();
    if (this.methodology == "MORBIDITY") {
      this.file = event.target.files[0];
      this.Filenamenew = this.file.name;
      this.formData.append(this.file.name, this.file);
      var token = localStorage.getItem("jwt");
      const uploadReq = new HttpRequest("PUT", `https://forlab-174007.appspot.com/api/Import/Importpatient/` + this.forecastId, this.formData, {
        headers: new HttpHeaders({
          Authorization: "Bearer " + token,
          userid: localStorage.getItem("userid"),
          countryid: localStorage.getItem("countryid"),
        }),
        reportProgress: true,
      });

      this.http.request(uploadReq).subscribe((event) => {
        if (this.stringarr == "S") {
          this.bsModalRef.hide();
          this.event.emit({ type: "next", methodology: this.methodology });
        } else {
          this.bsModalRef.hide();
          this.event.emit({ type: "next", methodology: this.methodology });
        }
        // TODO: Move to another page
        // if (this.stringarr == "S") {
        //     this._router.navigate(["/Forecast/Demographicsitebysite", this.forecastId,]);
        // } else {
        //     this._router.navigate(["/Forecast/DemographicAggregrate", this.forecastId,]);
        // }
      });
    } else {
      this.file = event.target.files[0];
      this.Filenamenew = this.file.name;
      const target: DataTransfer = <DataTransfer>event.target;
      if (target.files.length !== 1) throw new Error("Cannot use multiple files");
      const reader: FileReader = new FileReader();

      reader.onload = (e: any) => {
        /* read workbook */
        const bstr: string = e.target.result;
        this.wb = XLSX.read(bstr, { type: "binary" });
        /* grab first sheet */
        this.Sheetarr = this.wb.SheetNames;
        const wsname: string = this.wb.SheetNames[0];
        let ws: XLSX.WorkSheet;
        this.SheetList = this.wb.SheetNames;
        switch (this.methodology) {
          case "CONSUMPTION":
            if (this.SheetList.indexOf("Historical Consumption") > -1) {
              this.selectedSheet = "Historical Consumption";
              this.onUpdateTable("Historical Consumption");
            }
            break;
          case "SERVICE STATSTICS":
            if (this.SheetList.indexOf("Historical Service Data") > -1) {
              this.selectedSheet = "Historical Service Data";
              this.onUpdateTable("Historical Service Data");
            }
            break;
        }

        /* save data */
        var data = XLSX.utils.sheet_to_json(ws, { header: 1 });
      };
      reader.readAsBinaryString(target.files[0]);
    }
  }

  onCloseModal() {
    this.bsModalRef.hide();
  }

  openNextModal() {
    let Reportobject: Object;
    let importedlist1 = new Array();
    this.loading = true;
    if (this.excelarray.length > 0) {
      for (let index = 6; index < this.excelarray[0].length; index++) {
        let date1 = new Date(this.excelarray[0][index]);
        this.excelarray[0][index] = date1.getDate().toString() + "/" + (date1.getMonth() + 1) + "/" + date1.getFullYear();
        //console.log(date1)
      }
      if (this.methodology == "SERVICE STATSTICS") {
        // this._APIwithActionService.putAPI(this.forecastId, this.excelarray, "Import", "importservice").subscribe((data: any) => {
        // importedlist1 = JSON.parse(data["_body"]);
        // Reportobject = {
        //   receivereportdata: this.uploadimportedlist,
        // };
        // if (Reportobject["receivereportdata"].length > 0) {
        //   this._APIwithActionService.postAPI(Reportobject, "Import", "saveimportservice").subscribe((data: any) => {
        //     this._GlobalAPIService.SuccessMessage(data["_body"]);
        //     this.loading=false;
        //     // this._router.navigate(["/Forecast/forecastFactor", this.forecastId,]);
        //     this.bsModalRef.hide();
        //     this.event.emit({ type: "next", methodology: this.methodology });
        //   });
        // }
        // else {
        //   this.bsModalRef.hide();
        //   this.event.emit({ type: "next", methodology: this.methodology });
        // }

        // this._APIwithActionService.putAPI(this.forecastId, this.excelarray, "Import", "importservicenew").subscribe((data: any) => {
        //  importedlist1 = JSON.parse(data["_body"]);
        // Reportobject = {
        //   receivereportdata: importedlist1,
        // };
        // if (Reportobject["receivereportdata"].length > 0) {
        //   this._APIwithActionService.postAPI(Reportobject["receivereportdata"], "Import", "saveimportconsumption").subscribe((data: any) => {
        // this._GlobalAPIService.SuccessMessage(data["_body"]);
        this.loading = false;
        this.bsModalRef.hide();
        this.event.emit({ type: "next", methodology: this.methodology });
        //   });
        // } else {
        //   this.bsModalRef.hide();
        //   this.event.emit({ type: "next", methodology: this.methodology });
        // }
        // });
      } else if (this.methodology == "CONSUMPTION") {
        // this._APIwithActionService.putAPI(this.forecastId, this.excelarray, "Import", "Importconsumptionnew").subscribe((data: any) => {
        //   importedlist1 = JSON.parse(data["_body"]);
        //   Reportobject = {
        //     receivereportdata: this.uploadimportedlist,
        //   };
        // if (Reportobject["receivereportdata"].length > 0) {
        // this._APIwithActionService.postAPI(Reportobject["receivereportdata"], "Import", "saveimportconsumption").subscribe((data: any) => {
        // this._GlobalAPIService.SuccessMessage(data["_body"]);
        this.loading = false;
        this.bsModalRef.hide();
        this.event.emit({ type: "next", methodology: this.methodology });
        // });
        // } else {
        //   this.bsModalRef.hide();
        //   this.event.emit({ type: "next", methodology: this.methodology });
        // }
      } else {
        if (this.stringarr == "S") {
          this.bsModalRef.hide();
          this.event.emit({ type: "next", methodology: this.methodology });
        } else {
          this.bsModalRef.hide();
          this.event.emit({ type: "next", methodology: this.methodology });
        }
      }
    } else {
      this.bsModalRef.hide();
      this.event.emit({ type: "next", methodology: this.methodology });
    }
  }

  openPreviousModal() {
    this.bsModalRef.hide();
    this.event.emit({ type: "back" });
  }

  onUpload() {
    // let arr = new Array();
    // arr.push(this.headerlist);
    // arr.push(this.importedlist)
    //     console.log( this.importedlist)
    // this.headerlist
    if (this.excelarray.length > 0) {
      for (let index = 3; index < this.excelarray[0].length; index++) {
        let date1 = new Date(this.excelarray[0][index]);
        this.excelarray[0][index] = date1.getDate().toString() + "/" + (date1.getMonth() + 1) + "/" + date1.getFullYear();
        //console.log(date1)
      }
      console.log("this.excelarray", this.excelarray);
      switch (this.methodology) {
        case "CONSUMPTION":
          this.loading = true;
          this._APIwithActionService.putAPI(this.forecastId, this.excelarray, "Import", "Importconsumptionnew").subscribe((data: any) => {
            console.log("importconsumption-response", data);
            this.Messagelable= data["_body"];
            // this.uploadimportedlist = JSON.parse(data["_body"]);
            this.loading = false;
            if (this.importedlist.length == 0) {
              this._GlobalAPIService.FailureMessage("Something went wrong");
            }
          });
          break;
        case "SERVICE STATSTICS":
          this.loading = true;
          this._APIwithActionService.putAPI(this.forecastId, this.excelarray, "Import", "importservicenew").subscribe((data: any) => {
            console.log("importservice-response", data);
            this.Messagelable= data["_body"];
            // this.uploadimportedlist = JSON.parse(data["_body"]);
            this.loading = false;
            if (this.importedlist.length == 0) {
              this._GlobalAPIService.FailureMessage("Something went wrong");
            }
          });
          break;
      }
    }
  }
}
