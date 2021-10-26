import { Component, OnInit, Renderer, ViewChild, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormArray } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { GlobalAPIService } from '../../../shared/GlobalAPI.service';
import { APIwithActionService } from '../../../shared/APIwithAction.service';
import { Location } from '@angular/common';
import { ModalDirective, BsModalRef } from "ngx-bootstrap";
import { ModalDatatableComponent } from 'app/shared/ui/datatable/ModalDatatable.component';

@Component({
    selector: 'app-site-add-step-one',
    templateUrl: './SiteAddStepOne.component.html',
    styleUrls: ['./SiteAddStepOne.component.css']
})

export class SiteAddStepOneComponent implements OnInit {

    public event: EventEmitter<any> = new EventEmitter();
    itemID: any;
    buttonstatus = true;
    btnopen = false;
    btnedit = true;
    btnclose = true;

    siteForm1: FormGroup;
    title: string = "Create";
    errorMessage: any;
    SiteStatus = new Array();
    SiteInstrument = new Array();
    regionList: any[];
    categoryList: any[];
    InstrumentList = new Array();
    Instrumentform: FormGroup;
    statusform: FormGroup;
    testingAreaList = new Array();
    testingdaysList = new Array();
    testingareaid: number = 0;
    testingareaname: string;
    instrumentid: number = 0;
    instrumentname: string;
    instestingareaid: number = 0;
    instestingareaname: string;
    refsiteid: number = 0;
    refsitename: string;
    Reftestingareaid: number = 0;
    Reftestingareaname: string;
    RefSiteList = new Array();
    countryList = new Array();
    reftestingdaysList = new Array();
    Delidsiteinstrument: string = "";
    Delidsitetestingdays: string = "";
    Delidreferrallink: string = "";
    totalrunpercentage: any;
    Countryid: any;
    disbool: boolean;

    constructor(
        private _fb: FormBuilder,
        private _avRoute: ActivatedRoute,
        private _APIwithActionService: APIwithActionService,
        private _GlobalAPIService: GlobalAPIService,
        private _router: Router,
        public bsModalRef: BsModalRef
    ) {

        this.getcountry();
        this.getCategory();
        this.getinstrument();
        this.getTestingArea();

        if (localStorage.getItem("role") == "admin") {
            this.Countryid = '0'
            this.disbool = false
        } else {
            this.Countryid = localStorage.getItem("countryid")
            this.disbool = true
        }
    }

    getcountry() {
        this._APIwithActionService.getDataList('Site', 'Getcountrylist').subscribe((data) => {
            this.countryList = data;
        })

    }

    getTestingArea() {
        this._GlobalAPIService.getDataList('TestArea').subscribe((data) => {
            this.testingAreaList = data.aaData
        }), err => {
            console.log(err);
        }

    }

    ngOnInit() {
        if (this.itemID > 0) {
            this.title = "Edit";
            this._APIwithActionService.getDatabyID(this.itemID, 'Site', 'Getbyid').subscribe((resp) => {
                this.siteForm1.patchValue({
                    SiteID: resp["siteID"],
                    SiteName: resp["siteName"],
                    CategoryID: resp["categoryID"],
                    RegionId: resp["regionid"],
                    countryid: resp["countryId"],
                    WorkingDays: resp["workingDays"],
                    Latitude: resp["latitude"],
                    Longitude: resp["longitude"]

                    // formControlName2: myValue2 (can be omitted)
                });
                this.SiteStatus = resp["_siteStatuses"]
                this.SiteInstrument = resp["_siteInstruments"]
                this.testingdaysList = resp["_sitetestingdays"]
                this.reftestingdaysList = resp["_ReferralLinkage"]
                if (localStorage.getItem("role") != "admin") {
                    this.siteForm1.get('countryid').disable();
                }
                this.getRegion(this.Countryid)
                // this.fillsitestatus()
                // this.fillrefTestingdays()
                this.fillsiteinstrument()
                this.fillTestingdays()
                this.SiteInstrument.splice(0, this.SiteInstrument.length)
            }, error => this.errorMessage = error);
        }

        this.Instrumentform = this._fb.group({
            selectins: this._fb.array([])
        })
        this.statusform = this._fb.group({
            openedFrom: null
        })
        this.siteForm1 = this._fb.group({
            SiteID: 0,
            SiteName: ['', Validators.compose([Validators.required, Validators.maxLength(64)])],
            CategoryID: ['', [Validators.required]],
            RegionId: ['', [Validators.required]],
            countryid: [{ value: this.Countryid, disabled: this.disbool }, [Validators.required]],
            WorkingDays: 0,
            Latitude: 0,
            Longitude: 0,
            testingdays: 0,
            TestingAreaID: 0,
            Reftestingdays: 0,
            _sitetestingdays: this._fb.array([]),
            _ReferralLinkage: this._fb.array([]),
            _siteStatuses: this._fb.array([]),
            _siteinstrument: this._fb.array([])
        })
        // if (this.Countryid) {
            this.getRegion(this.Countryid);
        // }
    }


    initsiteinstrument() {
        let siteinstrument: FormGroup = this._fb.group({
            id: 0,
            instrumentID: 0,
            instrumentName: [{ value: '', disabled: true }],
            testingareaId: 0,
            testingareaName: [{ value: '', disabled: true }],
            siteID: 0,
            quantity: 0,
            TestRunPercentage: [{ value: 0 }],
        });
        return siteinstrument;
    }
    addsiteinstrument() {
        (<FormArray>this.siteForm1.controls["_siteinstrument"]).push(
            this.initsiteinstrument()
        );
    }
    initreftestingdays() {
        let testingdays: FormGroup = this._fb.group({
            id: 0,
            testingareaid: 0,
            testingareaName: [{ value: '', disabled: true }],
            siteid: 0,
            siteName: [{ value: '', disabled: true }],
            testingdays: 0,
            referensiteid: 0

        });
        return testingdays;
    }
    addreftestingdays() {
        (<FormArray>this.siteForm1.controls["_ReferralLinkage"]).push(
            this.initreftestingdays()
        );
    }
    addnewreftestingdays() {
        if (this.Reftestingareaid != 0 && this.refsiteid) {
            let index = this.reftestingdaysList.indexOf(this.reftestingdaysList.find(x => x.siteid == this.refsiteid && x.testingareaid == this.Reftestingareaid))
            if (index >= 0) {
            } else {
                this.reftestingdaysList.push({
                    id: 0,
                    testingareaid: this.Reftestingareaid,
                    testingareaName: this.Reftestingareaname,
                    siteid: this.refsiteid,
                    siteName: this.refsitename,
                    testingdays: this.siteForm1.controls["Reftestingdays"].value,
                    referensiteid: this.siteForm1.controls["SiteID"].value

                })
                let boxIndex = this.reftestingdaysList.length - 1;
                this.addreftestingdays();
                (<FormGroup>((<FormArray>this.siteForm1.controls["_ReferralLinkage"]).controls[boxIndex])).patchValue({
                    id: this.reftestingdaysList[boxIndex].id,
                    testingareaid: this.reftestingdaysList[boxIndex].testingareaid,
                    testingareaName: this.reftestingdaysList[boxIndex].testingareaName,
                    siteid: this.reftestingdaysList[boxIndex].siteid,
                    siteName: this.reftestingdaysList[boxIndex].siteName,
                    testingdays: this.reftestingdaysList[boxIndex].testingdays,
                    referensiteid: this.reftestingdaysList[boxIndex].referensiteid


                });
            }
        }
    }
    inittestingdays() {
        // Initialize add Box form field
        let testingdays: FormGroup = this._fb.group({
            id: 0,
            testingareaid: [''],
            testingareaName: [{ value: '', disabled: true }],
            testingdays: 0,
            siteID: 0
        });
        return testingdays;
    }
    addtestingdays() {
        (<FormArray>this.siteForm1.controls["_sitetestingdays"]).push(
            this.inittestingdays()
        );
    }
    Addtestingareaformarray() {
        let ss = (<FormArray>this.siteForm1.controls["_sitetestingdays"]);
        ss.controls = [];
        for (let boxIndex = 0; boxIndex < this.testingdaysList.length; boxIndex++) {
            this.addtestingdays();
            (<FormGroup>((<FormArray>this.siteForm1.controls["_sitetestingdays"]).controls[boxIndex])).patchValue({
                id: this.testingdaysList[boxIndex].id,
                testingareaid: this.testingdaysList[boxIndex].testingareaid,
                testingareaName: this.testingdaysList[boxIndex].testingareaName,
                testingdays: this.testingdaysList[boxIndex].testingdays,
                siteID: this.testingdaysList[boxIndex].siteID,

            });

        }
    }

    Addmultitestingarea(ins: any, ischeked: boolean) {
        if (ischeked == true) {
            let index = this.testingdaysList.indexOf(this.testingdaysList.find(x => x.testingareaid === this.testingareaid))
            if (index >= 0) {
            } else {
                this.testingdaysList.push({
                    id: 0,
                    testingareaid: ins.testingAreaID,
                    testingareaName: ins.areaName,
                    testingdays: 10,
                    siteID: this.siteForm1.controls["SiteID"].value
                })
            }
        } else {
            let index = this.testingdaysList.indexOf(this.testingdaysList.find(x => x.testingareaid === this.testingareaid))
            if (index >= 0) {
                this.testingdaysList.splice(index, 1);
            }
        }
    }

    inputvalue(args: any, index: any) {

    }

    bindrefSite(args) {
        this.refsiteid = args.target.value;
        this.refsitename = args.target.options[args.target.selectedIndex].text;
    }
    bindsitelist(args) {
        this._APIwithActionService.getDatabyID(args.target.value, 'Site', 'GetSitebyReg').subscribe((data) => {
            this.RefSiteList = data;
        })
    }
    selectReftestingArea(args) {
        this.Reftestingareaid = args.target.value;
        this.Reftestingareaname = args.target.options[args.target.selectedIndex].text;
    }
    selecttestingArea(args) {
        this.testingareaid = args.target.value;
        this.testingareaname = args.target.options[args.target.selectedIndex].text;
    }
    SelectInstrument(args) {
        this.instrumentid = args.target.value;
        this.instrumentname = args.target.options[args.target.selectedIndex].text;
    }
    SelectsiteindtrumentArea(args) {
        this.instestingareaid = args.target.value;
        this.instestingareaname = args.target.options[args.target.selectedIndex].text;
        // this.getinstrument(args.target.value);
    }
    initsitestatus() {
        // Initialize add Box form field
        let SiteStatus: FormGroup = this._fb.group({
            id: 0,
            openedFrom: [{ value: null, disabled: true }],
            closedOn: null,
            siteID: 0
        });
        return SiteStatus;
    }
    addsitestatus() {
        (<FormArray>this.siteForm1.controls["_siteStatuses"]).push(
            this.initsitestatus()
        );
        // get box length for box name like box 1,box 2 in sidebar boxes combo list
    }

    fillsitestatus() {
        for (let boxIndex = 0; boxIndex < this.SiteStatus.length; boxIndex++) {
            this.addsitestatus();
            (<FormGroup>((<FormArray>this.siteForm1.controls["_siteStatuses"]).controls[boxIndex])).patchValue({
                id: this.SiteStatus[boxIndex].id,
                openedFrom: new Date(this.SiteStatus[boxIndex].openedFrom),
                closedOn: new Date(this.SiteStatus[boxIndex].closedOn),
                siteID: this.SiteStatus[boxIndex].siteID,
            });

            if (this.SiteStatus[boxIndex].closedOn == null) {
                this.btnopen = false;
            } else {
                this.btnopen = true;
            }

        }
    }
    fillTestingdays() {
        for (let boxIndex = 0; boxIndex < this.testingdaysList.length; boxIndex++) {
            this.addtestingdays();
            (<FormGroup>((<FormArray>this.siteForm1.controls["_sitetestingdays"]).controls[boxIndex])).patchValue({
                id: this.testingdaysList[boxIndex].id,
                testingareaid: this.testingdaysList[boxIndex].testingareaid,
                testingareaName: this.testingdaysList[boxIndex].testingareaName,
                testingdays: this.testingdaysList[boxIndex].testingdays,
                siteID: this.testingdaysList[boxIndex].siteID,
            });
        }
    }

    fillrefTestingdays() {
        for (let boxIndex = 0; boxIndex < this.reftestingdaysList.length; boxIndex++) {
            this.addreftestingdays();
            (<FormGroup>((<FormArray>this.siteForm1.controls["_ReferralLinkage"]).controls[boxIndex])).patchValue({
                id: this.reftestingdaysList[boxIndex].id,
                testingareaid: this.reftestingdaysList[boxIndex].testingareaid,
                testingareaName: this.reftestingdaysList[boxIndex].testingareaName,
                siteid: this.reftestingdaysList[boxIndex].siteid,
                siteName: this.reftestingdaysList[boxIndex].siteName,
                testingdays: this.reftestingdaysList[boxIndex].testingdays,
                referensiteid: this.reftestingdaysList[boxIndex].referensiteid
            });
        }
    }

    fillsiteinstrument() {
        for (let boxIndex = 0; boxIndex < this.SiteInstrument.length; boxIndex++) {
            this.addsiteinstrument();
            (<FormGroup>((<FormArray>this.siteForm1.controls["_siteinstrument"]).controls[boxIndex])).patchValue({
                id: this.SiteInstrument[boxIndex].id,
                instrumentID: this.SiteInstrument[boxIndex].instrumentID,
                instrumentName: this.SiteInstrument[boxIndex].instrumentName,
                testingareaId: this.SiteInstrument[boxIndex].testingareaId,
                testingareaName: this.SiteInstrument[boxIndex].testingareaName,
                siteID: this.SiteInstrument[boxIndex].siteID,
                quantity: this.SiteInstrument[boxIndex].quantity,
                TestRunPercentage: this.SiteInstrument[boxIndex].testRunPercentage
            });
        }
    }
    getRegion(countryid: any) {
        this._APIwithActionService.getDatabyID(countryid, 'Site', 'GetregionbyCountryID').subscribe((data) => {
            this.regionList = data
            console.log( this.regionList )
        }), err => {
            console.log(err);
        }

    }
    getCategory() {
        this._GlobalAPIService.getDataList('SiteCategory').subscribe((data) => {
            this.categoryList = data.aaData
        }), err => {
            console.log(err);
        }

    }
    getinstrument() {
        this._APIwithActionService.getDataList('Instrument', 'GetAll').subscribe((data) => {
            this.InstrumentList = data.aaData
        }), err => {
            console.log(err);
        }
    }
    editSite(siteid) {
        this.buttonstatus = false;
    }
    next() {
        if (!this.siteForm1.valid) {
            return;
        }

        this.totalrunpercentage = 0;
        let _sitestatus = new Array();
        let _sitestatus1 = <FormArray>this.siteForm1.controls["_siteStatuses"]
        _sitestatus1.getRawValue().forEach(x => {
            _sitestatus.push(x)
        });
        let _siteins = new Array();
        let _siteins1 = <FormArray>this.siteForm1.controls["_siteinstrument"]
        _siteins1.getRawValue().forEach(x => {
            _siteins.push(x)
        });
        let diffareaid = new Array();
        for (let index = 0; index < _siteins.length; index++) {
            const element = _siteins[index];
            let j = diffareaid.findIndex(x => x.areaid === element.testingareaId)
            if (j >= 0) {
                diffareaid[j].testrunpercentage = parseFloat(diffareaid[j].testrunpercentage) + parseFloat(element.TestRunPercentage)
            }
            else {
                diffareaid.push({
                    areaid: element.testingareaId,
                    testrunpercentage: element.TestRunPercentage
                })
                //  this.totalrunpercentage = element.TestRunPercentage
            }

        }
        for (let index = 0; index < diffareaid.length; index++) {
            const element = diffareaid[index];
            if (element.testrunpercentage > 100 || element.testrunpercentage < 100) {
                this._GlobalAPIService.FailureMessage("Test Run Percentage should be equal 100 for same area")
                return;
            }
        }
        let _reflink = new Array();
        let _reflink1 = <FormArray>this.siteForm1.controls["_ReferralLinkage"]
        _reflink1.getRawValue().forEach(x => {
            _reflink.push(x)
        });
        let _sitetest = new Array();
        let _sitetest1 = <FormArray>this.siteForm1.controls["_sitetestingdays"]
        _sitetest1.getRawValue().forEach(x => {
            _sitetest.push(x)
        });
        let newdata = new Object();
        newdata = {
            SiteID: this.siteForm1.value.SiteID,
            SiteName: this.siteForm1.value.SiteName,
            CategoryID: this.siteForm1.value.CategoryID,
            countryid: this.siteForm1.getRawValue().countryid,
            RegionId: this.siteForm1.value.RegionId,
            WorkingDays: this.siteForm1.value.WorkingDays,
            Latitude: this.siteForm1.value.Latitude,
            Longitude: this.siteForm1.value.Longitude,
            Reftestingdays: this.siteForm1.value.Reftestingdays,
            _sitetestingdays: _sitetest,
            _ReferralLinkage: _reflink,
            _siteStatuses: _sitestatus,
            _siteinstruments: _siteins
        }

        this.event.emit({ type: "next", itemID: this.itemID, data: newdata })
        this.bsModalRef.hide();
        // if (this.title == "Create") {
        //     this._APIwithActionService.postAPI(newdata, 'Site', 'Post01')
        //         .subscribe((data) => {
        //             if (data["_body"] == "Success") {

        //                 this._GlobalAPIService.SuccessMessage("Site Saved Successfully");
        //                 this._router.navigate(['/Managedata/ManagedataList', 7]);
        //             } else {
        //                 this._GlobalAPIService.FailureMessage(data["_body"])
        //             }
        //         }, error => this.errorMessage = error)
        // } else if (this.title == "Edit") {
        //     this._APIwithActionService.putAPI(this.id, newdata, 'Site', 'Put01').subscribe((data) => {
        //         this._APIwithActionService.deleteData(this.Delidsiteinstrument, 'Site', 'Deletesiteinstrument').subscribe((data) => {
        //         })
        //         this._APIwithActionService.deleteData(this.Delidsitetestingdays, 'Site', 'Deletesitetestingdays').subscribe((data) => {
        //         })
        //         this._APIwithActionService.deleteData(this.Delidreferrallink, 'Site', 'Deletereferrallink').subscribe((data) => {
        //         })
        //         this._GlobalAPIService.SuccessMessage("Site Updated Successfully");
        //         this._router.navigate(['/Managedata/ManagedataList', 7]);
        //     }, error => this.errorMessage = error)
        // }

    }

    clearctrl() {
        this._router.navigate(['/Managedata/ManagedataList', 7]);
    }
    Addinformarray() {
        let ss = (<FormArray>this.siteForm1.controls["_siteinstrument"]);
        // ss.controls = [];
        for (let boxIndex = 0; boxIndex < this.SiteInstrument.length; boxIndex++) {
            this.addsiteinstrument();
            (<FormGroup>((<FormArray>this.siteForm1.controls["_siteinstrument"]).controls[ss.controls.length - 1])).patchValue({
                id: this.SiteInstrument[boxIndex].id,
                instrumentID: this.SiteInstrument[boxIndex].instrumentID,
                instrumentName: this.SiteInstrument[boxIndex].instrumentName,
                testingareaId: this.SiteInstrument[boxIndex].testingareaId,
                testingareaName: this.SiteInstrument[boxIndex].testingareaName,
                siteID: this.SiteInstrument[boxIndex].siteID,
                quantity: this.SiteInstrument[boxIndex].quantity,
                TestRunPercentage: this.SiteInstrument[boxIndex].testRunPercentage

            });

        }
        this.SiteInstrument.splice(0, this.SiteInstrument.length);
    }
    Addmultiinstrument(Ins: any, ischeked: boolean) {
        if (ischeked == true) {
            let index = this.SiteInstrument.indexOf(this.SiteInstrument.find(x => x.testingareaId === Ins.testingAreaid && x.instrumentID === Ins.instrumentID))
            if (index >= 0) {
            }
            else {
                this.SiteInstrument.push({
                    id: 0,
                    instrumentID: Ins.instrumentID,
                    instrumentName: Ins.instrumentName,
                    testingareaId: Ins.testingAreaid,
                    testingareaName: Ins.testingArea,
                    siteID: this.siteForm1.controls["SiteID"].value,
                    quantity: 1,
                    testRunPercentage: 0
                })

            }
        }
        else {
            let index = this.SiteInstrument.indexOf(this.SiteInstrument.find(x => x.testingareaId === Ins.testingAreaid && x.instrumentID === Ins.instrumentID))
            if (index >= 0) {
                this.SiteInstrument.splice(index, 1);
            }
        }
    }

    addnewopendate() {
        this.SiteStatus.push({
            id: 0,
            openedFrom: new Date(this.statusform.controls["openedFrom"].value),
            closedOn: null,
            siteID: this.itemID
        })
        let boxIndex = this.SiteStatus.length - 1;
        this.addsitestatus();
        (<FormGroup>((<FormArray>this.siteForm1.controls["_siteStatuses"]).controls[boxIndex])).patchValue({
            id: this.SiteStatus[boxIndex].id,
            openedFrom: new Date(this.SiteStatus[boxIndex].openedFrom),
            closedOn: new Date(this.SiteStatus[boxIndex].closedOn),
            siteID: this.SiteStatus[boxIndex].siteID,

        });
    }
    deltestdays(i) {
        this.testingdaysList.splice(i, 1);
        let delid: String
        delid = (<FormGroup>
            (<FormArray>this.siteForm1.controls["_sitetestingdays"]).controls[i]).controls["id"].value
        if (delid != "0") {
            this.Delidsitetestingdays = this.Delidsitetestingdays + "," + delid;
        }
        (<FormArray>this.siteForm1.controls["_sitetestingdays"]).removeAt(i)
    }
    delreftestdays(i) {
        this.reftestingdaysList.splice(i, 1);
        let delid: String
        delid = (<FormGroup>(<FormArray>this.siteForm1.controls["_ReferralLinkage"]).controls[i]).controls["id"].value
        if (delid != "0") {
            this.Delidreferrallink = this.Delidreferrallink + "," + delid;
        }
        (<FormArray>this.siteForm1.controls["_ReferralLinkage"]).removeAt(i)
    }
    delsiteinstrument(i) {
        this.SiteInstrument.splice(i, 1);
        let delid: String
        delid = (<FormGroup>(<FormArray>this.siteForm1.controls["_siteinstrument"]).controls[i]).controls["id"].value
        if (delid != "0") {
            this.Delidsiteinstrument = this.Delidsiteinstrument + "," + delid;
        }
        (<FormArray>this.siteForm1.controls["_siteinstrument"]).removeAt(i)
    }

    onCloseModal() {
        this.bsModalRef.hide();
    }

}  