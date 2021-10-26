import { Component, OnInit, EventEmitter } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap';
import { FormBuilder, FormGroup, Validators, FormArray } from '@angular/forms';

import { GlobalAPIService } from '../../shared/GlobalAPI.service';
import { APIwithActionService } from '../../shared/APIwithAction.service';

@Component({
    selector: 'app-forecast-morbidity-site',
    templateUrl: './ForecastMorbiditySite.component.html',
    styleUrls: ['ForecastMorbiditySite.component.css']
})

export class ForecastMorbiditySiteComponent implements OnInit {
    public event: EventEmitter<any> = new EventEmitter();
    forecastId: number;
    Show: boolean = false;
    siteList = new Array();
    selectSiteList = new Array();
    selectedSite: any;
    siteForm: FormGroup;
    loading = true;

    constructor(private _fb: FormBuilder, private _APIwithActionService: APIwithActionService, public bsModalRef: BsModalRef, private _GlobalAPIService: GlobalAPIService) { }

    ngOnInit() {
        this.siteForm = this._fb.group({
            _patientnumber: this._fb.array([]),
        });
        this._APIwithActionService.getDatabyID(localStorage.getItem("countryid"), 'Site', 'GetAll').subscribe((data) => {
            this.selectSiteList = data.aaData;
            this._APIwithActionService.getDatabyID(this.forecastId, "Forecsatinfo", "Getbyforecastid").subscribe((data) => {
                this.loading = false;
                for (var idx = 0; idx < data.length; idx++) {
                    this.addpatientnumber();
                    (<FormGroup>((<FormArray>this.siteForm.controls["_patientnumber"]).controls[idx])).patchValue({
                        id: data[idx].id,
                        ForecastinfoID: data[idx].forecastinfoID,
                        SiteID: data[idx].siteID,
                        SiteName: data[idx].siteName,
                        Currentpatient: data[idx].currentPatient,
                        Targetpatient: data[idx].targetPatient,
                        PopulationNumber: data[idx].populationNumber,
                        PrevalenceRate: data[idx].prevalenceRate,
                    });
                }
            })
        })
        if (this.forecastId > 0) {
            this._APIwithActionService.getDatabyID(this.forecastId, 'MMProgram', 'Getforecastparameterbyforecastid').subscribe((resp) => {
                // var forecastMethod = 0;
                // if (resp[0]) forecastMethod = resp[0]["forecastMethod"];
                // console.log('forecastMethod', forecastMethod);
                // if (forecastMethod == 1) {
                //     this.Show = true
                // } else {
                //     this.Show = false
                // }
                // console.log(this.Show);
                this.Show = true;
            })


        }

    }

    addpatientnumber() {
        (<FormArray>this.siteForm.controls["_patientnumber"]).push(
            this.initpatientnumber()
        );
    }
    initpatientnumber() {
        let patientnumber: FormGroup = this._fb.group(
            {
                id: 0,
                ForecastinfoID: 0,
                SiteID: 0,
                SiteName: [{ value: "", disabled: true }],
                Currentpatient: "0",
                Targetpatient: "0",
                PopulationNumber: "0",
                PrevalenceRate: "0",
            }
        );
        return patientnumber;
    }

    deleteSite(index) {
        (<FormArray>(this.siteForm.controls["_patientnumber"])).removeAt(index);
    }

    onCreateSite() {
        if (this.selectedSite) {
            this.addpatientnumber();
            let patientnumber = <FormArray>(
                this.siteForm.controls["_patientnumber"]
            );
            var lastIdx = patientnumber.getRawValue().length - 1;
            (<FormGroup>((<FormArray>this.siteForm.controls["_patientnumber"]).controls[lastIdx])).patchValue({
                id: 0,
                ForecastinfoID: this.forecastId,
                SiteID: this.selectedSite["siteID"],
                SiteName: this.selectedSite["siteName"],
                Currentpatient: 0,
                Targetpatient: 0,
                PopulationNumber: 0,
                PrevalenceRate: 0,
            });
        }
    }

    getSiteList(siteID) {
        if (siteID > 0) {
            this.selectedSite = this.selectSiteList.find(x => { return x.siteID === +siteID });
        }
    }

    onCloseModal() {
        this.bsModalRef.hide();
    }

    openNextModal() {
        let patientnumber = <FormArray>(
            this.siteForm.controls["_patientnumber"]
        );
        let patientnumberusage = new Array();
        if (patientnumber.getRawValue().length > 0) {
            for (var idx = 0; idx < patientnumber.getRawValue().length; idx++) {
                if (patientnumber.getRawValue()[idx].Currentpatient > patientnumber.getRawValue()[idx].Targetpatient) {
                    this._GlobalAPIService.FailureMessage(patientnumber.getRawValue()[idx].SiteName + " Should have Current Patient less than target Patient");
                    return;
                } else if (patientnumber.getRawValue()[idx].Targetpatient == 0) {
                    this._GlobalAPIService.FailureMessage(patientnumber.getRawValue()[idx].SiteName + " Should have target Patient bigger than zero");
                    return;
                } else {
                    patientnumberusage.push(patientnumber.getRawValue()[idx]);
                }
            }
        } else {
            this._GlobalAPIService.FailureMessage("Please Select atleast one site");
            return;
        }
        let newobject = new Object();
        newobject = {
            patientnumberusage,
        };

        this._APIwithActionService.postAPI(newobject, "Forecsatinfo", "saveforecastsiteinfo").subscribe((data) => {
            if (data["_body"] != 0) {
                this._GlobalAPIService.SuccessMessage("SitebySite information saved successfully");
            }
            this.bsModalRef.hide();
            this.event.emit({ type: "next" });
        })
    }

    openPreviousModal() {
        this.bsModalRef.hide();
        this.event.emit({ type: "back" });
    }

}

