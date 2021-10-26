import { Component, OnInit, EventEmitter } from "@angular/core";
import { BsModalService, BsModalRef } from "ngx-bootstrap";

import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { Router, ActivatedRoute } from "@angular/router";
import { APIwithActionService } from "../../shared/APIwithAction.service";
import { GlobalAPIService } from "../../shared/GlobalAPI.service";

@Component({
  selector: "app-forecast-test-add",
  templateUrl: "./ForecastTestAdd.component.html",
  styleUrls: ["ForecastTestAdd.component.css"],
})
export class ForecastTestAddComponent implements OnInit {
  public event: EventEmitter<any> = new EventEmitter();
  forecastId: number;
  testAreaList = new Array();
  selectedtest = new Array();
  tagArray = new Array();
  selectedTestArea = 0;
  loading = true;
  checkedAll = false;

  constructor(private _APIwithActionService: APIwithActionService, public bsModalRef: BsModalRef) {}

  ngOnInit() {
    this.selectedTestArea = 0;
    this._APIwithActionService.getDatabyID(this.forecastId, "Test", "getAlltestbytestingarea").subscribe((data) => {
      this.loading = false;
      this.testAreaList = data;
      for (var idx = 0; idx < this.testAreaList.length; idx++) {
        if (this.selectedTestArea == 0 && this.testAreaList[idx].tests.length > 0) this.selectedTestArea = idx;
        for (var idx_test = 0; idx_test < this.testAreaList[idx].tests.length; idx_test++) {
          if (this.testAreaList[idx].tests[idx_test].type == "A") {
            this.tagArray.push({
              testid: this.testAreaList[idx].tests[idx_test].testID,
              testName: this.testAreaList[idx].tests[idx_test].testName,
            });
          }
        }
      }
    });
  }

  onCloseModal() {
    this.bsModalRef.hide();
  }

  openNextModal() {
    for (var idx = 0; idx < this.tagArray.length; idx++) {
      //request only new tests (It will be removed when backend is updated)
      this.selectedtest.push({
        testid: this.tagArray[idx].testid,
        forecastID: this.forecastId,
        userId: localStorage.getItem("userid"),
      });
    }
    this._APIwithActionService.putAPI(this.forecastId, this.selectedtest, "Test", "Saveforecasttest").subscribe((data) => {
      this.bsModalRef.hide();
      this.event.emit({ type: "next" });
    });
  }

  openPreviousModal() {
    this.bsModalRef.hide();
    this.event.emit({ type: "back" });
  }

  handleSelectTestArea(index) {
    this.selectedTestArea = index;
  }

  deleteTag(removedId) {
    var removedAt = this.tagArray.findIndex((item) => item.testid == removedId);
    if (removedAt > -1) {
      this.tagArray.splice(removedAt, 1);
      for (var idx = 0; idx < this.testAreaList.length; idx++) {
        for (var idx_test = 0; idx_test < this.testAreaList[idx].tests.length; idx_test++) {
          if (this.testAreaList[idx].tests[idx_test].testID == removedId) {
            this.testAreaList[idx].tests[idx_test].type = "N";
          }
        }
      }
    }
  }

  onCheckedChange(testId: number, testName: string, testIndex: number, event: boolean) {
    if (event) {
      this.testAreaList[this.selectedTestArea].tests[testIndex].type = "A";
      this.tagArray.push({ testid: testId, testName: testName });
    } else {
      this.deleteTag(testId);
    }
  }
  onCheckedAllChange() {
    this.checkedAll = !this.checkedAll;
    this.tagArray = new Array();
    for (var idx = 0; idx < this.testAreaList.length; idx++) {
      for (var idx_test = 0; idx_test < this.testAreaList[idx].tests.length; idx_test++) {
        if (this.checkedAll) {
          this.tagArray.push({
            testid: this.testAreaList[idx].tests[idx_test].testID,
            testName: this.testAreaList[idx].tests[idx_test].testName,
          });
          this.testAreaList[idx].tests[idx_test].type = "A";
        } else {
          this.testAreaList[idx].tests[idx_test].type = "N";
        }
      }
    }
  }
}
