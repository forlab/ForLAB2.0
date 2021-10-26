import { Component, OnInit, EventEmitter } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { GlobalAPIService } from '../../shared/GlobalAPI.service';
import { APIwithActionService } from '../../shared/APIwithAction.service';
import { DeleteModalComponent } from 'app/shared/ui/datatable/DeleteModal/DeleteModal.component';

@Component({
    selector: 'app-forecast-new-prog-assum-patient',
    templateUrl: './ForecastNewProgAssumPatient.component.html',
    styleUrls: ['ForecastNewProgAssumPatient.component.css']
})

export class ForecastNewProgAssumPatientComponent implements OnInit {
    public event: EventEmitter<any> = new EventEmitter();
    forecastId: number;
    programId: number;
    forecastVarTypeList: any[] = ["Numeric", "Percentage"];
    selectedType = 0;
    forecastVarEffectList: any[] = ["Positive", "Negative"];
    selectedEffect = 0;
    generalassumptionlist = new Array();
    patientassumption = new Array();
    newProgram: FormGroup;
    bsDeleteModalRef: BsModalRef;

    constructor(private _APIwithActionService: APIwithActionService, public bsModalRef: BsModalRef, private _fb: FormBuilder,
        private _GlobalAPIService: GlobalAPIService, private modalService: BsModalService) { }

    ngOnInit() {
        this.newProgram = this._fb.group({
            variableName: ["", Validators.compose([Validators.required])],
        });
        this.getgeneralassumption();
    }

    getgeneralassumption() {
        this._APIwithActionService.getDataList('MMProgram', 'GetGeneralAssumptionList').subscribe((resp) => {
            this.generalassumptionlist = resp;
            this.patientassumption = this.generalassumptionlist.filter(x =>
                x.programId === this.programId && x.assumptionType === 1 && x.isActive === "Yes");
            console.log('this.patientassumption', this.patientassumption);
        })
    }

    deleteAssumption(item: any) {
        this.bsDeleteModalRef = this.modalService.show(DeleteModalComponent, { class: 'modal-delete' });
        this.bsDeleteModalRef.content.event.subscribe(res => {
            if (res.type == "delete") {
                var index = this.patientassumption.findIndex(x => x.id === item.id);
                if (this.patientassumption[index].isActive == "Yes") {
                    this.patientassumption[index].isActive = "No"
                }
                else {
                    this.patientassumption[index].isActive = "Yes"
                }
                var parameterobject = {
                    Id: item.id,
                    VariableName: item.variableName,
                    VariableDataType: item.variableDataType,
                    VariableDataTypeName: item.variableDataTypeName,
                    UseOn: item.useOn,
                    VariableFormula: item.variableFormula,
                    ProgramId: item.programId,
                    VarCode: item.varCode,
                    AssumptionType: item.assumptionType,
                    AssumptionTypename: item.assumptionTypename,
                    VariableEffect: item.variableEffect,
                    IsActive: item.isActive
                }
                this._APIwithActionService.putAPI(item.id, parameterobject, 'MMProgram', 'updategeneralassumptions').subscribe((data) => {
                    if (data["_body"] != 0) {
                        this._GlobalAPIService.SuccessMessage("Variable Deleted Successfully");
                        this.getgeneralassumption();
                    } else {
                        this._GlobalAPIService.FailureMessage("Variable already used so you can't delete this Group");
                    }
                })
            }
        })
    }

    onCreateVarieble() {
        let postassumption = new Object();
        postassumption = {
            Id: 0,
            VariableName: this.newProgram.controls["variableName"].value,
            VariableDataType: this.selectedType + 1,
            VariableDataTypeName: this.selectedType == 0 ? "Numeric" : "Percentage",
            UseOn: "OnAllSite",
            ProgramId: this.programId,
            VarCode: "xx",
            AssumptionType: 1,
            AssumptionTypename: "Patient_Number_Assumption",
            VariableEffect: this.selectedEffect == 0 ? true : false,
            IsActive: "Yes"
        }

        this._APIwithActionService.postAPI(postassumption, 'MMProgram', 'savegeneralassumptions').subscribe((data) => {
            if (data["_body"] != 0) {
                this.getgeneralassumption();
                this.newProgram.patchValue({ variableName: '' })
                this.newProgram.controls['variableName'].markAsUntouched();
            }
            else {
                this._GlobalAPIService.FailureMessage("Variable Name must not be duplicate");
            }
        })
    }
    handleChangeType(index) {
        this.selectedType = index;
    }
    handleChangeEffect(index) {
        this.selectedEffect = index;
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

