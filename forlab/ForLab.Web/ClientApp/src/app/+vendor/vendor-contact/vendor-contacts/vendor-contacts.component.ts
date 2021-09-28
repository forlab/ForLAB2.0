import { Component, ElementRef, OnInit, ViewChild, Injector, Input } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { fromEvent, Observable } from 'rxjs';
import { SelectionModel } from '@angular/cdk/collections';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { ConfirmDeleteComponent } from 'src/@core/directives/confirm-delete/confirm-delete.component';
import { VendorContactDto, VendorContactFilterDto } from 'src/@core/models/vendor/VendorContact';
import { TableColumn } from 'src/@core/models/common/response';
import { VendorContactsController } from 'src/@core/APIs/VendorContactsController';
import { ObjectDataSource } from 'src/@core/services/object.datasource';
import { takeUntil, tap, debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { ConfirmActiveComponent } from 'src/@core/directives/confirm-active/confirm-active.component';
import { BaseService } from 'src/@core/services/base.service';
import { AddEditVendorContactComponent } from '../add-edit-vendor-contact/add-edit-vendor-contact.component';
import { VendorsController } from 'src/@core/APIs/VendorsController';
import { ConfirmExportComponent } from 'src/@core/directives/confirm-export/confirm-export.component';

@Component({
  selector: 'app-vendor-contacts',
  templateUrl: './vendor-contacts.component.html',
  styleUrls: ['./vendor-contacts.component.scss']
})
export class VendorContactsComponent extends BaseService implements OnInit {
  // Inputs to reuse the component in other components
  @Input('fromVendorDetails') fromVendorDetails: boolean = false;
  @Input('vendorId') vendorId: number;
  @Input('vendorCreatedBy') vendorCreatedBy: number;

  // Drp
  vendors$: Observable<any[]>;
  
  filterDto: VendorContactFilterDto = new VendorContactFilterDto();

  // Table
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort;
  @ViewChild('searchInput', { static: false }) searchInput: ElementRef;
  columns: TableColumn<VendorContactDto>[] = [
    { label: 'Actions', property: 'actions', type: 'button', visible: true },
    // { label: 'Select', property: 'select', type: 'button', visible: false },
    { label: 'Status', property: 'isActive', type: 'button', visible: true },
    { label: 'Vendor', property: 'vendorName', type: 'text', visible: true },
    { label: 'Name', property: 'name', type: 'text', visible: true },
    { label: 'Telephone', property: 'telephone', type: 'text', visible: true },
    { label: 'Email', property: 'email', type: 'text', visible: true },
    { label: 'Creator', property: 'creator', type: 'text', visible: false },
    { label: 'Created On', property: 'createdOn', type: 'datetime', visible: false },
  ];
  dataSource: ObjectDataSource;
  selection = new SelectionModel<VendorContactDto>(true, []);
  skippedOnReset: string[] = [];

  constructor(public injector: Injector) {
    super(injector);

  }

  ngOnInit() {

    // Check
    if(this.fromVendorDetails) {
      const index = this.columns.findIndex(x => x.property == 'vendorName');
      this.columns.splice(index, 1);
      this.filterDto.vendorId = this.vendorId;
      this.skippedOnReset.push('vendorId');
    }
    
    // Load Drp
    if(!this.fromVendorDetails) {
      this.loadVendors();
    }

    this.dataSource = new ObjectDataSource(this.httpService);
  }

  ngAfterViewInit() {

    this.loadData();
    this.sort.sortChange.subscribe((res) => {
      this.paginator.pageIndex = 0;
      this.loadData();
    });
    
    this.paginator.page.pipe(tap(() => this.loadData())).subscribe();

    fromEvent(this.searchInput.nativeElement, 'keyup')
      .pipe(
        debounceTime(150),
        distinctUntilChanged(),
        tap(() => {
          this.paginator.pageIndex = 0;
          this.loadData();
        })
      )
      .subscribe();

  }

  loadVendors() {
    this.vendors$ = this.httpService.GET(VendorsController.GetAllAsDrp).pipe(map(res => res.data));
  }
  
  toggleColumnVisibility(column, event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    column.visible = !column.visible;
  }

  trackByProperty<T>(index: number, column: TableColumn<T>) {
    return column.property;
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  loadData() {

    this.selection.clear();

    let params: QueryParamsDto[] = [
      { key: 'pageSize', value: this.paginator.pageSize || 10 },
      { key: 'pageIndex', value: this.paginator.pageIndex + 1 || 1 },
      { key: 'applySort', value: this.sort && this.sort.active ? true : false },
      { key: 'sortProperty', value: this.sort && this.sort.active ? this.sort.active : null },
      { key: 'isAscending', value: this.sort && this.sort.direction == 'asc' ? true : false },
      // Filter
      { key: 'name', value: this.searchInput.nativeElement.value || this.filterDto.name },
    ];

    // Append FilterDto values to the paramaters
    params = params.concat(this.getFilterParamsDtos(this.filterDto));

    this.dataSource.loadObjects(VendorContactsController.GetAll, params);
  }
  /** Whether the number of selected elements matches the total number of rows. */
  isAllSelected() {
    const numSelected = this.selection.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }
  /** Selects all rows if they are not all selected; otherwise clear selection. */
  masterToggle() {
    if (this.isAllSelected()) {
      this.selection.clear();
    } else {
      this.selection.clear();
      this.dataSource.data.forEach(row => this.selection.select(row));
    }
  }

  reset() {
    if (this.filterDto != null) {
      Object.keys(this.filterDto).forEach(key => {
        if (!this.skippedOnReset.includes(key)) {
          this.filterDto[key] = null;
        }
      });

      // Reset page
      this.paginator.pageIndex = 0;
      // Reload the data
      this.loadData();
    }
  }

  createObject() {
    this.dialog.open(AddEditVendorContactComponent, {
      data: {
        vendorId: this.vendorId,
        fromVendorDetails: this.fromVendorDetails,
        vendorContactDto: new VendorContactDto()
      }
    })
      .afterClosed()
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res) {
          this.loadData();
        }
      });
  }

  updateObject(vendorContact: VendorContactDto) {
    this.dialog.open(AddEditVendorContactComponent, {
      data: {
        vendorId: this.vendorId,
        fromVendorDetails: this.fromVendorDetails,
        vendorContactDto: vendorContact
      }
    })
      .afterClosed()
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res) {
          this.loadData();
        }
      });
  }

  updateIsActive(vendorContact: VendorContactDto) {
    this.dialog.open(ConfirmActiveComponent, {
      data: {
        url: VendorContactsController.UpdateIsActive,
        objectInfo: [{ key: 'Contact Name', value: vendorContact.name }, { key: 'Contact Email', value: vendorContact.email }, { key: 'Vendor', value: vendorContact.vendorName }],
        isActive: !vendorContact.isActive,
        queryParamsDto: { key: 'vendorContactId', value: vendorContact.id },
      }
    })
      .afterClosed()
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res) {
          this.loadData();
        }
      });
  }

  deleteObject(vendorContact: VendorContactDto) {
    this.dialog.open(ConfirmDeleteComponent, {
      data: {
        url: VendorContactsController.RemoveVendorContact,
        objectInfo: [{ key: 'Contact Name', value: vendorContact.name }, { key: 'Contact Email', value: vendorContact.email }, { key: 'Vendor', value: vendorContact.vendorName }],
        queryParamsDto: { key: 'vendorContactId', value: vendorContact.id },
      }
    })
      .afterClosed()
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res) {
          this.loadData();
        }
      });
  }

  export() {
    let params: QueryParamsDto[] = [
      { key: 'pageSize', value: this.paginator.pageSize || 10 },
      { key: 'pageIndex', value: this.paginator.pageIndex + 1 || 1 },
      { key: 'applySort', value: this.sort && this.sort.active ? true : false },
      { key: 'sortProperty', value: this.sort && this.sort.active ? this.sort.active : null },
      { key: 'isAscending', value: this.sort && this.sort.direction == 'asc' ? true : false },
      // Filter
      { key: 'name', value: this.searchInput.nativeElement.value || this.filterDto.name },
    ];

    // Append FilterDto values to the paramaters
    params = params.concat(this.getFilterParamsDtos(this.filterDto));

    this.dialog.open(ConfirmExportComponent, {
      data: {
        url: VendorContactsController.ExportVendorContacts,
        fileName: 'VendorContacts',
        queryParamsDtos: params,
      }
    });
  }
  
}
