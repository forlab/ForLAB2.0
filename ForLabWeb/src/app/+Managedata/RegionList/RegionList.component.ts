import { Component, Inject, OnInit, ElementRef, AfterViewInit, Renderer, ViewChild, Directive, TemplateRef } from '@angular/core';
import { Http, Headers } from '@angular/http';
import { Router, ActivatedRoute, Route } from '@angular/router';
import { GlobalAPIService } from '../../shared/GlobalAPI.service';
import { APIwithActionService } from '../../shared/APIwithAction.service';
import { FadeInTop } from "../../shared/animations/fade-in-top.decorator";
import { NotificationService } from "../../shared/utils/notification.service";
import * as XLSX from 'xlsx';
import * as FileSaver from 'file-saver';
import { BsModalRef, BsModalService } from 'ngx-bootstrap';
import { ManageDatatableComponent } from 'app/shared/ui/datatable/ManageDatatable.component';
import { RegionAddComponent } from '../RegionAdd/RegionAdd.component';
import { DeleteModalComponent } from 'app/shared/ui/datatable/DeleteModal/DeleteModal.component';
declare var $: any;
declare let pdfMake: any;

@FadeInTop()
@Component({
  selector: 'app-RegionList',
  templateUrl: './RegionList.component.html'
})
export class RegionlistComponent implements AfterViewInit, OnInit {

  public options;
  @ViewChild(ManageDatatableComponent) DataView: ManageDatatableComponent
  id: number;
  rowindex: number;
  bsRegionAddModalRef: BsModalRef;
  bsRegionEditModalRef: BsModalRef;
  bsDeleteModalRef: BsModalRef;
  countryList: Array<any> = new Array();

  constructor(public http: Http, private _GlobalAPIService: GlobalAPIService, private _APIwithActionService: APIwithActionService, private modalService: BsModalService, private _router: Router) { }

  ngOnInit() {
    const router = this._router
    let regionlist = new Array();
    let regionlist1 = new Array();
    const fileType = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';
    const fileExtension = '.xlsx';

    var aButtonsData = [];
    aButtonsData.push({ text: '', className: 'btn-rect' });
    aButtonsData.push({
      text: '',
      className: 'btn-export',
      action: function (e, dt, node, config) {
        regionlist1.push({
          RegionName: "Region",
          ShortName: "Short Name",
        })
        regionlist.forEach(element => {
          regionlist1.push({
            RegionName: element.regionName,
            ShortName: element.shortName,
          })
        });
        const ws: XLSX.WorkSheet = XLSX.utils.json_to_sheet(regionlist1, { skipHeader: true });//{header:["Testing Area","Instrument Name","Max Through Put","Per Test Control","Daily Control Test","Weekly Control Test","Monthly Control Test","Quarterly control Test"]}
        const wb: XLSX.WorkBook = { Sheets: { 'Region': ws }, SheetNames: ['Region'] };
        const excelBuffer: any = XLSX.write(wb, { bookType: 'xlsx', type: 'array', cellStyles: true });
        const data1: Blob = new Blob([excelBuffer], { type: fileType });
        FileSaver.saveAs(data1, "Region List" + fileExtension);
      }
    })
    aButtonsData.push({ text: '', extend: 'pdf', className: 'btn-pdf', filename: 'Region List' });
    aButtonsData.push({ text: '', extend: 'print', className: 'btn-print' })
  //  if (localStorage.getItem("role") == "admin") {
      aButtonsData.push({
        text: 'Import',
        className: 'btn-import',
        action: function (e, dt, node, config) {
          router.navigate(["/ImportData"])
        }
      });
  //  }
    aButtonsData.push({
      text: 'New Region',
      className: 'btn-new',
      action: (e, dt, node, config) => {
        this.bsRegionAddModalRef = this.modalService.show(RegionAddComponent, { class: 'modal-regionadd', ignoreBackdropClick: false });
        this.bsRegionAddModalRef.content.event.subscribe(res => {
          $.fn.dataTable.tables({ visible: true, api: true }).ajax.reload();
        })
      }
    })
    this.DataView.options = {
      dom: "Bfrtip",
      aaSorting: [],
      ajax: (data, callback, settings) => {
        this._APIwithActionService.getDatabyID(localStorage.getItem("countryid"), 'Site', 'GetregionbyCountryID').subscribe((data) => {
          regionlist = data;
          callback({
            aaData: data,
          })
        })
      },
      columns: [
        { data: 'regionName' },
        { data: 'countryName' },
        {
          render: (data, type, fullRow, meta) => {
            return `
                      <a class='region-edit icon-edit' data-reg-id="${fullRow.regionID}"></a>
                      <a class='region-delete icon-trash' data-reg-id="${fullRow.regionID}"></a>
                    `;
          }
        }
      ],
      buttons: aButtonsData
    };
  }

  ngAfterViewInit(): void {
    document.querySelector('#region-table').addEventListener('click', (event) => {
      let target = <Element>event.target;
      if (target.className.includes('region-edit')) {
        var initialState = {
          itemID: target.getAttribute('data-reg-id')
        };
        this.bsRegionEditModalRef = this.modalService.show(RegionAddComponent, { class: 'modal-regionadd', ignoreBackdropClick: false, initialState });
        this.bsRegionEditModalRef.content.event.subscribe(res => {
          $.fn.dataTable.tables({ visible: true, api: true }).ajax.reload();
        })
      }
      if (target.className.includes('region-delete')) {
        this.id = parseInt(target.getAttribute('data-reg-id'));
        this.bsDeleteModalRef = this.modalService.show(DeleteModalComponent, { class: 'modal-delete' });
        this.bsDeleteModalRef.content.event.subscribe(res => {
          if (res.type == "delete") {
            this._GlobalAPIService.deleteData(this.id, 'Region').subscribe((data) => {
              $.fn.dataTable.tables({ visible: true, api: true }).ajax.reload();
              if (data["_body"] != 0) {
                this._GlobalAPIService.SuccessMessage("Product Type Deleted Successfully");
              } else {
                this._GlobalAPIService.FailureMessage("Product Type already used so you can't delete this ProductType");
              }
            });
          }
        })
      }
    });
  }

}