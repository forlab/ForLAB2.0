import { Component, OnInit, EventEmitter, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { BsModalRef, BsModalService, ModalDirective } from 'ngx-bootstrap';
import { FormBuilder, FormGroup, Validators, FormArray } from '@angular/forms';

import { GlobalAPIService } from '../../shared/GlobalAPI.service';
import { APIwithActionService } from '../../shared/APIwithAction.service';

@Component({
    selector: 'app-forecast-morbidity-test',
    templateUrl: './ForecastMorbidityTest.component.html',
    styleUrls: ['ForecastMorbidityTest.component.css']
})

export class ForecastMorbidityTestComponent implements OnInit {
    public event: EventEmitter<any> = new EventEmitter();
    forecastId: number;
    programId: number;
    demographictestingprotocolmodel: FormGroup;
    testingprotocolvalue = new Array();
  selectedtestid1: number = 0;
  selectedpatientgroupid: number = 0;
  parameterlistvar = new Array();
    testList = new Array();
    selectedTest: any;
    groupList = new Array();
    selectedGroupID = 0;
    HeaderArray = new Array();
    demographictestingprotocol: FormGroup;
    parameterlist = new Array();
    controlArray = new Array();
    testingprotocollist=new Array();
    // HeaderArray = new Array();
    // controlArray = new Array();
    selectedHearderBtn = 0;
    loading = true;
    @ViewChild('lgModal4') public lgModal4: ModalDirective;
    constructor(private _fb: FormBuilder, private _APIwithActionService: APIwithActionService, public bsModalRef: BsModalRef, private _GlobalAPIService: GlobalAPIService) { }

    ngOnInit() {
        this.demographictestingprotocol = this._fb.group({
            _testingprotocol: this._fb.array([]),
        });
        this.demographictestingprotocolmodel = this._fb.group({
            _testingprotocolVariable: this._fb.array([]),
      
      
          })
        this._APIwithActionService.getDatabyID(this.forecastId, "Test", "getAlltestbytestingarea").subscribe((data) => {
            for (var idx = 0; idx < data.length; idx++) {
                if (data[idx].tests.length) {
                    for (var idx_test = 0; idx_test < data[idx].tests.length; idx_test++) {
                        this.testList.push(data[idx].tests[idx_test]);
                    }
                }
            }
        });
        this._APIwithActionService.getDatabyID(this.forecastId, "MMProgram", "Getpatientgroupforforecast", "id=" + this.programId).subscribe((data) => {
            for (var idx = 0; idx < data.length; idx++) {
                this.groupList.push({
                    groupID: data[idx].id,
                    patientGroupName: data[idx].groupName
                })
            }
            this.selectedGroupID = this.groupList[0].groupID;
        });

        this._APIwithActionService.getDatabyID(this.forecastId, "Assumption", "GettestforecastAssumptionnew").subscribe((data11) => {
            this._APIwithActionService.getDatabyID(this.forecastId, "Assumption", "GetforecastDynamiccontrol", "entitytype=5").subscribe((data) => {
                this._APIwithActionService.getDatabyID(this.forecastId, "Assumption", "getdynamicheader", "entitytype=5").subscribe((data2) => {
                    this.controlArray = data;
                    this.HeaderArray = data2;
            this.loading = false;
            this.parameterlist = data11;
            let ss = <FormArray>(
                this.demographictestingprotocol.controls["_testingprotocol"]
            );
            ss.controls = [];
            for (let idx = 0; idx < this.parameterlist.length; idx++) {
                ss.push(this._fb.group(this.parameterlist[idx]));
            }
            console.log('this.demographictestingprotocol', this.demographictestingprotocol);
                });
            });
        });


    }
    edit(testID: FormGroup, index: number) {
        let ss = <FormArray>this.demographictestingprotocolmodel.controls["_testingprotocolVariable"];
        ss.controls = [];
        this._APIwithActionService.getDatabyID(this.forecastId, 'Assumption', 'GettestforecastAssumptionnewbytestId', 'param=' + testID.controls["testID"].value + "," + testID.controls["patientGroupID"].value).subscribe((data2) => {
          this.parameterlistvar = data2;
          console.log(this.parameterlistvar)
          ss.controls = [];
          for (let index = 0; index < this.parameterlistvar.length; index++) {
            ss.push(this._fb.group(this.parameterlistvar[index]))
    
    
          }
        })
    
        this.selectedpatientgroupid = testID.controls["patientGroupID"].value;
        this.selectedtestid1 = testID.controls["testID"].value;
        console.log(ss);
        this.lgModal4.show();
    
    
      }
    getTestList(testID: number) {
        if (testID > 0) {
            this.selectedTest = this.testList.find(x => { return x.testID === +testID });
        }
    }
    handleSelecteHeaderBtn(index) {
        this.selectedHearderBtn = index;
    }
    handleForecastTestType(index) {
        this.selectedGroupID = index;
    }
    deltest(testID: FormGroup, index: number) {
        this._APIwithActionService.deleteData(testID.controls["testID"].value, "Assumption", "deletetestingprotocol", "param=" + this.forecastId + "," + testID.controls["patientGroupID"].value).subscribe((data) => {
            (<FormArray>(this.demographictestingprotocol.controls["_testingprotocol"])).removeAt(index);
        });
    }
    addgeneralassumption() {

        let ss = <FormArray>this.demographictestingprotocolmodel.controls["_testingprotocolVariable"];
        console.log(ss)
        ss.getRawValue().forEach(x => {
    
    
          let index = this.testingprotocolvalue.findIndex(z => z.Parameterid == x.id && z.TestID == this.selectedtestid1 && z.PatientGroupID == this.selectedpatientgroupid)
          if (index < 0) {
            this.testingprotocolvalue.push({
    
              Parameterid: x.id,
              Parametervalue: parseFloat(x.value),
              Forecastid: this.forecastId,
              TestID: this.selectedtestid1,
              PatientGroupID: this.selectedpatientgroupid,
             // UserId: this.UserId
            })
          }
          else {
            this.testingprotocolvalue[index].Parametervalue = parseFloat(x.value);
          }
        })
    
    
        if (this.testingprotocolvalue.length > 0) {
          this._APIwithActionService.postAPI(this.testingprotocolvalue, 'Assumption', 'savetestinggeneralassumptionvalue').subscribe((data) => {
    
            if (data["_body"] != 0) {
              // this._GlobalAPIService.SuccessMessage("Testing Protocol Saved Successfully");
            }
    
          }
          )
        }
        this.lgModal4.hide();
        ss.controls=[];
        console.log(this.testingprotocolvalue)
      }
      inputvalue(args, i, datatype) {
        let name = args.target.name;
    
        if (name == "percentagePanel") {
          if (args.target.value > 100) {
            this._GlobalAPIService.FailureMessage("Percentage should not be greater than 100");
            (<FormArray>(this.demographictestingprotocol.controls["_testingprotocol"])).controls[i].patchValue({
              percentagePanel: 0
            })
          }
        }
        else if (datatype == 2) {
          if (args.target.value > 100) {
            this._GlobalAPIService.FailureMessage("Percentage should not be greater than 100");
            (<FormArray>(this.demographictestingprotocol.controls["_testingprotocol"])).controls[i].patchValue({
              [name]: 0
            })
          }
    
    
    
        }
        //  alert(name);
    
      }
    addtestdata() {
        let newarray = (<FormArray>this.demographictestingprotocol.controls["_testingprotocol"]).controls;
        let ss = <FormArray>this.demographictestingprotocol.controls["_testingprotocol"];
        let isexist: boolean = false;
        console.log(ss);
        for (let index = 0; index < newarray.length; index++) {
          if ((<FormGroup>newarray[index]).controls.testID.value == this.selectedTest) {
            isexist = true
            break;
          }
          else {
            isexist = false
          }
    
        }
        if (isexist == false) {
          this.savedata();
          this._APIwithActionService.getDatabyID(this.forecastId, 'Assumption', 'GettestforecastAssumption', 'testid=' + this.selectedTest
          ).subscribe((data11) => {
    
    
            this._APIwithActionService.getDatabyID(this.forecastId, 'Assumption', 'GetforecastDynamiccontrol', 'entitytype=5').subscribe((data) => {
              this.controlArray = data;
              console.log(this.controlArray)
              this._APIwithActionService.getDatabyID(this.forecastId, 'Assumption', 'getdynamicheader', 'entitytype=5').subscribe((data2) => {
                this.HeaderArray = data2
    
                this.parameterlist = data11;
                console.log(this.parameterlist)
               // ss.controls = [];
                for (let index = 0; index < this.parameterlist.length; index++) {
                  ss.push(this._fb.group(this.parameterlist[index]))
    
    
                }
              })
            })
    
          })
    
    
          // for (let index = 0; index < ss.length; index++) {
          //   ss.controls[index].setValue({
          //     testID: data.testid
    
          //   })
    
          // }
        }
    
      }
    onCreateTest() {
        if (this.selectedTest) {
            this.addTestcategory();
            let patientnumber = <FormArray>(
                this.demographictestingprotocol.controls["_testingprotocol"]
            );
            var lastIdx = patientnumber.getRawValue().length - 1;
            (<FormGroup>((<FormArray>(this.demographictestingprotocol.controls["_testingprotocol"])).controls[lastIdx])).patchValue({
                id: 0,
                testID: this.selectedTest.testID,
                testName: this.selectedTest.testName,
                patientGroupID: this.selectedGroupID,
                forecastinfoID: 0,
                percentagePanel: 0,
                totalTestPerYear: 0,
                baseline: 0,
                UserId: 0
            });

        }

    }

    initTestcategory() {
        let patientgp: FormGroup = this._fb.group({
            id: 0,
            testID: 0,
            testName: "",
            patientGroupID: 0,
            forecastinfoID: 0,
            percentagePanel: 0,
            totalTestPerYear: 0,
            baseline: 0,
            UserId: 0
        });
        return patientgp;
    }
    addTestcategory() {
        (<FormArray>this.demographictestingprotocol.controls["_testingprotocol"]).push(
            this.initTestcategory()
        );
    }

    onCloseModal() {
        this.bsModalRef.hide();
    }

    openNextModal() {


        let ss = <FormArray>this.demographictestingprotocol.controls["_testingprotocol"];


        this.testingprotocollist.splice(0, this.testingprotocollist.length);
        this.testingprotocolvalue.splice(0, this.testingprotocolvalue.length);
        ss.getRawValue().forEach(x => {
          if (x.testID > 0) {
            this.testingprotocollist.push({
              ID: x.id,
              TestID: x.testID,
              PatientGroupID: x.patientGroupID,
              ForecastinfoID: x.forecastinfoID,
              PercentagePanel: x.percentagePanel,
              TotalTestPerYear: x.totalTestPerYear,
              Baseline:x.baseline,
            //  UserId: this.UserId
    
            })
            for (let index = 0; index < this.controlArray.length; index++) {
              if (this.controlArray[index].type == "number" && this.controlArray[index].id != 0) {
                this.testingprotocolvalue.push({
    
                  Parameterid: this.controlArray[index].id,
                  Parametervalue: x[this.controlArray[index].name],
                  Forecastid: x.forecastinfoID,
                  TestID: x.testID,
                  PatientGroupID: x.patientGroupID,
               //   UserId: this.UserId
                })
    
              }
    
            }
    
          }
          else {
            return;
          }
        });
        if (this.testingprotocollist.length > 0) {
          this._APIwithActionService.postAPI(this.testingprotocollist, "Assumption", "Savetestingprotocol").subscribe((data) => {
    
            this._APIwithActionService.postAPI(this.testingprotocolvalue, 'Assumption', 'savetestinggeneralassumptionvalue').subscribe((data) => {
    
              if (data["_body"] != 0) {
                this.bsModalRef.hide();
                this.event.emit({ type: "next" });
               // this._GlobalAPIService.SuccessMessage("Testing Protocol Saved Successfully");
              }
    
            }
            )
          })
        }
        else {
            this.bsModalRef.hide();
            this.event.emit({ type: "next" });
        }
        // let ss = <FormArray>(
        //     this.demographictestingprotocol.controls["_testingprotocol"]
        // );
        // var testingprotocollist = new Array();
        // ss.getRawValue().forEach((x) => {
        //     if (x.testID > 0) {
        //         testingprotocollist.push({
        //             ID: x.id,
        //             TestID: x.testID,
        //             PatientGroupID: x.patientGroupID,
        //             ForecastinfoID: x.forecastinfoID,
        //             PercentagePanel: x.percentagePanel,
        //             TotalTestPerYear: x.totalTestPerYear,
        //             Baseline: x.baseline,
        //             UserId: 0,
        //         });

        //     } else {
        //         return;
        //     }
        // });
        // if (testingprotocollist.length > 0) {
        //     this._APIwithActionService.postAPI(testingprotocollist, "Assumption", "Savetestingprotocol").subscribe((data) => {
        //         this.bsModalRef.hide();
        //         this.event.emit({ type: "next" });
        //     });
        // } else {
        //     this.bsModalRef.hide();
        //     this.event.emit({ type: "next" });
        // }
    }

    openPreviousModal() {
        this.bsModalRef.hide();
        this.event.emit({ type: "back" });
    }
    savedata() {
        let ss = <FormArray>this.demographictestingprotocol.controls["_testingprotocol"];
    
    
        this.testingprotocollist.splice(0, this.testingprotocollist.length);
        //  this.testingprotocolvalue.splice(0, this.testingprotocolvalue.length);
        ss.getRawValue().forEach(x => {
          if (x.testID > 0) {
            this.testingprotocollist.push({
              ID: x.id,
              TestID: x.testID,
              PatientGroupID: x.patientGroupID,
              ForecastinfoID: x.forecastinfoID,
              PercentagePanel: x.percentagePanel,
              TotalTestPerYear: x.totalTestPerYear,
              Baseline: x.baseline,
            //  UserId: this.UserId
    
            })
            // for (let index = 0; index < this.controlArray.length; index++) {
            //   if (this.controlArray[index].type == "number" && this.controlArray[index].id != 0) {
            //     this.testingprotocolvalue.push({
    
            //       Parameterid: this.controlArray[index].id,
            //       Parametervalue: x[this.controlArray[index].name],
            //       Forecastid: x.forecastinfoID,
            //       TestID: x.testID,
            //       PatientGroupID: x.patientGroupID,
            //       UserId: this.UserId
            //     })
    
            //   }
    
            // }
    
          }
          else {
            return;
          }
        });
        if (this.testingprotocollist.length > 0) {
          this._APIwithActionService.postAPI(this.testingprotocollist, "Assumption", "Savetestingprotocol").subscribe((data) => {
            // if (this.testingprotocolvalue.length > 0) {
            //   this._APIwithActionService.postAPI(this.testingprotocolvalue, 'Assumption', 'savetestinggeneralassumptionvalue').subscribe((data) => {
    
            //     if (data["_body"] != 0) {
            //       // this._GlobalAPIService.SuccessMessage("Testing Protocol Saved Successfully");
            //     }
    
            //   }
            //   )
            // }
          })
        }
      }
}

