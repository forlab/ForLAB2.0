import { Component, OnInit, EventEmitter, Output } from "@angular/core";

import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { Router, ActivatedRoute } from "@angular/router";
import { APIwithActionService } from "../../shared/APIwithAction.service";
import { GlobalAPIService } from "../../shared/GlobalAPI.service";
import { NotificationService } from "../../shared/utils/notification.service";
import { BsModalRef, BsModalService } from "ngx-bootstrap";
import { Tree } from "@angular/router/src/utils/tree";
import { NumberValueAccessor } from "@angular/forms/src/directives";
declare var $: any;

@Component({
  selector: "app-programadd",
  templateUrl: "./ProgramAdd.component.html",
  styleUrls: ["ProgramAdd.component.css"],
})
export class ProgramAddComponent implements OnInit {
  @Output() close = new EventEmitter();
  constructor(
    public bsModalRef: BsModalRef,
    private _fb: FormBuilder,
    private _avRoute: ActivatedRoute,
    private notificationService: NotificationService,
    private _GlobalAPIService: GlobalAPIService,
    private _router: Router,
    private _APIwithActionService: APIwithActionService
  ) {}
  public event: EventEmitter<any> = new EventEmitter();
  suggestionAreaList: any[];
  forecastMethodList: any[];
  variableTargetList: any[];
  variablePopulationList: any[];
  testingYearList: any[];
  patientVarList: any[];
  variableTypeList: any[];
  variableEffectList: any[];
  idxsuggestionArea: any;
  idxforecastMethod: any;
  targetDisabled: boolean;
  populationDisabled: boolean;
  SuggustedvariableDisabled: boolean;
  ForecastmethodDisabled: boolean;
  idxvariableType: any;
  idxvariableEffect: any;
  idxVariableTarget: any;
  idxVariablePopulation: any;
  idxtestingYear: any;
  parametername = new Array();
  parameterobject = new Object();
  demosettingform: FormGroup;
  suggestedvariable = new Object();
  selectedprogramid: number = 0;
  programname: string;
  generalassumptionlist = new Array();
  patientassumption = new Array();
  testingassumption = new Array();
  productassumption = new Array();
  patientgrouplist = new Array();
  patientgroup = new Array();
  totalforecast: number;
  MMForecastParameterList = new Array();
  MMforcastparameter = new Array();
  ProgramList = new Array();
  itemid: any;
  hidtitle: boolean = false;
  ngAfterViewChecked() {}
  getprogramparameter() {
    this._APIwithActionService.getDataList("MMProgram", "Getforecastparameter").subscribe((resp) => {
      this.MMForecastParameterList = resp;
      console.log(resp);
    });
  }
  //   getprogramlist() {
  //     this._APIwithActionService.getDatabyID('MMProgram', 'Get').subscribe((resp) => {
  //         this.ProgramList = resp;
  //     })
  // }
  onCloseModal() {
    this.bsModalRef.hide();
  }
  ngOnInit() {
    this.selectedprogramid = this.itemid;
    console.log(this.itemid);
    console.log(this.totalforecast);
    if (this.totalforecast > 0) {
      this.SuggustedvariableDisabled = true;
      this.ForecastmethodDisabled = true;
    } else {
      this.SuggustedvariableDisabled = false;
      this.ForecastmethodDisabled = false;
    }
    this.getprogramparameter();
    this.idxsuggestionArea = 2;
    this.idxforecastMethod = 0;
    this.idxvariableType = 0;
    this.idxtestingYear = 0;
    this.targetDisabled = true;
    this.populationDisabled = false;
    this.hidtitle = false;
    // this.forecastMethodList = [{ methodName: "Based on historical patient data" }, { methodName: "Program Target" }, { methodName: "From the general population" }];
    this.forecastMethodList = [{ methodName: "Based on historical patient data" }, { methodName: "Program Target" }];
    this.variableTargetList = [{ Name: "CurrentPatient" }, { Name: "TargetPatient" }];
    this.variablePopulationList = [{ Name: "Population Number" }, { Name: "Prevalence Rate" }];
    this.variableTypeList = [{ name: "Numeric" }, { name: "Percentage" }];
    this.variableEffectList = [{ name: "Positive" }, { name: "Negative" }];
    this.testingYearList = [{ name: "1 Year" }, { name: "2 Year" }];
    this.patientVarList = [
      { name: "variable1" },
      { name: "variable2" },
      { name: "variable3" },
      { name: "variable4" },
      { name: "variable5" },
      { name: "variable6" },
      { name: "variable7" },
      { name: "variable8" },
      { name: "variable9" },
      { name: "variable10" },
    ];
    this.demosettingform = this._fb.group({
      id: 0,
      Programname: ["", Validators.compose([Validators.required, Validators.maxLength(64)])],
      groupName: ["", Validators.compose([Validators.required, Validators.maxLength(200)])],
      Isactivegroup: [true],
      variablenamepatient: ["", Validators.compose([Validators.required, Validators.maxLength(200)])],
      variabletypepatient: ["1", [Validators.required]],
      patienteffect: ["Positive"],
      Isactivepatient: [true],
      variablenametesting: ["", Validators.compose([Validators.required, Validators.maxLength(200)])],
      variabletypetesting: ["1", [Validators.required]],
      testingeffect: ["Positive"],
      Isactivetesting: [true],
      variablenameproduct: ["", Validators.compose([Validators.required, Validators.maxLength(200)])],
      variabletypeproduct: ["1", [Validators.required]],
      producteffect: ["Positive"],
      Isactiveproduct: [true],
      Years: ["", [Validators.required]],
    });
    if (this.selectedprogramid > 0 || this.selectedprogramid != undefined) {
      this._APIwithActionService.getDatabyID(this.selectedprogramid, "MMProgram", "Get").subscribe((resp) => {
        this.demosettingform.patchValue({
          id: resp.Id,
          Programname: resp.programName,
        });
        this.ProgramList = resp;
      });
      this._APIwithActionService.getDatabyID(this.selectedprogramid, "MMProgram", "Getforecastparameterbyprogramid").subscribe((resp) => {
        console.log(resp);
        if (resp.length > 0) {
          if (resp[0].forecastMethod == 1) {
            this.idxforecastMethod = 1;
            this.targetDisabled = false;
            this.populationDisabled = true;
            this.hidtitle = false;
          } else if (resp[0].forecastMethod == 2) {
            this.idxforecastMethod = 2;
            this.targetDisabled = true;
            this.populationDisabled = false;
            this.hidtitle = false;
          } else {
            this.idxforecastMethod = 0;
            this.targetDisabled = true;
            this.populationDisabled = false;
            this.hidtitle = false;
          }
        }
      });
    }

    this._APIwithActionService.getDataList("MMProgram", "Get").subscribe((resp) => {
      this.suggestionAreaList = resp;
    });
  }
  save() {}

  clearctrl() {
    this.close.emit(true);
  }
  nextForm() {
    var activeIndex = parseInt($("ul.progressbar li.active").index()) + 1;
    if (activeIndex > 0) $(".prevForm-btn").removeAttr("disabled");
    if (activeIndex < 6) $(".nextForm-btn").removeAttr("disabled");
    if (activeIndex == 6) $(".nextForm-btn").html("Finish");
    if (activeIndex > 6) this.close.emit(true);
    if (activeIndex == 2) {
      if (this.parametername.length != 0) {
        this._APIwithActionService.postAPI(this.parameterobject, "MMProgram", "Saveforecastparameter").subscribe((data) => {
          this.parameterobject = "";
          this.parametername.splice(0, this.parametername.length);
        });
      }
      this.getpatientgroup();
    }
    if (activeIndex ==3) {
      this.getgeneralassumption();
    }
    $(".step-form").removeClass("active");
    var activeformClass = ".step" + (activeIndex + 1).toString();
    $(activeformClass).addClass("active");
    $("ul.progressbar li")
      .eq(activeIndex - 1)
      .removeClass("active")
      .addClass("passed");
    $("ul.progressbar li").eq(activeIndex).addClass("active");
  }
  prevForm() {
    var activeIndex = parseInt($("ul.progressbar li.active").index()) + 1;
    if (activeIndex == 2) $(".prevForm-btn").attr("disabled", "disabled");
    if (activeIndex > 2) $(".prevForm-btn").removeAttr("disabled");
    if (activeIndex == 6) $(".nextForm-btn").attr("disabled", "disabled");
    if (activeIndex < 6) $(".nextForm-btn").removeAttr("disabled");

    $(".step-form").removeClass("active");
    var activeformClass = ".step" + (activeIndex - 1).toString();
    $(activeformClass).addClass("active");
    $("ul.progressbar li")
      .eq(activeIndex - 1)
      .removeClass("active");
    $("ul.progressbar li")
      .eq(activeIndex - 2)
      .removeClass("passed")
      .addClass("active");
  }
  handlesuggestionArea(index) {
    this.idxsuggestionArea = index;
  }
  handleForecastMethod(index) {
    let postforecastparameter = new Array();
    let methodname = "";
    if (index == 1) {
      this.parametername[0] = "CurrentPatient";
      this.parametername[1] = "TargetPatient";
      methodname = "Target_Based";
    } else if (index == 2) {
      this.parametername[0] = "PopulationNumber";
      this.parametername[1] = "PrevalenceRate";
      methodname = "Population_Based";
    } else {
      this.parametername[0] = "PopulationNumber";
      this.parametername[1] = "PrevalenceRate";
      index = 3;
      methodname = "HistroicalPatient_Based";
    }
    console.log(this.parametername);
    for (let index1 = 0; index1 < this.parametername.length; index1++) {
      postforecastparameter.push({
        id: 0,
        forecastMethod: index,
        forecastMethodname: methodname,
        variableName: this.parametername[index1],
        variableDataType: 1,
        variableDataTypename: "Numeric",
        useOn: "OnAllSite",
        variableFormula: "",
        ProgramId: this.selectedprogramid,
        VarCode: this.parametername[index1][0] + this.selectedprogramid,
        IsPrimaryOutput: false,
        VariableEffect: true,
        isActive: "Yes",
      });
    }
    this.MMforcastparameter.splice(0, this.MMforcastparameter.length);
    this.MMforcastparameter = postforecastparameter;
    postforecastparameter.forEach((x) => {
      this.MMForecastParameterList.push(x);
    });
    this.parameterobject = {
      id: this.selectedprogramid,
      ProgramName: this.programname,
      _mMForecastParameter: this.MMforcastparameter,
    };
    console.log(this.MMForecastParameterList);
    this.idxforecastMethod = index;
    if (index == 1) {
      this.targetDisabled = false;
      this.populationDisabled = true;
      this.hidtitle = false;
    } else if (index == 2) {
      this.targetDisabled = true;
      this.populationDisabled = false;
      this.hidtitle = false;
    } else {
      this.targetDisabled = true;
      this.populationDisabled = true;
      this.hidtitle = true;
    }
  }
  handleVariableType(index) {
    this.idxvariableType = index;
  }
  handleVariableEffect(index) {
    this.idxvariableEffect = index;
  }
  handleVariableTarget(index) {
    this.idxVariableTarget = index;
  }
  handleVariablePopulation(index) {
    this.idxVariablePopulation = index;
  }
  handletestingYear(index) {
    this.idxtestingYear = index;
let year;
if (this.testingYearList[index].name="1 Year")
{
  year=1;
}
else  
{
  year=2;
}
    
    let programobject = new Object();
    programobject = {
        id: this.selectedprogramid,
        ProgramName: this.programname,
        NoofYear: year
    }
    this._APIwithActionService.putAPI(this.selectedprogramid, programobject, 'MMProgram', 'updateProgram').subscribe((data) => {
        if (data['status'] == '200') {
            this._GlobalAPIService.SuccessMessage('Testing Year updated successfully');
        }
        console.log(data)

    })
  }
  getpatientgroup() {
    this._APIwithActionService.getDataList("MMProgram", "Getpatientgroup").subscribe((resp) => {
      this.patientgrouplist = resp;

      if (this.selectedprogramid != 0) {
        this.patientgroup = this.patientgrouplist.filter((x) => x.programId === this.selectedprogramid);
      }
    });
  }
  addnewprogram(programname: string = "") {
    let newprogram = new Object();

    if (programname != "") {
      newprogram = {
        Id: this.demosettingform.controls["id"].value,
        ProgramName: programname,
      };
    } else {
      newprogram = {
        Id: this.demosettingform.controls["id"].value,
        ProgramName: this.demosettingform.controls["Programname"].value,
      };
    }
    this.suggestedvariable = {
      id: 0,
      Name: newprogram["ProgramName"],
      Type_name: "Program",
    };
    this._APIwithActionService.postAPI(newprogram, "MMProgram", "SaveProgram").subscribe((data) => {
      if (data["_body"] != 0) {
        this._GlobalAPIService.SuccessMessage("Program saved successfully");
        this.demosettingform.patchValue({
          Programname: "",
        });
        this.demosettingform.controls["Programname"].markAsUntouched();

        this.demosettingform.patchValue({
          id: data["_body"],
          Programname: newprogram["ProgramName"],
          // Years: this.ProgramList[i].noofYear === 2 ? "2" : "1"
        });
        this.selectedprogramid = Number(this.demosettingform.controls["id"].value);
        this.programname = newprogram["ProgramName"];
        //  this.Settingdisplayproperty("block");

        // this.getprogramlist();
        this.getgeneralassumption();
        //Update Program List
        this._APIwithActionService.getDataList("MMProgram", "Get").subscribe((resp) => {
          this.suggestionAreaList = resp;
        });
      } else {
        this._GlobalAPIService.FailureMessage("Program Name must not be duplicate");
      }
    });
  }
  getgeneralassumption() {
    this._APIwithActionService.getDataList("MMProgram", "GetGeneralAssumptionList").subscribe((resp) => {
      this.generalassumptionlist = resp;
      this.patientassumption = this.generalassumptionlist.filter((x) => x.programId === this.selectedprogramid && x.assumptionType === 1);

      this.testingassumption = this.generalassumptionlist.filter((x) => x.programId === this.selectedprogramid && x.assumptionType === 3);
      this.productassumption = this.generalassumptionlist.filter((x) => x.programId === this.selectedprogramid && x.assumptionType === 2);
    });
  }
  addnewgroup(groupname: string = "") {
    let newgroup = new Object();
    if (groupname != "") {
      newgroup = {
        Id: 0,
        GroupName: groupname,
        ProgramId: this.selectedprogramid,
        IsActive: true,
      };
    } else {
      newgroup = {
        Id: 0,
        GroupName: this.demosettingform.controls["groupName"].value,
        ProgramId: this.selectedprogramid,
        IsActive: true,
      };
    }
    this.suggestedvariable = {
      id: 0,
      Name: newgroup["GroupName"],
      Type_name: "Patient/Population Group",
    };
    this._APIwithActionService.postAPI(newgroup, "MMProgram", "savegroup").subscribe((data) => {
      if (data["_body"] != 0) {
        // this._APIwithActionService.postAPI(this.suggestedvariable, "MMProgram", "savesuggustion").subscribe((data1) => {

        // })
        this._GlobalAPIService.SuccessMessage("Group saved successfully");
        this.demosettingform.patchValue({
          groupName: "",
        });
        this.demosettingform.controls["groupName"].markAsUntouched();
        this.getpatientgroup();
        // if (groupname == "") {
        //     this.lgModal1.show()
        // }
      } else {
        this._GlobalAPIService.FailureMessage("Group Name must not be Duplicate");
      }
    });
  }
  smartModEg2(item: any) {
    let index: number;
    let groupoject = new Object();
    this.notificationService.smartMessageBox(
      {
        title: "Operation",
        content: "For Activation/Deactivation " + item.groupName + " Press Yes For no operation Press No",
        buttons: "[No][Yes]",
      },
      (ButtonPressed) => {
        if (ButtonPressed === "Yes") {
          index = this.patientgroup.findIndex((x) => x.id === item.id);
          if (this.patientgroup[index].isActive == "Yes") {
            this.patientgroup[index].isActive = "No";
          } else {
            this.patientgroup[index].isActive = "Yes";
          }
          groupoject = {
            Id: item.id,
            GroupName: item.groupName,
            ProgramId: item.programId,
            IsActive: this.patientgroup[index].isActive,
          };
          this._APIwithActionService.putAPI(item.id, groupoject, "MMProgram", "updategroup").subscribe((data) => {});
        }
        if (ButtonPressed === "No") {
        }
      }
    );
  }
  smartModEg1(item: any, Type: string) {
    let variablename: string;
    let index: number;
    let groupoject = new Object();
    let parameterobject = new Object();
    if (Type == "Group") {
      variablename = item.groupName + " Group";
    } else {
      variablename = item.variableName + " Variable";
    }
    this.notificationService.smartMessageBox(
      {
        title: "Operation",
        content: "For Activation/Deactivation " + variablename + " Press Yes For no operation Press No",
        buttons: "[No][Yes]",
      },
      (ButtonPressed) => {
        if (ButtonPressed === "Yes") {
          if (Type === "Patient") {
            index = this.patientassumption.findIndex((x) => x.id === item.id);
            if (this.patientassumption[index].isActive == "Yes") {
              this.patientassumption[index].isActive = "No";
            } else {
              this.patientassumption[index].isActive = "Yes";
            }
            parameterobject = {
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
              IsActive: item.isActive,
            };
            this._APIwithActionService.putAPI(item.id, parameterobject, "MMProgram", "updategeneralassumptions").subscribe((data) => {});
          } else if (Type === "Test") {
            index = this.testingassumption.findIndex((x) => x.id === item.id);
            if (this.testingassumption[index].isActive == "Yes") {
              this.testingassumption[index].isActive = "No";
            } else {
              this.testingassumption[index].isActive = "Yes";
            }
            parameterobject = {
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
              IsActive: item.isActive,
            };
            this._APIwithActionService.putAPI(item.id, parameterobject, "MMProgram", "updategeneralassumptions").subscribe((data) => {});
          } else {
            index = this.productassumption.findIndex((x) => x.id === item.id);
            if (this.productassumption[index].isActive == "Yes") {
              this.productassumption[index].isActive = "No";
            } else {
              this.productassumption[index].isActive = "Yes";
            }
            parameterobject = {
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
              IsActive: item.isActive,
            };
            this._APIwithActionService.putAPI(item.id, parameterobject, "MMProgram", "updategeneralassumptions").subscribe((data) => {});
          }
        }
        if (ButtonPressed === "No") {
        }
      }
    );
  }
  addAssumption(Assumtiontype: number, variablename: string = "") {
    let postassumption = new Object();
    if (Assumtiontype == 1) {
      if (variablename != "") {
        postassumption = {
          Id: 0,
          VariableName: variablename,
          VariableDataType: this.demosettingform.controls["variabletypepatient"].value,
          VariableDataTypeName: this.demosettingform.controls["variabletypepatient"].value == 1 ? "Numeric" : "Percentage",
          UseOn: "OnAllSite",
          ProgramId: this.selectedprogramid,
          VarCode: "xx",
          AssumptionType: 1,
          AssumptionTypename: "Patient_Number_Assumption",
          VariableEffect: this.demosettingform.controls["patienteffect"].value == "Positive" ? true : false,
          IsActive: this.demosettingform.controls["Isactivepatient"].value == true ? "Yes" : "No",
        };
      } else {
        postassumption = {
          Id: 0,
          VariableName: this.demosettingform.controls["variablenamepatient"].value,
          VariableDataType: this.demosettingform.controls["variabletypepatient"].value,
          VariableDataTypeName: this.demosettingform.controls["variabletypepatient"].value == 1 ? "Numeric" : "Percentage",
          UseOn: "OnAllSite",
          ProgramId: this.selectedprogramid,
          VarCode: "xx",
          AssumptionType: 1,
          AssumptionTypename: "Patient_Number_Assumption",
          VariableEffect: this.demosettingform.controls["patienteffect"].value == "Positive" ? true : false,
          IsActive: this.demosettingform.controls["Isactivepatient"].value == true ? "Yes" : "No",
        };
      }

      this.suggestedvariable = {
        id: 0,
        Name: postassumption["VariableName"],
        Type_name: "Patient Assumption",
      };
    } else if (Assumtiontype == 3) {
      if (variablename != "") {
        postassumption = {
          Id: 0,
          VariableName: variablename,
          VariableDataType: this.demosettingform.controls["variabletypetesting"].value,
          VariableDataTypeName: this.demosettingform.controls["variabletypetesting"].value == 1 ? "Numeric" : "Percentage",
          UseOn: "OnAllSite",
          ProgramId: this.selectedprogramid,
          VarCode: "xx",
          AssumptionType: 3,
          AssumptionTypename: "Test_Assumption",
          VariableEffect: this.demosettingform.controls["testingeffect"].value == "Positive" ? true : false,
          IsActive: this.demosettingform.controls["Isactivetesting"].value == true ? "Yes" : "No",
        };
      } else {
        postassumption = {
          Id: 0,
          VariableName: this.demosettingform.controls["variablenametesting"].value,
          VariableDataType: this.demosettingform.controls["variabletypetesting"].value,
          VariableDataTypeName: this.demosettingform.controls["variabletypetesting"].value == 1 ? "Numeric" : "Percentage",
          UseOn: "OnAllSite",
          ProgramId: this.selectedprogramid,
          VarCode: "xx",
          AssumptionType: 3,
          AssumptionTypename: "Test_Assumption",
          VariableEffect: this.demosettingform.controls["testingeffect"].value == "Positive" ? true : false,
          IsActive: this.demosettingform.controls["Isactivetesting"].value == true ? "Yes" : "No",
        };
      }
      this.suggestedvariable = {
        id: 0,
        Name: postassumption["VariableName"],
        Type_name: "Testing Assumption",
      };
    } else {
      if (variablename != "") {
        postassumption = {
          Id: 0,
          VariableName: variablename,
          VariableDataType: this.demosettingform.controls["variabletypeproduct"].value,
          VariableDataTypeName: this.demosettingform.controls["variabletypeproduct"].value == 1 ? "Numeric" : "Percentage",
          UseOn: "OnAllSite",
          ProgramId: this.selectedprogramid,
          VarCode: "xx",
          AssumptionType: 2,
          AssumptionTypename: "Product_Assumption",
          VariableEffect: this.demosettingform.controls["producteffect"].value == "Positive" ? true : false,
          IsActive: this.demosettingform.controls["Isactiveproduct"].value == true ? "Yes" : "No",
        };
      } else {
        postassumption = {
          Id: 0,
          VariableName: this.demosettingform.controls["variablenameproduct"].value,
          VariableDataType: this.demosettingform.controls["variabletypeproduct"].value,
          VariableDataTypeName: this.demosettingform.controls["variabletypeproduct"].value == 1 ? "Numeric" : "Percentage",
          UseOn: "OnAllSite",
          ProgramId: this.selectedprogramid,
          VarCode: "xx",
          AssumptionType: 2,
          AssumptionTypename: "Product_Assumption",
          VariableEffect: this.demosettingform.controls["producteffect"].value == "Positive" ? true : false,
          IsActive: this.demosettingform.controls["Isactiveproduct"].value == true ? "Yes" : "No",
        };
      }
      this.suggestedvariable = {
        id: 0,
        Name: postassumption["VariableName"],
        Type_name: "Product Assumption",
      };
    }

    // this.patientassumptionlist.push(postassumption)

    this._APIwithActionService.postAPI(postassumption, "MMProgram", "savegeneralassumptions").subscribe((data) => {
      if (data["_body"] != 0) {
        this.getgeneralassumption();
        this.demosettingform.patchValue({
          variablenamepatient: "",
          variabletypepatient: "1",
          patienteffect: "Positive",
          Isactivepatient: true,
          variablenametesting: "",
          variabletypetesting: "1",
          testingeffect: "Positive",
          Isactivetesting: true,
          variablenameproduct: "",
          variabletypeproduct: "1",
          producteffect: "Positive",
          Isactiveproduct: true,
        });
        this.demosettingform.controls["variablenamepatient"].markAsUntouched();
        this.demosettingform.controls["variabletypepatient"].markAsUntouched();
        this.demosettingform.controls["variablenametesting"].markAsUntouched();
        this.demosettingform.controls["variabletypetesting"].markAsUntouched();
        this.demosettingform.controls["variablenameproduct"].markAsUntouched();
        this.demosettingform.controls["variabletypeproduct"].markAsUntouched();
        // if (variablename =="")
        // {
        //    // this.lgModal1.show()
        // }
      } else {
        this._GlobalAPIService.FailureMessage("Variable Name must not be duplicate");
      }
      //  this.patientassumption = this.patientassumptionlist.filter(x => x.programId === this.selectedprogramid && x.assumptionType === 1);
      //  this.testingassumption = this.Testingassumptionlist.filter(x => x.programId === this.selectedprogramid && x.assumptionType === 3);
      //  this.productassumption = this.Productassumptionlist.filter(x => x.programId === this.selectedprogramid && x.assumptionType === 3);
    });
  }
}
