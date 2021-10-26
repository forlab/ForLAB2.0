import { Component, OnInit, EventEmitter } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap';
import { FormBuilder, FormGroup, Validators, FormArray } from '@angular/forms';
import * as Highcharts from 'highcharts';

import { GlobalAPIService } from '../../shared/GlobalAPI.service';
import { APIwithActionService } from '../../shared/APIwithAction.service';

@Component({
  selector: 'app-forecast-morbidity-group',
  templateUrl: './ForecastMorbidityGroup.component.html',
  styleUrls: ['ForecastMorbidityGroup.component.css']
})

export class ForecastMorbidityGroupComponent implements OnInit {
  public event: EventEmitter<any> = new EventEmitter();
  forecastId: number;
  programId: number;
  forecasttype: string;
  patientgrouparr = new Array();
  totalpercentage: number = 0;
  oldtoltalpercentage: number = 0;
  totaltargetpatient: number = 0;
  groupForm: FormGroup;
  colorList = ["#E53560", "#9229E2", "#EFBF20", "#1BB192", "#052644", "#00BAF1", "#EA1455", "#00B08E"];
  loading = true;

  constructor(private _fb: FormBuilder, private _APIwithActionService: APIwithActionService, public bsModalRef: BsModalRef, private _GlobalAPIService: GlobalAPIService) { }

  ngOnInit() {

    if (this.forecastId > 0) {
      // this._APIwithActionService.getDatabyID(this.forecastId, "Forecsatinfo", "GetbyId").subscribe((resp) => {
      //   this.programId = resp["programId"];
        this._APIwithActionService.getDatabyID(this.forecastId, "Forecsatinfo", "Getpatientgroupbydemoforecastid").subscribe((data) => {
          this.loading = false;
          this.programId = data[0].programid
          this.patientgrouparr = new Array();
          for (let idx = 0; idx < data.length; idx++) {
            this.patientgrouparr.push({
              name: data[idx].patientGroupName,
              y: parseInt(data[idx].patientPercentage),
              color: this.colorList[idx]
            });

            this.oldtoltalpercentage = this.oldtoltalpercentage + parseFloat(data[idx].patientPercentage);
            this.addsitecategory();
            (<FormGroup>((<FormArray>(this.groupForm.controls["_patientgroup"])).controls[idx])).patchValue({
              ID: data[idx].id,
              ForecastInfoID: data[idx].forecastinfoID,
              PatientGroupName: data[idx].patientGroupName,
              PatientPercentage: data[idx].patientPercentage,
              PatientRatio: data[idx].patientRatio,
              GroupID: data[idx].groupID,
            });
          }

          this.onGroupStatusCharts();
        });

        this._APIwithActionService.getDatabyID(this.forecastId, "Forecsatinfo", "Gettotaltargetpatient", "programid=" + this.programId).subscribe((data) => {
          this.totaltargetpatient = data;
        });
     // });
    }

    this.groupForm = this._fb.group({
      _patientgroup: this._fb.array([]),
    });
    this.groupForm.controls._patientgroup.valueChanges.subscribe(
      (change) => {
        const calculateAmount = (patientgroup: any[]): number => {
          return patientgroup.reduce((acc, current) => {
            this.totalpercentage = acc + parseFloat(current.PatientPercentage || 0);
            return acc + parseFloat(current.PatientPercentage || 0);
          }, 0);
        };
        calculateAmount(this.groupForm.controls._patientgroup.value);
        let patientnumber = <FormArray>(
          this.groupForm.controls["_patientgroup"]
        );
        this.patientgrouparr = new Array();
        for (let idx = 0; idx < patientnumber.getRawValue().length; idx++) {
          this.patientgrouparr.push({
            name: patientnumber.getRawValue()[idx].PatientGroupName,
            y: parseInt(patientnumber.getRawValue()[idx].PatientPercentage),
            color: this.colorList[idx]
          });
        }
        this.onGroupStatusCharts();
      }
    );
  }

  calculatepatientratio(searchValue: any, index: any) {
    if (searchValue <= 100) {
      (<FormGroup>((<FormArray>this.groupForm.controls["_patientgroup"]).controls[index])).patchValue({
        PatientRatio: (parseFloat(searchValue) * this.totaltargetpatient) / 100,
      });
    } else {
      this._GlobalAPIService.FailureMessage("Percentage Should not be greater than 100");
      (<FormGroup>((<FormArray>this.groupForm.controls["_patientgroup"]).controls[index])).patchValue({
        PatientRatio: 0,
        PatientPercentage: 0,
      });
    }
  }

  initsitecategory() {
    let patientgp: FormGroup = this._fb.group({
      ID: 0,
      GroupID: 0,
      ForecastInfoID: 0,
      PatientGroupName: [{ value: "", disabled: true }],
      PatientPercentage: 0,
      PatientRatio: [{ value: 0, disabled: true }],
    });
    return patientgp;
  }
  addsitecategory() {
    (<FormArray>this.groupForm.controls["_patientgroup"]).push(
      this.initsitecategory()
    );
  }

  onGroupStatusCharts() {
    Highcharts.chart('group-status', {
      chart: {
        plotBackgroundColor: null,
        plotBorderWidth: null,
        type: 'pie',
        margin: 0,
        events: {
          render: function (e) {
            if (document.getElementById("total_pie")) {
              document.getElementById("total_pie").parentElement.remove();
            }
            var text = this.renderer.html('<h1 id="total_pie" style="font-weight:bold; letter-spacing: 0.1em; text-align: center;">' + this.series[0].yData.length + '</h1><span style="letter-spacing: 0.1em;">Groups</span>').add();
            var textBBox = text.getBBox();
            var x = this.plotLeft + (this.plotWidth * 0.5) - (textBBox.width * 0.5);
            var y = this.plotTop + (this.plotHeight * 0.5) - (textBBox.height * 0.5);
            text.attr({ x: x, y: y + 6 });
          },
        }
      },
      legend: false,
      credits: {
        enabled: false
      },
      title: {
        text: '',
        style: {
          fontSize: '14px',
          fontWeight: 'bold'
        },
        verticalAlign: 'bottom'
      },
      tooltip: {
        pointFormat: '{series.name}: <b>{point.y}</b>'
      },
      plotOptions: {
        pie: {
          allowPointSelect: true,
          cursor: 'pointer',
          size: '100%',
          dataLabels: {
            useHTML: true,
            enabled: true,
            formatter: function () {
              return '<span style="color:' + this.point.color + '; font-size: 12px;">' + this.point.name + ':' + this.point.y + '%' + '</span>';
            }
          },
          showInLegend: true
        }
      },
      series: [{
        colorByPoint: true,
        data: this.patientgrouparr,
        innerSize: '50%',
        size: 180
      }]
    });
  }
  delproductprice(index) {
    (<FormArray>(this.groupForm.controls["_patientgroup"])).removeAt(index);
  }

  onCreateGroup(groupName: string) {
    if (groupName) {
      let patientnumber = <FormArray>(
        this.groupForm.controls["_patientgroup"]
      );
      for (let idx = 0; idx < patientnumber.getRawValue().length; idx++) {
        if (patientnumber.getRawValue()[idx].PatientGroupName == groupName) {
          this._GlobalAPIService.FailureMessage("Group Name is duplicated")
          return;
        }
      }
      this.addsitecategory();
      var lastIdx = patientnumber.getRawValue().length - 1;
      (<FormGroup>((<FormArray>(this.groupForm.controls["_patientgroup"])).controls[lastIdx])).patchValue({
        ID: 0,
        ForecastInfoID: this.forecastId,
        PatientGroupName: groupName,
        PatientPercentage: 0,
        PatientRatio: 0,
        GroupID: 0,
      });
      let patientGroup = new Array();
      var newgroup = {
        Id: 0,
        groupName: groupName,
        programId: this.programId,
        forecastid: this.forecastId,
        isActive: true,
        type: "",
      };
      patientGroup.push(newgroup);
      this._APIwithActionService.postAPI(patientGroup, "MMProgram", "saveforecastmmgroup").subscribe((data) => {
        this.ngOnInit();
      })

    }
  }


  onCloseModal() {
    this.bsModalRef.hide();
  }

  openNextModal() {
    let patientgpnumber = <FormArray>(this.groupForm.controls["_patientgroup"]);
    let patientgroupusage = new Array();
    patientgpnumber.getRawValue().forEach((x) => {
      patientgroupusage.push(x);
    });
    if (this.totalpercentage > 100 || this.totalpercentage < 100) {
      this._GlobalAPIService.FailureMessage("Ratio of Group should be equal to 100");
      return;
    } else {
      this._APIwithActionService.postAPI(patientgroupusage, "Forecsatinfo", "savepatientgroup").subscribe((data) => {
        if (data["_body"] != "0") {
          if (this.oldtoltalpercentage == 0) {
            this._GlobalAPIService.SuccessMessage("Patient Group Saved Successfully");
          }
          this.bsModalRef.hide();
          this.event.emit({ type: "next" });
        }
      });
    }
  }

  openPreviousModal() {
    this.bsModalRef.hide();
    this.event.emit({ type: "back" });
  }

}

