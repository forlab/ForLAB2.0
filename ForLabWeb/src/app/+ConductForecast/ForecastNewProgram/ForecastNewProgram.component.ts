import { Component, OnInit, EventEmitter } from "@angular/core";
import { Router, ActivatedRoute } from "@angular/router";
import { BsModalRef, BsModalService } from "ngx-bootstrap";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";

import { GlobalAPIService } from "../../shared/GlobalAPI.service";
import { APIwithActionService } from "../../shared/APIwithAction.service";

@Component({
  selector: "app-forecast-new-program",
  templateUrl: "./ForecastNewProgram.component.html",
  styleUrls: ["ForecastNewProgram.component.css"],
})
export class ForecastNewProgramComponent implements OnInit {
  public event: EventEmitter<any> = new EventEmitter();
  forecastId: number;
  forecastMethodList = ["Target_Based", "Population_Based"];
  selectedMethod = 0;
  forecastVariableList = ["CurrentPatient", "TargetPatient"];
  selectedVariable = 0;

  constructor(private _APIwithActionService: APIwithActionService, public bsModalRef: BsModalRef, private _fb: FormBuilder) {}

  ngOnInit() {}

  handleChangeMethod(index) {
    this.selectedMethod = index;
  }
  handleChangeVariable(index) {
    this.selectedVariable = index;
  }

  onCloseModal() {
    this.bsModalRef.hide();
  }

  openNextModal() {
    this.bsModalRef.hide();
    this.event.emit({ type: "next" });
  }

  openPreviousModal() {
    this.bsModalRef.hide();
    this.event.emit({ type: "back" });
  }

  cancelAndSelectCurrent() {
    this.bsModalRef.hide();
    this.event.emit({ type: "back", methodology: "cancelCreateProgram" });
  }
}
