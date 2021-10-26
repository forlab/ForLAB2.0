import { Component, OnInit, EventEmitter } from "@angular/core";
import { Router, ActivatedRoute } from "@angular/router";
import { BsModalRef, BsModalService } from "ngx-bootstrap";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";

import { GlobalAPIService } from "../../shared/GlobalAPI.service";
import { APIwithActionService } from "../../shared/APIwithAction.service";

@Component({
  selector: "app-forecast-conduct-site",
  templateUrl: "./ForecastConductSite.component.html",
  styleUrls: ["ForecastConductSite.component.css"],
})
export class ForecastConductSiteComponent implements OnInit {
  public event: EventEmitter<any> = new EventEmitter();
  forecastId: number;
  currentConductType = -1;
  methodology: string;
  programid: number;
  exist: number = 0;
  loading = true;

  constructor(private _APIwithActionService: APIwithActionService, public bsModalRef: BsModalRef) {}

  ngOnInit() {
    this._APIwithActionService.getDatabyID(this.forecastId, "Forecsatinfo", "GetbyId").subscribe((resp) => {
      this.loading = false;
      this.methodology = resp["methodology"];
      this.programid = resp["programId"];
      switch (resp["forecastType"]) {
        case "S":
          this.currentConductType = 0;
          break;
        case "C":
          this.currentConductType = 1;
          break;
      }
    });
    this._APIwithActionService.getDatabyID(this.forecastId, "Forecsatinfo", "Isdataimported").subscribe((resp) => {
      this.exist = resp;
    });
  }

  save() {}

  handleSelectType(index) {
    var type = "";
    this.currentConductType = index;
    if (index == 0) {
      type = "S";
    } else {
      type = "C";
    }
    this._APIwithActionService.putAPI(this.forecastId, type, "Forecsatinfo", "Put01").subscribe((data) => {});
  }

  onCloseModal() {
    this.bsModalRef.hide();
  }

  openNextModal() {
    this.bsModalRef.hide();
    this.event.emit({ type: "next", methodology: this.methodology });
  }

  openPreviousModal() {
    this.bsModalRef.hide();
    this.event.emit({ type: "back", methodology: this.methodology });
  }
}
