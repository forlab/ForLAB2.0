import { Component, OnInit, ViewChild, Injector, ViewEncapsulation, Input, ChangeDetectionStrategy } from '@angular/core';
import { TableColumn } from 'src/@core/models/common/response';
import { SelectionModel } from '@angular/cdk/collections';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { BaseService } from 'src/@core/services/base.service';
import { MatPaginator } from '@angular/material/paginator';
import { VendorContactDto } from 'src/@core/models/vendor/VendorContact';
import { VendorDto } from 'src/@core/models/vendor/Vendor';
import { VendorContactsController } from 'src/@core/APIs/VendorContactsController';
import { map, takeUntil } from 'rxjs/operators';
import { VendorsController } from 'src/@core/APIs/VendorsController';
import { FormValidationError } from 'src/@core/models/common/SharedModel';
import { TableVirtualScrollDataSource } from 'ng-table-virtual-scroll';
import { combineLatest } from 'rxjs';

@Component({
  selector: 'import-vendor-contacts',
  templateUrl: './import-vendor-contacts.component.html',
  styleUrls: ['./import-vendor-contacts.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ImportVendorContactsComponent extends BaseService implements OnInit {
  // Drp
  vendors: VendorDto[] = [];
  // Table
  @Input('data') data: VendorContactDto[] = [];
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  columns: TableColumn<VendorContactDto>[] = [
    { label: 'Actions', property: 'actions', type: 'button', visible: true },
    { label: '#', property: 'index', type: 'custom', visible: true },
    { label: 'Vendor', property: 'vendorId', type: 'select', visible: true },
    { label: 'Name', property: 'name', type: 'text', visible: true },
    { label: 'Email', property: 'email', type: 'text', visible: true },
    { label: 'Telephone', property: 'telephone', type: 'text', visible: true },
  ];
  dataSource = new TableVirtualScrollDataSource();
  selection = new SelectionModel<VendorContactDto>(true, []);
  // Form
  formArry: FormGroup[];
  // vars
  loadingData: boolean = true;
  errors: FormValidationError[] = [];
  successRes: any;
  done: boolean = false;

  constructor(private fb: FormBuilder, public injector: Injector) {
    super(injector);
  }

  ngOnInit() {

    combineLatest([this.loadVendors()])
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(([vendors]) => {
        // Set Drp data
        this.vendors = vendors;

        // Patch the Table Data
        this.formArry = Array.from(Array(this.data.length), (_, i) => i + 1).map(x => {
          let form = this.appendObject();

          // Vendor
          this.data[x - 1].vendorId = vendors.find(item => item.name.trim().toLowerCase() == this.data[x - 1].vendorName.trim().toLowerCase())?.id;

          // Patch FormGroup Value
          form.patchValue(this.data[x - 1]);
          return form;
        });

        this.loadingData = false;
        this.dataSource = new TableVirtualScrollDataSource(this.formArry);
        this.dataSource.paginator = this.paginator;
      });

  }

  loadVendors() {
    return this.httpService.GET(VendorsController.GetAllAsDrp).pipe(map(res => res.data));
  }

  get visibleColumns() {
    if (!this.columns) {
      return [];
    }
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  toggleColumnVisibility(column, event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    column.visible = !column.visible;
  }

  trackByProperty<T>(index: number, column: TableColumn<T>) {
    return column.property;
  }

  appendObject() {
    return this.fb.group({
      vendorId: [null, Validators.compose([Validators.required])],
      name: [null, Validators.compose([Validators.required])],
      email: [null, Validators.compose([Validators.required, Validators.email])],
      telephone: [null, Validators.compose([Validators.required])],
    })
  }

  addObject() {
    this.formArry.unshift(this.appendObject());
    this.dataSource = new TableVirtualScrollDataSource(this.formArry);
    this.dataSource.paginator = this.paginator;
  }

  deleteObject(row) {
    const index = this.formArry.indexOf(row);
    this.formArry.splice(index, 1);
    this.dataSource = new TableVirtualScrollDataSource(this.formArry);
    this.dataSource.paginator = this.paginator;
  }

  submit() {
    const invalidFormGroups = this.formArry?.filter(x => x.invalid);
    /** check form */
    this.errors = [];
    if (invalidFormGroups?.length > 0) {
      invalidFormGroups.forEach(x => x.markAllAsTouched());
      this.errors = this.getFormGroupsErrors(invalidFormGroups, this.formArry);
      return;
    }

    this.loading = true;
    let objectDtos: VendorContactDto[] = [];

    objectDtos = this.formArry.map(element => {
      let val = new VendorContactDto();
      val.vendorId = Number(element.value.vendorId);
      val.name = String(element.value.name);
      val.email = String(element.value.email);
      val.telephone = String(element.value.telephone);
      return val;
    });

    this.httpService.POST(VendorContactsController.ImportVendorContacts, objectDtos)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {

        if (res.isPassed) {
          this.successRes = res.data;
          this.done = true;
          this.alertService.success('Uploaded success');
          this._ref.detectChanges();
        } else {
          this.loading = false;
          this.alertService.error(res.message)
          this._ref.detectChanges();
        }

      }, err => {
        this.alertService.exception();
        this.loading = false;
        this._ref.detectChanges();
      });

  }

  getRowNumber(i: number): number {
    const rowNumber = (this.paginator.pageIndex * this.paginator.pageSize) + (i + 1);
    return this.formatInt(rowNumber, false)
  }

}
