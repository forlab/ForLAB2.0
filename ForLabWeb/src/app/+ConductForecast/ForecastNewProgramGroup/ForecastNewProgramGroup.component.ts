import { Component, OnInit, EventEmitter } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { GlobalAPIService } from '../../shared/GlobalAPI.service';
import { APIwithActionService } from '../../shared/APIwithAction.service';
import { DeleteModalComponent } from 'app/shared/ui/datatable/DeleteModal/DeleteModal.component';

@Component({
    selector: 'app-forecast-new-program-group',
    templateUrl: './ForecastNewProgramGroup.component.html',
    styleUrls: ['ForecastNewProgramGroup.component.css']
})

export class ForecastNewProgramGroupComponent implements OnInit {
    public event: EventEmitter<any> = new EventEmitter();
    forecastId: number;
    programId: number;
    patientgrouplist = new Array();
    patientgroup = new Array();
    newProgram: FormGroup;
    bsDeleteModalRef: BsModalRef;

    constructor(private _APIwithActionService: APIwithActionService, public bsModalRef: BsModalRef, private modalService: BsModalService,
        private _fb: FormBuilder, private _GlobalAPIService: GlobalAPIService) { }

    ngOnInit() {
        this.newProgram = this._fb.group({
            groupName: ["", Validators.compose([Validators.required])],
        });
        this.getpatientgroup();

    }

    onCloseModal() {
        this.bsModalRef.hide();
    }

    openNextModal() {
        this.bsModalRef.hide();
        this.event.emit({ type: "next" });
    }

    getpatientgroup() {
        this._APIwithActionService.getDataList('MMProgram', 'Getpatientgroup').subscribe((resp) => {
            this.patientgrouplist = resp;
            console.log('this.patientgrouplist', resp);
            if (this.programId != 0) {
                this.patientgroup = this.patientgrouplist.filter(x => x.programId === this.programId && x.isActive === "Yes");
            }
        })
    }

    onCreateGroup() {
        let newgroup = new Object();
        newgroup = {
            Id: 0,
            GroupName: this.newProgram.controls['groupName'].value,
            ProgramId: this.programId,
            IsActive: true
        }
        this._APIwithActionService.postAPI(newgroup, 'MMProgram', 'savegroup').subscribe((data) => {
            if (data["_body"] != 0) {
                this._GlobalAPIService.SuccessMessage("Group saved successfully");
                this.newProgram.patchValue({
                    groupName: ''
                })
                this.newProgram.controls['groupName'].markAsUntouched();
                this.getpatientgroup();
            }
            else {
                this._GlobalAPIService.FailureMessage("Group Name must not be Duplicate");
            }
        })
    }

    deleteGroup(groupItem: any) {
        this.bsDeleteModalRef = this.modalService.show(DeleteModalComponent, { class: 'modal-delete' });
        this.bsDeleteModalRef.content.event.subscribe(res => {
            if (res.type == "delete") {
                var index = this.patientgroup.findIndex(x => x.id === groupItem.id);
                if (this.patientgroup[index].isActive == "Yes") {
                    this.patientgroup[index].isActive = "No"
                }
                else {
                    this.patientgroup[index].isActive = "Yes"
                }
                var groupoject = {
                    Id: groupItem.id,
                    GroupName: groupItem.groupName,
                    ProgramId: groupItem.programId,
                    IsActive: this.patientgroup[index].isActive
                }
                this._APIwithActionService.putAPI(groupItem.id, groupoject, 'MMProgram', 'updategroup').subscribe((data) => {
                    console.log("deleteGroup", data);
                    if (data["_body"] != 0) {
                        this._GlobalAPIService.SuccessMessage("Group Deleted Successfully");
                        this.getpatientgroup();
                    } else {
                        this._GlobalAPIService.FailureMessage("Group already used so you can't delete this Group");
                    }
                })
            }
        })

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

