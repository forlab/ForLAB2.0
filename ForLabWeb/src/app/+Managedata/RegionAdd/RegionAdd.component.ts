import { Component, OnInit, Renderer, Output, EventEmitter } from '@angular/core';

import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { Site } from '../RegionAdd/Region.model'
import { GlobalAPIService } from '../../shared/GlobalAPI.service';
import { APIwithActionService } from '../../shared/APIwithAction.service';

import { NotificationService } from "../../shared/utils/notification.service";
import { BsModalRef } from 'ngx-bootstrap';


@Component({
  selector: 'app-RegionAdd',
  templateUrl: './RegionAdd.component.html',
  styleUrls: ['./RegionAdd.component.css']
})

export class RegionAddComponent implements OnInit {
  public event: EventEmitter<any> = new EventEmitter();
  itemID: any;
  submitBtnName = "Add Region";
  public SiteList: Site[];
  buttonstatus = true;
  Selectedsiteid: number = 0;
  regionForm: FormGroup;
  errorMessage: any;
  countryList = new Array();
  CountryId: any
  disbool: boolean

  constructor(private notificationService: NotificationService, private _fb: FormBuilder, public bsModalRef: BsModalRef, private _APIwithActionService: APIwithActionService,
    private _GlobalAPIService: GlobalAPIService, private _router: Router) {

    if (localStorage.getItem("role") == "admin") {
      this.CountryId = ''
      this.disbool = false
    } else {
      this.CountryId = localStorage.getItem("countryid")
      this.disbool = true
    }
  }
  Opensiteadd() {
    this._router.navigate(["/Managedata/SiteAdd"]);
  }
  deletesite() {
    this.notificationService.smartMessageBox({
      title: "Deletion",
      content: "Do you want to delete " + this.itemID + " Site",
      buttons: '[No][Yes]'
    }, (ButtonPressed) => {
      if (ButtonPressed === "Yes") {
        let table = document.querySelector('table');
        this._APIwithActionService.deleteData(this.Selectedsiteid, 'Site', 'Del01').subscribe((data) => {
          if (data["_body"] != 0) {
            //  this._render.setElementStyle(table.rows[this.rowindex],'display','none')      
            this.notificationService.smallBox({
              title: "Deletion",
              content: "<i class='fa fa-clock-o'></i> <i>Site Deleted</i>",
              color: "#659265",
              iconSmall: "fa fa-check fa-2x fadeInRight animated",
              timeout: 4000
              // function:this.delete(SiteCategory)
            });
          }
          else {
            this.notificationService.smallBox({
              title: "Cancelation",
              content: "<i class='fa fa-clock-o'></i> <i>Site already used so you could not delete this Site</i>",
              color: "#C46A69",
              iconSmall: "fa fa-times fa-2x fadeInRight animated",
              timeout: 4000
            });
          }
        })
      }
      if (ButtonPressed === "No") {
        this.notificationService.smallBox({
          title: "Cancelation",
          content: "<i class='fa fa-clock-o'></i> <i>Deletion Cancelled</i>",
          color: "#C46A69",
          iconSmall: "fa fa-times fa-2x fadeInRight animated",
          timeout: 4000
        });
      }
    });
  }
  ngOnInit() {
    this.regionForm = this._fb.group({
      RegionID: 0,
      regionName: ['', Validators.compose([Validators.required, Validators.maxLength(64)])],
      countryId: [{ value: this.CountryId, disabled: this.disbool }, [Validators.required]],
    })

    this._APIwithActionService.getDataList('Site', 'Getcountrylist').subscribe((data) => {
      this.countryList = data;
      // let div = document.querySelector(".Sitelist")
      if (this.itemID > 0) {
        // this._render.setElementStyle(div, 'display', 'block')
        this.submitBtnName = "Update Region";
        this._APIwithActionService.getDatabyID(this.itemID, 'Site', 'GetSitebyReg').subscribe((data) => {
          this.SiteList = data
        }), err => {
          console.log(err);
        }
        this._GlobalAPIService.getDatabyID(this.itemID, 'Region').subscribe((resp) => {
          this.regionForm.setValue({
            RegionID: resp["regionID"],
            regionName: resp["regionName"],
            countryId: resp["countryId"]
            // formControlName2: myValue2 (can be omitted)
          });
          if (localStorage.getItem("role") != "admin") {
            this.regionForm.get('countryId').disable();
          }
        }, error => this.errorMessage = error);
      } else {
        // this._render.setElementStyle(div, 'display', 'none')
      }
    })
    //  this.getSiteCategories();
  }
  editSite(siteid) {
    this.buttonstatus = false;
    this.Selectedsiteid = siteid;

  }
  editsitedetail() {
    if (this.Selectedsiteid != 0) {
      this._router.navigate(["/Managedata/SiteAdd", this.Selectedsiteid])
    }
  }
  save() {
    if (!this.regionForm.valid) {
      return;
    }
    if (!this.itemID) {
      this._GlobalAPIService.postAPI(this.regionForm.getRawValue(), 'Region').subscribe((data) => {
        if (data["_body"] == "Success") {
          this._GlobalAPIService.SuccessMessage("Region Saved Successfully");
          this.event.emit(this.regionForm.value);
          this.bsModalRef.hide();
        } else {
          this._GlobalAPIService.FailureMessage("Region already exists");
        }
      }, error => this.errorMessage = error)
    } else {
      this._GlobalAPIService.putAPI(this.itemID, this.regionForm.getRawValue(), 'Region').subscribe((data) => {
        if (data["_body"] == "Success") {
          this._GlobalAPIService.SuccessMessage("Region Updated Successfully");
          this.event.emit(this.regionForm.value);
          this.bsModalRef.hide();
        } else {
          this._GlobalAPIService.FailureMessage("Region already exists");
        }
      }, error => this.errorMessage = error)
    }
  }

  onCloseModal() {
    this.bsModalRef.hide();
  }
}  