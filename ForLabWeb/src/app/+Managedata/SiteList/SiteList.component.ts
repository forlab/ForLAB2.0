import { Component, Inject, OnInit, ElementRef, AfterViewInit, Renderer, ViewChild, Directive, TemplateRef } from '@angular/core';
import { Http, Headers } from '@angular/http';
// import {DataTables} from 'angular-datatables';
import { Router, ActivatedRoute, Route } from '@angular/router';
import { APIwithActionService } from '../../shared/APIwithAction.service';
import { FadeInTop } from "../../shared/animations/fade-in-top.decorator";
import { SiteCategory } from '../Category.model';
import { NotificationService } from "../../shared/utils/notification.service";
import { GlobalVariable } from '../../shared/globalclass'
import * as XLSX from 'xlsx';
import * as FileSaver from 'file-saver';
import { BsModalService, BsModalRef } from 'ngx-bootstrap';
import { ManageDatatableComponent } from 'app/shared/ui/datatable/ManageDatatable.component';
import { SiteAddStepTwoComponent } from './SiteAddStepTwo/SiteAddStepTwo.component';
import { SiteAddStepOneComponent } from './SiteAddStepOne/SiteAddStepOne.component';
import { type } from 'os';
import { DeleteModalComponent } from 'app/shared/ui/datatable/DeleteModal/DeleteModal.component';
import { GlobalAPIService } from 'app/shared/GlobalAPI.service';

declare var $: any;
declare let pdfMake: any;

@FadeInTop()
@Component({
  selector: 'app-SiteList',
  templateUrl: './SiteList.component.html'
})
export class SiteListComponent implements AfterViewInit, OnInit {

  public options;
  @ViewChild(ManageDatatableComponent) DataView: ManageDatatableComponent

  id: string;
  rowIndex: number;
  bsSiteAddModalRef1: BsModalRef;
  bsSiteEditModalRef1: BsModalRef;
  bsSiteAddModalRef2: BsModalRef;
  bsDeleteModalRef: BsModalRef;

  constructor(
    public http: Http,
    private notificationService: NotificationService,
    private _router: Router,
    private _render: Renderer,
    private _APIwithActionService: APIwithActionService,
    private modalService: BsModalService,
    private _GlobalAPIService: GlobalAPIService
  ) {

  }
  delete(SiteID) {

    let table = document.querySelector('table');
    this._APIwithActionService.deleteData(SiteID, 'Site', 'Del01').subscribe((data) => {
      this._render.setElementStyle(table.rows[this.rowIndex], 'display', 'none')
    }, error => alert(error))

  }
  ngAfterViewInit(): void {
    document.querySelector('#site-table').addEventListener('click', (event) => {
      let target = <Element>event.target;// Cast EventTarget into an Element
      if (target.className.includes('site-edit')) {
        var initialState = {
          itemID: target.getAttribute('data-site-id')
        };
        this.bsSiteEditModalRef1 = this.modalService.show(SiteAddStepOneComponent, { class: 'modal-siteadd', ignoreBackdropClick: false, initialState });
        this.bsSiteEditModalRef1.content.event.subscribe(res => {
          if (res.type == "next") {
            this.openSecondModal(res)
          }
        })
      }
      if (target.className.includes('site-delete')) {
        this.id = target.getAttribute('data-site-id');
        this.bsDeleteModalRef = this.modalService.show(DeleteModalComponent, { class: 'modal-delete' });
        this.bsDeleteModalRef.content.event.subscribe(res => {
          if (res.type == "delete") {
            this._APIwithActionService.deleteData(this.id, 'Site', 'Del01').subscribe((data) => {
              console.log('deleteData', data);
              if (data["statusText"] == "OK" && data.ok) {
                this._GlobalAPIService.SuccessMessage("Site Type Deleted Successfully");
              } else {
                this._GlobalAPIService.FailureMessage("Deletion Cancelled");
              }
              $.fn.dataTable.tables({ visible: true, api: true }).ajax.reload();
            });
          }
        })
      }
    });
  }

  ngOnInit() {
    const router = this._router;
    let siteList = new Array();
    let siteList1 = new Array();
    const fileType = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';
    const fileExtension = '.xlsx';

    var aButtonsData = [];
    aButtonsData.push({ text: '', className: 'btn-rect' });
    aButtonsData.push({
      text: '',
      className: 'btn-export',
      action: function (e, dt, node, config) {
        siteList1.push({
          Region: "Region",
          SiteCategory: "Site Category",
          SiteName: "Site Name",
          WorkingDays: "Working Days"
        })
        siteList.forEach(element => {
          siteList1.push({
            Region: element.regionName,
            SiteCategory: element.categoryName,
            SiteName: element.siteName,
            WorkingDays: element.workingDays,
          })
        });
        const ws: XLSX.WorkSheet = XLSX.utils.json_to_sheet(siteList1, { skipHeader: true });//{header:["Testing Area","Instrument Name","Max Through Put","Per Test Control","Daily Control Test","Weekly Control Test","Monthly Control Test","Quarterly control Test"]}
        const wb: XLSX.WorkBook = { Sheets: { 'Site': ws }, SheetNames: ['Site'] };
        const excelBuffer: any = XLSX.write(wb, { bookType: 'xlsx', type: 'array', cellStyles: true });
        const data1: Blob = new Blob([excelBuffer], { type: fileType });
        FileSaver.saveAs(data1, "Site List" + fileExtension);
      }
    })

    aButtonsData.push({ text: '', extend: 'pdf', className: 'btn-pdf', filename: 'Site List' });
    aButtonsData.push({ text: '', extend: 'print', className: 'btn-print' })

   // if (localStorage.getItem("role") == "admin") {
      aButtonsData.push({
        text: 'Import',
        className: 'btn-import',
        action: function (e, dt, node, config) {
          router.navigate(["/ImportData"])
        }
      });
  //  }

    aButtonsData.push({
      text: 'New Site',
      className: 'btn-new',
      action: (e, dt, node, config) => {
        this.openFirstModal();
      }
    })

    this.DataView.options = {
      dom: "Bfrtip",
      aaSorting: [],
      ajax: (data, callback, settings) => {
        this._APIwithActionService.getDatabyID(localStorage.getItem("countryid"), 'Site', 'GetAll')
          .subscribe((data) => {
            siteList = data.aaData
            callback({
              aaData: data.aaData,
            })
          })
      },

      columns: [
        { data: 'siteName' },
        { data: 'countryName' },
        { data: 'regionName' },
        { data: 'categoryName' },
        { data: 'workingDays' },
        {
          render: (data, type, fullRow, meta) => {
            return `
                      <a class='site-edit icon-edit' data-site-id="${fullRow.siteID}"></a>
                      <a class='site-delete icon-trash' data-site-id="${fullRow.siteID}"></a>
                        `;
          }
        }
      ],
      buttons: aButtonsData
    };
  }

  openFirstModal() {
    if (this.bsSiteAddModalRef2) {
      this.bsSiteAddModalRef2.hide();
    }
    this.bsSiteAddModalRef1 = this.modalService.show(SiteAddStepOneComponent, { class: 'modal-siteadd', ignoreBackdropClick: false });
    this.bsSiteAddModalRef1.content.event.subscribe(res => {
      if (res.type == "next") {
        this.openSecondModal(res)
      }
    })
  }

  openSecondModal(params) {
    if (this.bsSiteAddModalRef1) {
      this.bsSiteAddModalRef1.hide();
    }
    var initialState = {
      itemID: params.itemID,
      siteForm1: params.data
    };
    this.bsSiteAddModalRef2 = this.modalService.show(SiteAddStepTwoComponent, { class: 'modal-siteadd', ignoreBackdropClick: false, initialState });
    this.bsSiteAddModalRef2.content.event.subscribe(res => {
      if (res.type == "back") {
        this.openFirstModal();
      } else {
        $.fn.dataTable.tables({ visible: true, api: true }).ajax.reload();
      }
    })
  }

}
