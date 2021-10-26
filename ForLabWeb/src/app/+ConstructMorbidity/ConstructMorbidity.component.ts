import { Component, ViewChild, TemplateRef, Renderer } from "@angular/core";
import { APIwithActionService } from "../shared/APIwithAction.service";
import { ManageDatatableComponent } from "app/shared/ui/datatable/ManageDatatable.component";
import { GlobalAPIService } from "../shared/GlobalAPI.service";
import { BsModalRef, BsModalService } from "ngx-bootstrap";
import { Http } from "@angular/http";
import { NotificationService } from "app/shared/utils/notification.service";
import { Router } from "@angular/router";
import { ProgramAddComponent } from "./ProgramAdd/ProgramAdd.component";
import { DeleteModalComponent } from "app/shared/ui/datatable/DeleteModal/DeleteModal.component";
declare var $: any;
@Component({
  selector: "app-conduct-morbidity",
  templateUrl: "./ConstructMorbidity.component.html",
  styleUrls: ["./ConstructMorbidity.component.css"],
})
export class ConstructMorbidityComponent {
  @ViewChild(ManageDatatableComponent) DataView: ManageDatatableComponent;
  @ViewChild(TemplateRef) programaddModal: any;
  id: number;
  totalforecast: number;
  rowindex: number;
  bsProgramAddModalRef: BsModalRef;
  ProgramList = new Array();
  bsProgramEditModalRef: BsModalRef;
  bsDeleteModalRef: BsModalRef;
  constructor(
    public http: Http,
    private _GlobalAPIService: GlobalAPIService,
    private _router: Router,
    private _APIwithActionService: APIwithActionService,
    private modalService: BsModalService
  ) {}

  ngAfterViewInit(): void {
    document.querySelector("body").addEventListener("click", (event) => {
      let target = <Element>event.target; // Cast EventTarget into an Element
      if (target.className.includes("Edit")) {
        this.id = +target.getAttribute("data-pro-id");
        this.totalforecast = +target.getAttribute("data-totalfore");
        var initialState = {
          itemid: this.id,
          totalforecast: this.totalforecast,
        };
        this.bsProgramEditModalRef = this.modalService.show(ProgramAddComponent, { class: "modal-programadd", initialState });
        this.bsProgramEditModalRef.content.event.subscribe((res) => {
          // if (res.type == "next") {
          //   this.forecastId = res.forecastId;
          //   this.openForecastTestAddModal();
          //   // this.openForecastNewProgramModal();
          // }
        });
        // this._router.navigate(["/Managedata/InstrumentAdd", target.getAttribute('data-ins-id')]);
      }
      if (target.className.includes("del")) {
        this.id = parseInt(target.getAttribute("data-pro-id"));
        this.totalforecast = +target.getAttribute("data-totalfore");
        if (this.totalforecast > 0) {
          this._GlobalAPIService.FailureMessage("Program already used in forecast you can't delete this program");
        } else {
          this.bsDeleteModalRef = this.modalService.show(DeleteModalComponent, { class: "modal-delete" });
          this.bsDeleteModalRef.content.event.subscribe((res) => {
            if (res.type == "delete") {
              this._APIwithActionService.deleteData(this.id, "MMProgram", "Delete").subscribe((data) => {
                $.fn.dataTable.tables({ visible: true, api: true }).ajax.reload();
                if (data["_body"] != 0) {
                  this._GlobalAPIService.SuccessMessage("Program Deleted Successfully");
                } else {
                }
              });
            }
          });
        }
        // this.id = parseInt(target.getAttribute('data-ins-id'));
        // this.rowindex = target.parentElement.parentElement["rowIndex"];
      }
    });
  }
  ngOnInit() {
    const router = this._router;
    const APIwithActionService = this._APIwithActionService;
    var aButtonsData = new Array();
    aButtonsData.push({
      text: "New Program",
      className: "btn-new",
      action: (e, dt, node, config) => {
        this.bsProgramAddModalRef = this.modalService.show(ProgramAddComponent, { class: "modal-programadd", ignoreBackdropClick: false });
      },
    });
    aButtonsData.push({ text: "", className: "btn-rect" });

    this.DataView.options = {
      dom: "Bfrtip",
      aaSorting: [],
      ajax: (data, callback, settings) => {
        this._APIwithActionService.getDataList("MMProgram", "Getprogramlist").subscribe((data) => {
          callback({
            aaData: data,
          });
        });
      },
      columns: [
        { data: "programName" },
        { data: "forecastmethod" },
        { data: "totalforecast" },
        {
          render: (data, type, fullRow, meta) => {
            return `
            <a class='con-Edit icon-edit' data-pro-id="${fullRow.id}" data-totalfore="${fullRow.totalforecast}"></a>
            <a class='con-del icon-trash' data-pro-id="${fullRow.id}" data-totalfore="${fullRow.totalforecast}"></a>
            `;
          },
        },
      ],
      columnDefs: [
        {
          targets: 0,
          orderable: false,
        },
      ],
      select: {
        style: "os",
        selector: "td:first-child",
      },
      buttons: aButtonsData,
    };
  }

  onProgramAddClose() {
    this.bsProgramAddModalRef.hide();
  }
  getprogramlist() {
    this._APIwithActionService.getDataList("MMProgram", "Getprogramlist").subscribe((resp) => {
      this.ProgramList = resp;
    });
  }
}
