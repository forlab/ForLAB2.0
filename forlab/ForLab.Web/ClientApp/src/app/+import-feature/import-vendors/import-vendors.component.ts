import { Component, OnInit, ViewChild, Injector, ViewEncapsulation, Input, ChangeDetectionStrategy } from '@angular/core';
import { TableColumn } from 'src/@core/models/common/response';
import { SelectionModel } from '@angular/cdk/collections';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { BaseService } from 'src/@core/services/base.service';
import { MatPaginator } from '@angular/material/paginator';
import { VendorDto } from 'src/@core/models/vendor/Vendor';
import { VendorsController } from 'src/@core/APIs/VendorsController';
import { takeUntil } from 'rxjs/operators';
import { RxwebValidators } from '@rxweb/reactive-form-validators';
import { FormValidationError } from 'src/@core/models/common/SharedModel';
import { TableVirtualScrollDataSource } from 'ng-table-virtual-scroll';

@Component({
  selector: 'import-vendors',
  templateUrl: './import-vendors.component.html',
  styleUrls: ['./import-vendors.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ImportVendorsComponent extends BaseService implements OnInit {

  // Table
  @Input('data') data: VendorDto[];
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  columns: TableColumn<VendorDto>[] = [
    { label: 'Actions', property: 'actions', type: 'button', visible: true },
    { label: '#', property: 'index', type: 'custom', visible: true },
    { label: 'Name', property: 'name', type: 'text', visible: true },
    { label: 'Telephone', property: 'telephone', type: 'text', visible: true },
    { label: 'Email', property: 'email', type: 'text', visible: true },
    { label: 'Address', property: 'address', type: 'text', visible: true },
    { label: 'URL', property: 'url', type: 'text', visible: true },
  ];
  dataSource = new TableVirtualScrollDataSource();
  selection = new SelectionModel<VendorDto>(true, []);
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
    // Patch the Table Data
    this.formArry = Array.from(Array(this.data.length), (_, i) => i + 1).map(x => {
      let form = this.appendObject();
      // Patch FormGroup Value
      form.patchValue(this.data[x - 1]);
      return form;
    });

    this.loadingData = false;
    this.dataSource = new TableVirtualScrollDataSource(this.formArry);
    setTimeout(() => { this.dataSource.paginator = this.paginator; })
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
      name: [null, Validators.compose([Validators.required, RxwebValidators.unique()])],
      email: [null, Validators.compose([Validators.required, Validators.email, RxwebValidators.unique()])],
      telephone: [null, Validators.compose([Validators.required])],
      url: [null, Validators.compose([RxwebValidators.url()])],
      address: [null, Validators.compose([])],
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
    let objectDtos: VendorDto[] = [];

    objectDtos = this.formArry.map(element => {
      let val = new VendorDto();
      val.name = String(element.value.name);
      val.email = String(element.value.email);
      val.telephone = String(element.value.telephone);
      val.url = String(element.value.url);
      val.address = String(element.value.address);
      return val;
    });

    this.httpService.POST(VendorsController.ImportVendors, objectDtos)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {

        if (res.isPassed) {
          this.successRes = res.data;
          this.done = true;
          this.alertService.success('Uploaded success');
          this._ref.detectChanges();
        } else {
          if (res.data) {
            this.errors = res.data;
          }
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
