import { Component, OnInit, EventEmitter } from "@angular/core";
import { Router, ActivatedRoute } from "@angular/router";
import { BsModalRef, BsModalService } from "ngx-bootstrap";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";

import { GlobalAPIService } from "../../shared/GlobalAPI.service";
import { APIwithActionService } from "../../shared/APIwithAction.service";

@Component({
  selector: "app-forecast-morbidity-disease",
  templateUrl: "./ForecastMorbidityDisease.component.html",
  styleUrls: ["ForecastMorbidityDisease.component.css"],
})
export class ForecastMorbidityDiseaseComponent implements OnInit {
  public event: EventEmitter<any> = new EventEmitter();
  forecastId: number;
  morbidityPrograms = new Array();
  selectedProgram = 0;
  newProgram: FormGroup;
  selectedprogramid: number = 0;
  loading = true;

  constructor(private _APIwithActionService: APIwithActionService, public bsModalRef: BsModalRef, private _fb: FormBuilder, private _GlobalAPIService: GlobalAPIService) {}

  ngOnInit() {
    this.newProgram = this._fb.group({
      id: 0,
      ProgramName: ["", Validators.compose([Validators.required])],
    });

    this._APIwithActionService.getDataList("MMProgram", "Get").subscribe((resp) => {
      this._APIwithActionService.getDatabyID(this.forecastId, "Forecsatinfo", "GetbyId").subscribe((info) => {
        this.loading = false;
        this.morbidityPrograms = resp;
        if (info.programId) {
          this.selectedProgram = this.morbidityPrograms.findIndex((x) => x.id == info.programId);
        }
        this.selectedprogramid = this.morbidityPrograms[this.selectedProgram].id;
      });
    });
  }

  handleChangeProgram(index) {
    this.selectedProgram = index;
    this.selectedprogramid = this.morbidityPrograms[this.selectedProgram].id;
  }

  onCreateProgram() {
    let newprogram = new Object();
    newprogram = {
      Id: this.newProgram.controls["id"].value,
      ProgramName: this.newProgram.controls["ProgramName"].value,
    };
    this._APIwithActionService.postAPI(newprogram, "MMProgram", "SaveProgram").subscribe((data) => {
      if (data["_body"] != 0) {
        this._GlobalAPIService.SuccessMessage("Program saved successfully");
        this.newProgram.patchValue({
          ProgramName: "",
        });
        this.newProgram.controls["ProgramName"].markAsUntouched();
        this.newProgram.patchValue({
          id: data["_body"],
          ProgramName: newprogram["ProgramName"],
        });
        this.selectedprogramid = Number(this.newProgram.controls["id"].value);
        // this.getgeneralassumption();
        this.bsModalRef.hide();
        this.event.emit({ type: "next", methodology: "createProgram", programId: this.selectedprogramid });
      } else {
        this._GlobalAPIService.FailureMessage("Program Name must not be duplicate");
      }
    });
    // this.selectedprogramid = 95;
    // this.bsModalRef.hide();
    // this.event.emit({ type: "next", methodology: "createProgram", programId: this.selectedprogramid });
  }

  onCloseModal() {
    this.bsModalRef.hide();
  }

  openNextModal() {
    this._APIwithActionService.putAPI(this.forecastId, this.selectedprogramid, "Forecsatinfo", "updateprogram").subscribe((data) => {
      this.bsModalRef.hide();
      this.event.emit({ type: "next", programId: this.selectedprogramid });
    });
  }

  openPreviousModal() {
    this.bsModalRef.hide();
    this.event.emit({ type: "back" });
  }
}
