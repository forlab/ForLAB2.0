import { Component, OnInit, ElementRef, AfterViewInit, Renderer, ViewChild, TemplateRef } from '@angular/core';
import { Http } from '@angular/http';
import { Router, ActivatedRoute } from '@angular/router';
import { APIwithActionService } from '../../shared/APIwithAction.service';
import { FadeInTop } from "../../shared/animations/fade-in-top.decorator";
import { NotificationService } from "../../shared/utils/notification.service";
import * as XLSX from 'xlsx';
import * as FileSaver from 'file-saver';
import { BsModalRef, BsModalService } from 'ngx-bootstrap';
import { ManageDatatableComponent } from 'app/shared/ui/datatable/ManageDatatable.component';
import { ProductAddComponent } from '../ProductAdd/ProductAdd.component';
import { DeleteModalComponent } from 'app/shared/ui/datatable/DeleteModal/DeleteModal.component';
import { GlobalAPIService } from 'app/shared/GlobalAPI.service';
declare var $: any;

@FadeInTop()
@Component({
  selector: 'app-ProductList',
  templateUrl: './ProductList.component.html'
})
export class ProductListComponent implements AfterViewInit, OnInit {
  public options;
  @ViewChild(ManageDatatableComponent) DataView: ManageDatatableComponent
  @ViewChild(TemplateRef) proaddModal: any;
  id: string;
  bsProAddModalRef: BsModalRef;
  bsProEditModalRef: BsModalRef;
  bsDeleteModalRef: BsModalRef;

  constructor(public http: Http, private _GlobalAPIService: GlobalAPIService, private _router: Router,
    private _APIwithActionServicee: APIwithActionService, private modalService: BsModalService) {

  }
  ngOnInit() {
    const router = this._router
    let productlist = new Array();
    let productlist1 = new Array();
    const fileType = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';
    const fileExtension = '.xlsx';
    var aButtonsData = [];
    aButtonsData.push({ text: '', className: 'btn-rect' });
    aButtonsData.push({
      text: '',
      className: 'btn-export',
      action: function (e, dt, node, config) {
        productlist.push({
          ProductName: "Product Name",
          ProductType: "Product Type",
          SerialNo: "Serial No",
          Specification: "Specification",
          BasicUnit: "Basic Unit",
          Minpackpersite: "Min Packs Per Site",
          RapidTestSpecification: "Rapid Test Specification",
          Price: "Price",
          PackingSize: "Packing Size",
          PriceAsDate: "Price As of Date"
        })
        productlist1.forEach(element => {
          productlist.push({
            ProductName: element.productName,
            ProductType: element.productType,
            SerialNo: element.catalog,
            Specification: "",
            BasicUnit: element.basicUnit,
            Minpackpersite: element.minpacksize,
            RapidTestSpecification: "",
            Price: element.packcost,
            PackingSize: element.packsize,
            PriceAsDate: element.priceDate
          })
        });
        const ws: XLSX.WorkSheet = XLSX.utils.json_to_sheet(productlist, { skipHeader: true });//{header:["Testing Area","Instrument Name","Max Through Put","Per Test Control","Daily Control Test","Weekly Control Test","Monthly Control Test","Quarterly control Test"]}
        ws["A1"].fill = {
          type: 'pattern',
          pattern: 'darkVertical',
          fgColor: 'red'//{argb:'FFFF0000'}
        };
        ws["A1"].font = {
          bold: true
        }
        const wb: XLSX.WorkBook = { Sheets: { 'Product': ws }, SheetNames: ['Product'] };
        const excelBuffer: any = XLSX.write(wb, { bookType: 'xlsx', type: 'array', cellStyles: true });
        const data1: Blob = new Blob([excelBuffer], { type: fileType });
        FileSaver.saveAs(data1, "Product List" + fileExtension);
      }
    })
    aButtonsData.push({ text: '', extend: 'pdf', className: 'btn-pdf', filename: 'Product List' })
    aButtonsData.push({ text: '', extend: 'print', className: 'btn-print' })
 //   if (localStorage.getItem("role") == "admin") {
      aButtonsData.push({
        text: 'Import',
        className: 'btn-import',
        action: function (e, dt, node, config) {
          router.navigate(["/ImportData"])
        }
      });
 //   }
    aButtonsData.push({
      text: 'New Product',
      className: 'btn-new',
      action: (e, dt, node, config) => {
        this.bsProAddModalRef = this.modalService.show(ProductAddComponent, { class: 'modal-proadd', ignoreBackdropClick: false });
        this.bsProAddModalRef.content.event.subscribe(res => {
          $.fn.dataTable.tables({ visible: true, api: true }).ajax.reload();
        })
      }
    })
    this.DataView.options = {
      dom: "Bfrtip",
      aaSorting: [],
      ajax: (data, callback, settings) => {
        this._APIwithActionServicee.getDataList('Product', 'GetAll').subscribe((data) => {
          productlist1 = data.aaData
          callback({
            aaData: data.aaData,
          })
        })
      },
      columns: [
        { data: 'productName' },
        { data: 'productType' },
        { data: 'basicUnit' },
        { data: 'packsize' },
        { data: 'packcost' },      
        { data: 'minpacksize' },
        {
          render: (data, type, fullRow, meta) => {
            return `
                      <a class='product-edit icon-edit' data-pro-id="${fullRow.productID}"></a>
                      <a class='product-delete icon-trash' data-pro-id="${fullRow.productID}"></a>
                    `;
          }
        }
      ],
      buttons: aButtonsData
    };
  }

  delete(ProductID) {

    this._APIwithActionServicee.deleteData(ProductID, 'Product', 'Del01').subscribe((data) => {
    }, error => alert(error))

  }
  ngAfterViewInit(): void {
    document.querySelector('#product-table').addEventListener('click', (event) => {
      let target = <Element>event.target;// Cast EventTarget into an Element

      if (target.className.includes('product-edit')) {
        // this._router.navigate(["/Managedata/ProductAdd", target.getAttribute('data-pro-id')]);
        var initialState = {
          itemID: target.getAttribute('data-pro-id')
        };
        this.bsProEditModalRef = this.modalService.show(ProductAddComponent, { class: 'modal-proadd', ignoreBackdropClick: false, initialState });
        this.bsProEditModalRef.content.event.subscribe(res => {
          $.fn.dataTable.tables({ visible: true, api: true }).ajax.reload();
        })
      }
      if (target.className.includes('product-delete')) {
        this.id = target.getAttribute('data-pro-id');
        this.bsDeleteModalRef = this.modalService.show(DeleteModalComponent, { class: 'modal-delete' });
        this.bsDeleteModalRef.content.event.subscribe(res => {
          if (res.type == "delete") {
            this._APIwithActionServicee.deleteData(this.id, 'Product', 'Del01').subscribe((data) => {
              $.fn.dataTable.tables({ visible: true, api: true }).ajax.reload();
              if (data["_body"] != 0) {
                this._GlobalAPIService.SuccessMessage("Product Deleted Successfully");
              } else {
                this._GlobalAPIService.FailureMessage("Product already used so you can't delete this Product");
              }
            });
          }
        })
      }
    });
  }

}