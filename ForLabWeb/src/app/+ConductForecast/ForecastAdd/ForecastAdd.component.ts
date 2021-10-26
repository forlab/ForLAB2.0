import { Component, OnInit, EventEmitter } from "@angular/core";
import { BsModalRef, BsModalService } from "ngx-bootstrap";
import { Router, ActivatedRoute } from "@angular/router";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";

import { APIwithActionService } from "../../shared/APIwithAction.service";
import { GlobalAPIService } from "../../shared/GlobalAPI.service";
import { formatDate } from "ngx-bootstrap/chronos";

@Component({
  selector: "app-forecast-add",
  templateUrl: "./ForecastAdd.component.html",
  styleUrls: ["ForecastAdd.component.css"],
})
export class ForecastAddComponent implements OnInit {
  public event: EventEmitter<any> = new EventEmitter();
  myForecastType: any;
  forecastTypeList: any[] = ["NATIONAL", "GLOBAL", "PROGRAM"];
  duration: string = "0 Months";

  defineforecast: FormGroup;
  tempData: any;
  Foracstinfoobj: Object;
  date: Date;
  forecastId: number;
  Id: number = 0;
  constructor(
    private _fb: FormBuilder,
    private _APIwithActionService: APIwithActionService,
    public bsModalRef: BsModalRef,
    private _GlobalAPIService: GlobalAPIService
  ) {
    this.myForecastType = 0;
  }

  ngOnInit() {
    if (this.forecastId > 0) {
      this.myForecastType = -1;

      this._APIwithActionService.getDatabyID(this.forecastId, "Forecsatinfo", "GetbyId").subscribe((resp) => {
        console.log("resp", resp);
        this.defineforecast.patchValue({
          ForecastID: resp["forecastNo"],
          forecastdate: new Array(new Date(resp["startDate"]), new Date(resp["forecastDate"])),
          Period: resp["period"],
          scopeofforecast: resp["scopeOfTheForecast"],
        });
        this.tempData = resp;

        if (this.forecastTypeList.indexOf(resp["scopeOfTheForecast"]) > -1) {
          this.myForecastType = this.forecastTypeList.indexOf(resp["scopeOfTheForecast"]);
        }
        this.onValueChange(this.defineforecast.get("forecastdate").value);
      });
    }

    this.defineforecast = this._fb.group({
      ForecastID: ["", Validators.compose([Validators.required, Validators.maxLength(32)])],
      forecastdate: [null, [Validators.required]],
      Period: "Monthly",
      scopeofforecast: "NATIONAL",
    });
  }

  handleForecastType(index: any) {
    this.myForecastType = index;
    this.defineforecast.patchValue({
      scopeofforecast: this.forecastTypeList[index],
    });
  }

  monthDiff(d1: Date, d2: Date) {
    var months: number;
    months = (d2.getFullYear() - d1.getFullYear()) * 12;
    months -= d1.getMonth();
    months += d2.getMonth();
    return months < 0 ? 0 : months + 1;
  }

  onValueChange(event) {
    if (event && event.length > 1) {
      var diffMonths = this.monthDiff(event[0], event[1]);
      var years = Math.floor(diffMonths / 12);
      var months = diffMonths - years * 12;
      if (years && !months) this.duration = years + "Years";
      else if (years && months) this.duration = years + "Years and " + months + "Months";
      else if (!years && months) this.duration = months + "Months";
    }
  }

  onCloseModal() {
    this.bsModalRef.hide();
  }

  openNextModal() {
    if (this.forecastId > 0) {
      if (
        this.tempData["startDate"] == formatDate(this.defineforecast.controls["forecastdate"].value[0], "YYYY-MM-DDT00:00:00") &&
        this.tempData["forecastDate"] == formatDate(this.defineforecast.controls["forecastdate"].value[1], "YYYY-MM-DDT00:00:00") &&
        this.tempData["scopeOfTheForecast"] == this.defineforecast.controls["scopeofforecast"].value &&
        this.tempData["forecastNo"] == this.defineforecast.controls["ForecastID"].value
      ) {
        this.bsModalRef.hide();
        this.event.emit({ type: "next", forecastId: this.forecastId });
        return;
      }
    }
    this.date = new Date();
    this.Foracstinfoobj = {
      ForecastID: this.forecastId,
      ForecastNo: this.defineforecast.controls["ForecastID"].value,
      Methodology: "",
      DataUsage: "",
      scopeOfTheForecast: this.defineforecast.controls["scopeofforecast"].value,
      Status: "OPEN",
      StartDate: formatDate(this.defineforecast.controls["forecastdate"].value[0], "DD/MMM/YYYY"),
      Period: "", //Removed with discussion
      ForecastDate: formatDate(this.defineforecast.controls["forecastdate"].value[1], "DD/MMM/YYYY"),
      SlowMovingPeriod: this.defineforecast.controls["Period"].value,
      ForecastType: "S",
      Method: "",
      Extension: 4,
      LastUpdated: this.date,
      Countryid: localStorage.getItem("countryid"),
    };
    this._APIwithActionService.postAPI(this.Foracstinfoobj, "Forecsatinfo", "saveforecastinfo").subscribe((data) => {
      if (data["_body"] != 0) {
        this.Id = data["_body"];
        this._GlobalAPIService.SuccessMessage("Forecast Info Saved Successfully");
        this.bsModalRef.hide();
        this.event.emit({ type: "next", forecastId: this.Id });
      } else {
        this._GlobalAPIService.FailureMessage("Duplicate ForecastID");
      }
    });
  }
}
