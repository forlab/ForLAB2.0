import { Component, Inject, OnInit, ElementRef, AfterViewInit, Renderer, ViewChild, Directive, TemplateRef } from '@angular/core';
import { Http, Headers } from '@angular/http';
// import {DataTables} from 'angular-datatables';
import { Router, ActivatedRoute, Route } from '@angular/router';
import { GlobalAPIService } from '../../shared/GlobalAPI.service';
import { FadeInTop } from "../../shared/animations/fade-in-top.decorator";
import { SiteCategory } from '../CategoryAdd/Category.model';
import { NotificationService } from "../../shared/utils/notification.service";
import { BsModalRef, BsModalService } from 'ngx-bootstrap';
import { ManageDatatableComponent } from 'app/shared/ui/datatable/ManageDatatable.component';

import * as XLSX from 'xlsx';
import * as FileSaver from 'file-saver';

import { ModalDirective } from "ngx-bootstrap";
import { ScriptService } from '../Managedata.service';
import { AddSitecategoryComponent } from '../CategoryAdd/CategoryAdd.component';
import { DeleteModalComponent } from 'app/shared/ui/datatable/DeleteModal/DeleteModal.component';
declare let pdfMake: any;
declare var $: any;
@FadeInTop()
@Component({
  selector: 'app-CategoryList',
  templateUrl: './CategoryList.component.html'
})
export class CategoryListComponent implements AfterViewInit, OnInit {
  public catList: SiteCategory[];
  public options;
  @ViewChild(ManageDatatableComponent) DataView: ManageDatatableComponent
  @ViewChild(TemplateRef) cateaddModal: any;
  id: number;
  rowindex: number;
  bsCateAddModalRef: BsModalRef;
  bsCateEditModalRef: BsModalRef;
  bsDeleteModalRef: BsModalRef;

  constructor(public http: Http, private notificationService: NotificationService, private _router: Router, private _render: Renderer,
    private _GlobalAPIService: GlobalAPIService, private modalService: BsModalService) {
    // this.getSiteCategories();
  }

  ngOnInit() {
    const router = this._router
    var aButtonsData = [];
    aButtonsData.push({ text: '', className: 'btn-rect' });
    aButtonsData.push({ text: '', extend: 'csv', className: 'btn-export', filename: 'Category List' });
    aButtonsData.push({ text: '', extend: 'pdf', className: 'btn-pdf', filename: 'Category List' });
    aButtonsData.push({ text: '', extend: 'print', className: 'btn-print' })
   // if (localStorage.getItem("role") == "admin") {
      aButtonsData.push({
        text: 'Import',
        className: 'btn-import',
        action: function (e, dt, node, config) {
          router.navigate(["/ImportData"])
        }
      });
 //   }
    aButtonsData.push({
      text: 'New Category',
      className: 'btn-new',
      action: (e, dt, node, config) => {
        this.bsCateAddModalRef = this.modalService.show(AddSitecategoryComponent, { class: 'modal-cateadd', ignoreBackdropClick: false });
        this.bsCateAddModalRef.content.event.subscribe(res => {
          $.fn.dataTable.tables({ visible: true, api: true }).ajax.reload();
        })
      }
    })

    this.DataView.options = {
      dom: "Bfrtip",
      aaSorting: [],
      ajax: (data, callback, settings) => {
        this._GlobalAPIService.getDataList('SiteCategory')
          .subscribe((data) => {
            callback({
              aaData: data.aaData,
            })
          })
      },
      columns: [
        { data: 'categoryName' },
        {
          render: (data, type, fullRow, meta) => {
            return `
                      <a class='category-edit icon-edit' data-cat-id="${fullRow.categoryID}"></a>
                      <a class='category-delete icon-trash' data-cat-id="${fullRow.categoryID}"></a>
                      `;
          }
        }],
      buttons: aButtonsData
    };
  }

  getSiteCategories() {
    this._GlobalAPIService.getDataList('SiteCategory').subscribe((data) => {
      this.catList = data
    }), err => {
      console.log(err);
    }
  }

  ngAfterViewInit(): void {
    document.querySelector('#category-table').addEventListener('click', (event) => {
      let target = <Element>event.target;// Cast EventTarget into an Element
      if (target.className.includes('category-edit')) {
        var initialState = {
          itemID: target.getAttribute('data-cat-id')
        };
        this.bsCateAddModalRef = this.modalService.show(AddSitecategoryComponent, { class: 'modal-cateadd', ignoreBackdropClick: false, initialState });
        this.bsCateAddModalRef.content.event.subscribe(res => {
          $.fn.dataTable.tables({ visible: true, api: true }).ajax.reload();
        })
      }
      if (target.className.includes('category-delete')) {
        this.id = parseInt(target.getAttribute('data-cat-id'));
        this.bsDeleteModalRef = this.modalService.show(DeleteModalComponent, { class: 'modal-delete' });
        this.bsDeleteModalRef.content.event.subscribe(res => {
          if (res.type == "delete") {
            this._GlobalAPIService.deleteData(this.id, 'SiteCategory').subscribe((data) => {
              $.fn.dataTable.tables({ visible: true, api: true }).ajax.reload();
              if (data["_body"] != 0) {
                this._GlobalAPIService.SuccessMessage("SiteCategory Deleted Successfully");
              } else {
                this._GlobalAPIService.FailureMessage("Site Category already used so you can't delete this category");
              }
            });
          }
        })
      }
    });
  }

}