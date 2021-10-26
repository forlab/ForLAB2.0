import { Component, OnInit, ElementRef, ViewChild, Renderer, TemplateRef, Input, Output, EventEmitter } from "@angular/core";
import { APIwithActionService } from "app/shared/APIwithAction.service";
import { ManageDatatableComponent } from "app/shared/ui/datatable/ManageDatatable.component";
import { BsModalRef, BsModalService } from "ngx-bootstrap";
import { NotificationService } from "app/shared/utils/notification.service";
import { Http } from "@angular/http";
import { ForecastAddComponent } from "../ForecastAdd/ForecastAdd.component";
import { ForecastTestAddComponent } from "../ForecastTestAdd/ForecastTestAdd.component";
import { ForecastInstrAddComponent } from "../ForecastInstrAdd/ForecastInstrAdd.component";
import { ForecastInstrListComponent } from "../ForecastInstrList/ForecastInstrList.component";
import { ForecastProductTestComponent } from "../ForecastProductTest/ForecastProductTest.component";
import { ForecastMethodSelectComponent } from "../ForecastMethodSelect/ForecastMethodSelect.component";
import { ForecastCalculationSelectComponent } from "../ForecastCalculationSelect/ForecastCalculationSelect.component";
import { ForecastConductSiteComponent } from "../ForecastConductSite/ForecastConductSite.component";
import { ForecastImportServiceComponent } from "../ForecastImportService/ForecastImportService.component";
import { ForecastFactorOutputComponent } from "../ForecastFactorOutput/ForecastFactorOutput.component";
import { ForecastSuccessComponent } from "../ForecastSuccess/ForecastSuccess.component";
import { ForecastMorbidityDiseaseComponent } from "../ForecastMorbidityDisease/ForecastMorbidityDisease.component";
import { ForecastMorbiditySiteComponent } from "../ForecastMorbiditySite/ForecastMorbiditySite.component";
import { ForecastMorbidityTestComponent } from "../ForecastMorbidityTest/ForecastMorbidityTest.component";
import { ForecastMorbidityProdTypeComponent } from "../ForecastMorbidityProdType/ForecastMorbidityProdType.component";
import { ForecastNewProgramComponent } from "../ForecastNewProgram/ForecastNewProgram.component";
import { ForecastNewProgramGroupComponent } from "../ForecastNewProgramGroup/ForecastNewProgramGroup.component";
import { ForecastNewProgAssumPatientComponent } from "../ForecastNewProgAssumPatient/ForecastNewProgAssumPatient.component";
import { ForecastNewProgAssumTestingComponent } from "../ForecastNewProgAssumTesting/ForecastNewProgAssumTesting.component";
import { ForecastNewProgAssumProductComponent } from "../ForecastNewProgAssumProduct/ForecastNewProgAssumProduct.component";
import { ForecastNewProgramProtocolComponent } from "../ForecastNewProgramProtocol/ForecastNewProgramProtocol.component";
import { ForecastMorbidityGroupComponent } from "../ForecastMorbidityGroup/ForecastMorbidityGroup.component";
import { Router } from "@angular/router";
import { DeleteModalComponent } from "app/shared/ui/datatable/DeleteModal/DeleteModal.component";
import { GlobalAPIService } from "app/shared/GlobalAPI.service";

declare var $: any;

@Component({
  selector: "app-conduct-forecast-list",
  templateUrl: "./ConductForecastList.component.html",
  styleUrls: ["./ConductForecastList.component.css"],
})
export class ConductForecastListComponent implements OnInit {
  @ViewChild(ManageDatatableComponent) DataView: ManageDatatableComponent;

  bsForecastAddModalRef: BsModalRef;
  bsForecastTestAddModalRef: BsModalRef;
  bsForecastInstrAddModalRef: BsModalRef;
  bsForecastInstrListModalRef: BsModalRef;
  bsForecastProductTestModalRef: BsModalRef;
  bsForecastMethodSelectModalRef: BsModalRef;

  bsForecastConductSiteModalRef: BsModalRef;
  bsForecastImportServiceModalRef: BsModalRef;
  bsForecastFactorOutputModalRef: BsModalRef;
  bsForecastCalculationSelectModalRef: BsModalRef;
  bsForecastSuccessModalRef: BsModalRef;
  bsForecastMorbidityDiseaseModalRef: BsModalRef;
  bsForecastMorbiditySiteModalRef: BsModalRef;
  bsForecastMorbidityGroupModalRef: BsModalRef;
  bsForecastMorbidityTestModalRef: BsModalRef;
  bsForecastMorbidityProdTypeModalRef: BsModalRef;
  bsForecastNewProgramModalRef: BsModalRef;
  bsForecastNewProgramGroupModalRef: BsModalRef;
  bsForecastNewProgAssumPatientModalRef: BsModalRef;
  bsForecastNewProgAssumTestingModalRef: BsModalRef;
  bsForecastNewProgAssumProductModalRef: BsModalRef;
  bsForecastNewProgramProtocolModalRef: BsModalRef;
  bsDeleteModalRef: BsModalRef;

  forecastId: number;
  programId: number;
  rowindex: number;
  checkedAll: boolean;
  constructor(
    public http: Http,
    private _GlobalAPIService: GlobalAPIService,
    private _router: Router,
    private _render: Renderer,
    private _APIwithActionService: APIwithActionService,
    private modalService: BsModalService
  ) {}

  ngAfterViewInit(): void {
    document.querySelector("#conduct-table").addEventListener("click", (event) => {
      let target = <Element>event.target; // Cast EventTarget into an Element
      if (target.className.includes("conduct-report")) {
        this._router.navigate(["/ConductForecast/ForecastReport", +target.getAttribute("data-conduct-id")]);
      } else if (target.className.includes("conduct-edit")) {
        this.forecastId = +target.getAttribute("data-conduct-id");
        this.openFirstModal();
      } else if (target.className.includes("conduct-delete")) {
        this.forecastId = parseInt(target.getAttribute("data-conduct-id"));
        this.bsDeleteModalRef = this.modalService.show(DeleteModalComponent, { class: "modal-delete" });
        this.bsDeleteModalRef.content.event.subscribe((res) => {
          if (res.type == "delete") {
            this._APIwithActionService.deleteData(this.forecastId, "Forecsatinfo", "Delete").subscribe((data) => {
              $.fn.dataTable.tables({ visible: true, api: true }).ajax.reload();
              if (parseInt(data["_body"]) > 0) {
                this._GlobalAPIService.SuccessMessage("forecast Deleted Successfully");
              } else {
                // this._GlobalAPIService.FailureMessage("Site Category already used so you can't delete this category");
              }
            });
            // this._GlobalAPIService.FailureMessage("Endpoint is processing...");
          }
        });
      }
    });
  }
  ngOnInit() {
    this.checkedAll = false;
    const APIwithActionService = this._APIwithActionService;
    var aButtonsData = new Array();
    aButtonsData.push({
      text: "",
      className: "btn-view",
      action: (e, dt, node, config) => {
        this._router.navigate(["/ConductForecast/CostsComparison"]);
      },
    });
    aButtonsData.push({ text: "", className: "btn-trash" });
    aButtonsData.push({
      text: "New Forecast",
      className: "btn-new",
      action: (e, dt, node, config) => {
        this.forecastId = 0;
        this.openFirstModal();
      },
    });
    aButtonsData.push({ text: "", className: "btn-rect" });
    this.DataView.options = {
      dom: "Bfrtip",
      aaSorting: [],
      ajax: (data, callback, settings) => {
        this._APIwithActionService.getDatabyID(0, "Forecsatinfo", "getrecentforecast").subscribe((data) => {
          console.log("Forecast Data", data);
          callback({
            aaData: data.slice(0, 100),
          });
        });
      },
      columns: [
        { data: "checked", defaultContent: "" },
        { data: "name" },
        { data: "dateofforecast" },
        { data: "duration" },
        { data: "forecastvalue" },
        {
          render: (data, type, fullRow, meta) => {
            return `
            <a class='conduct-report icon-report' data-conduct-id="${fullRow.id}"></a>
            <a class='conduct-edit icon-edit' data-conduct-id="${fullRow.id}"></a>
            <a class='conduct-delete icon-trash' data-conduct-id="${fullRow.id}"></a>
            `;
          },
        },
      ],
      columnDefs: [
        {
          targets: 0,
          orderable: false,
          className: "select-checkbox",
        },
      ],
      select: {
        style: "multi",
        selector: "td:first-child",
      },
      buttons: aButtonsData,
    };
  }

  openFirstModal() {
    var initialState = {
      forecastId: this.forecastId,
    };
    this.bsForecastAddModalRef = this.modalService.show(ForecastAddComponent, { class: "modal-forecastadd", initialState });
    this.bsForecastAddModalRef.content.event.subscribe((res) => {
      if (res.type == "next") {
        this.forecastId = res.forecastId;
        this.openForecastTestAddModal();
        // this.openForecastImportServiceModal();
      }
    });
  }

  openForecastTestAddModal() {
    var initialState = {
      forecastId: this.forecastId,
    };
    this.bsForecastTestAddModalRef = this.modalService.show(ForecastTestAddComponent, { class: "modal-forecastadd", initialState });
    this.bsForecastTestAddModalRef.content.event.subscribe((res) => {
      if (res.type == "next") {
        this.openForecastInstrAddModal();
      } else if (res.type == "back") {
        this.openFirstModal();
      }
    });
  }

  openForecastInstrAddModal() {
    var initialState = {
      forecastId: this.forecastId,
    };
    this.bsForecastInstrAddModalRef = this.modalService.show(ForecastInstrAddComponent, { class: "modal-forecastadd", initialState });
    this.bsForecastInstrAddModalRef.content.event.subscribe((res) => {
      if (res.type == "next") {
        this.openForecastInstrListModal();
      } else if (res.type == "back") {
        this.openForecastTestAddModal();
      }
    });
  }

  openForecastInstrListModal() {
    var initialState = {
      forecastId: this.forecastId,
    };
    this.bsForecastInstrListModalRef = this.modalService.show(ForecastInstrListComponent, { class: "modal-forecastadd", initialState });
    this.bsForecastInstrListModalRef.content.event.subscribe((res) => {
      if (res.type == "next") {
        this.openForecastProductTestModal();
      } else if (res.type == "back") {
        this.openForecastInstrAddModal();
      }
    });
  }

  openForecastProductTestModal() {
    var initialState = {
      forecastId: this.forecastId,
    };
    this.bsForecastProductTestModalRef = this.modalService.show(ForecastProductTestComponent, { class: "modal-forecastadd", initialState });
    this.bsForecastProductTestModalRef.content.event.subscribe((res) => {
      if (res.type == "next") {
        this.openForecastMethodSelectModal();
      } else if (res.type == "back") {
        this.openForecastInstrListModal();
      }
    });
  }

  openForecastMethodSelectModal() {
    var initialState = {
      forecastId: this.forecastId,
    };
    this.bsForecastMethodSelectModalRef = this.modalService.show(ForecastMethodSelectComponent, { class: "modal-forecastadd", initialState });
    this.bsForecastMethodSelectModalRef.content.event.subscribe((res) => {
      if (res.type == "next") {
        if (res.methodology == "MORBIDITY") {
          this.openForecastMorbidityDiseaseModal();
        } else {
          this.openForecastConductSiteModal();
        }
      } else if (res.type == "back") {
        this.openForecastProductTestModal();
      }
    });
  }

  /* Service & Consumption Modals */
  openForecastConductSiteModal() {
    var initialState = {
      forecastId: this.forecastId,
    };
    this.bsForecastConductSiteModalRef = this.modalService.show(ForecastConductSiteComponent, { class: "modal-forecastadd", initialState });
    this.bsForecastConductSiteModalRef.content.event.subscribe((res) => {
      if (res.type == "next") {
        this.openForecastImportServiceModal();
      } else if (res.type == "back") {
        if (res.methodology == "MORBIDITY") {
          this.openForecastMorbidityDiseaseModal();
        } else {
          this.openForecastMethodSelectModal();
        }
      }
    });
  }

  openForecastImportServiceModal() {
    var initialState = {
      forecastId: this.forecastId,
    };
    this.bsForecastImportServiceModalRef = this.modalService.show(ForecastImportServiceComponent, { class: "modal-forecastadd", initialState });
    this.bsForecastImportServiceModalRef.content.event.subscribe((res) => {
      if (res.type == "next") {
        if (res.methodology == "MORBIDITY") {
          this.openForecastMorbiditySiteModal();
        } else {
          this.openForecastFactorOutputModal();
        }
      } else if (res.type == "back") {
        this.openForecastConductSiteModal();
      }
    });
  }

  openForecastFactorOutputModal() {
    var initialState = {
      forecastId: this.forecastId,
    };
    this.bsForecastFactorOutputModalRef = this.modalService.show(ForecastFactorOutputComponent, { class: "modal-forecastadd", initialState });
    this.bsForecastFactorOutputModalRef.content.event.subscribe((res) => {
      if (res.type == "next") {
        // this.openForecastCalculationSelectModal();
        this.openForecastSuccessModal();
      } else if (res.type == "back") {
        this.openForecastImportServiceModal();
      }
    });
  }

  openForecastCalculationSelectModal() {
    var initialState = {
      forecastId: this.forecastId,
    };
    this.bsForecastCalculationSelectModalRef = this.modalService.show(ForecastCalculationSelectComponent, { class: "modal-forecastadd", initialState });
    this.bsForecastCalculationSelectModalRef.content.event.subscribe((res) => {
      if (res.type == "next") {
        this.openForecastSuccessModal();
      } else if (res.type == "back") {
        this.openForecastFactorOutputModal();
      }
    });
  }

  openForecastSuccessModal() {
    var initialState = {
      forecastId: this.forecastId,
    };
    this.bsForecastSuccessModalRef = this.modalService.show(ForecastSuccessComponent, { class: "modal-forecastadd", initialState });
    this.bsForecastSuccessModalRef.content.event.subscribe((res) => {
      if (res.type == "next") {
        this._router.navigate(["/ConductForecast/ForecastReport", this.forecastId]);
      } else if (res.type == "back") {
        this.openForecastFactorOutputModal();
      }
    });
  }

  /* Morbidity Modals */
  openForecastMorbidityDiseaseModal() {
    var initialState = {
      forecastId: this.forecastId,
    };
    this.bsForecastMorbidityDiseaseModalRef = this.modalService.show(ForecastMorbidityDiseaseComponent, { class: "modal-forecastadd", initialState });
    this.bsForecastMorbidityDiseaseModalRef.content.event.subscribe((res) => {
      if (res.type == "next") {
        this.programId = res.programId;
        if (res.methodology == "createProgram") {
          this.openForecastNewProgramModal();
        } else {
          this.openForecastConductSiteModal();
        }
      } else if (res.type == "back") {
        this.openForecastMethodSelectModal();
      }
    });
  }

  openForecastMorbiditySiteModal() {
    var initialState = {
      forecastId: this.forecastId,
    };
    this.bsForecastMorbiditySiteModalRef = this.modalService.show(ForecastMorbiditySiteComponent, { class: "modal-forecastadd", initialState });
    this.bsForecastMorbiditySiteModalRef.content.event.subscribe((res) => {
      if (res.type == "next") {
        this.openForecastMorbidityGroupModal();
      } else if (res.type == "back") {
        this.openForecastImportServiceModal();
      }
    });
  }

  openForecastMorbidityGroupModal() {
    var initialState = {
      forecastId: this.forecastId,
    };
    this.bsForecastMorbidityGroupModalRef = this.modalService.show(ForecastMorbidityGroupComponent, { class: "modal-forecastadd", initialState });
    this.bsForecastMorbidityGroupModalRef.content.event.subscribe((res) => {
      if (res.type == "next") {
        this.openForecastMorbidityTestModal();
      } else if (res.type == "back") {
        this.openForecastMorbiditySiteModal();
      }
    });
  }

  openForecastMorbidityTestModal() {
    var initialState = {
      forecastId: this.forecastId,
      programId: this.programId,
    };
    this.bsForecastMorbidityTestModalRef = this.modalService.show(ForecastMorbidityTestComponent, { class: "modal-forecastadd", initialState });
    this.bsForecastMorbidityTestModalRef.content.event.subscribe((res) => {
      if (res.type == "next") {
        this.openForecastMorbidityProdTypeModal();
      } else if (res.type == "back") {
        this.openForecastMorbidityGroupModal();
      }
    });
  }

  openForecastMorbidityProdTypeModal() {
    var initialState = {
      forecastId: this.forecastId,
      programId: this.programId,
    };
    this.bsForecastMorbidityProdTypeModalRef = this.modalService.show(ForecastMorbidityProdTypeComponent, { class: "modal-forecastadd", initialState });
    this.bsForecastMorbidityProdTypeModalRef.content.event.subscribe((res) => {
      if (res.type == "next") {
        this.openForecastSuccessModal();
      } else if (res.type == "back") {
        this.openForecastMorbidityTestModal();
      }
    });
  }

  /* Morbidity Create New Program */
  openForecastNewProgramModal() {
    var initialState = {
      forecastId: this.forecastId,
      programId: this.programId,
    };
    this.bsForecastNewProgramModalRef = this.modalService.show(ForecastNewProgramComponent, { class: "modal-forecastadd", initialState });
    this.bsForecastNewProgramModalRef.content.event.subscribe((res) => {
      if (res.type == "next") {
        this.openForecastNewProgramGroupModal();
      } else if (res.type == "back") {
        if (res.methodology == "cancelCreateProgram") {
          this.openForecastMorbidityDiseaseModal();
        } else {
          this.openForecastMorbidityDiseaseModal();
        }
      }
    });
  }

  openForecastNewProgramGroupModal() {
    var initialState = {
      forecastId: this.forecastId,
      programId: this.programId,
    };
    this.bsForecastNewProgramGroupModalRef = this.modalService.show(ForecastNewProgramGroupComponent, { class: "modal-forecastadd", initialState });
    this.bsForecastNewProgramGroupModalRef.content.event.subscribe((res) => {
      if (res.type == "next") {
        this.openForecastNewProgAssumPatientModal();
      } else if (res.type == "back") {
        if (res.methodology == "cancelCreateProgram") {
          this.openForecastMorbidityDiseaseModal();
        } else {
          this.openForecastNewProgramModal();
        }
      }
    });
  }

  openForecastNewProgAssumPatientModal() {
    var initialState = {
      forecastId: this.forecastId,
      programId: this.programId,
    };
    this.bsForecastNewProgAssumPatientModalRef = this.modalService.show(ForecastNewProgAssumPatientComponent, { class: "modal-forecastadd", initialState });
    this.bsForecastNewProgAssumPatientModalRef.content.event.subscribe((res) => {
      if (res.type == "next") {
        this.openForecastNewProgAssumTestingModal();
      } else if (res.type == "back") {
        if (res.methodology == "cancelCreateProgram") {
          this.openForecastMorbidityDiseaseModal();
        } else {
          this.openForecastNewProgramGroupModal();
        }
      }
    });
  }

  openForecastNewProgAssumTestingModal() {
    var initialState = {
      forecastId: this.forecastId,
      programId: this.programId,
    };
    this.bsForecastNewProgAssumTestingModalRef = this.modalService.show(ForecastNewProgAssumTestingComponent, { class: "modal-forecastadd", initialState });
    this.bsForecastNewProgAssumTestingModalRef.content.event.subscribe((res) => {
      if (res.type == "next") {
        this.openForecastNewProgAssumProductModal();
      } else if (res.type == "back") {
        if (res.methodology == "cancelCreateProgram") {
          this.openForecastMorbidityDiseaseModal();
        } else {
          this.openForecastNewProgAssumPatientModal();
        }
      }
    });
  }

  openForecastNewProgAssumProductModal() {
    var initialState = {
      forecastId: this.forecastId,
      programId: this.programId,
    };
    this.bsForecastNewProgAssumProductModalRef = this.modalService.show(ForecastNewProgAssumProductComponent, { class: "modal-forecastadd", initialState });
    this.bsForecastNewProgAssumProductModalRef.content.event.subscribe((res) => {
      if (res.type == "next") {
        this.openForecastNewProgramProtocolModal();
      } else if (res.type == "back") {
        if (res.methodology == "cancelCreateProgram") {
          this.openForecastMorbidityDiseaseModal();
        } else {
          this.openForecastNewProgAssumTestingModal();
        }
      }
    });
  }

  openForecastNewProgramProtocolModal() {
    var initialState = {
      forecastId: this.forecastId,
      programId: this.programId,
    };
    this.bsForecastNewProgramProtocolModalRef = this.modalService.show(ForecastNewProgramProtocolComponent, { class: "modal-forecastadd", initialState });
    this.bsForecastNewProgramProtocolModalRef.content.event.subscribe((res) => {
      if (res.type == "next") {
        this.openForecastMorbidityDiseaseModal();
      } else if (res.type == "back") {
        if (res.methodology == "cancelCreateProgram") {
          this.openForecastMorbidityDiseaseModal();
        } else {
          this.openForecastNewProgAssumProductModal();
        }
      }
    });
  }

  onCheckedAllChange() {
    if (!this.checkedAll) {
      $.fn.dataTable.tables({ visible: true, api: true }).rows().select();
    } else {
      $.fn.dataTable.tables({ visible: true, api: true }).rows().deselect();
    }
    this.checkedAll = !this.checkedAll;
  }
}
