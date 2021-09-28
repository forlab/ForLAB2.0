import { Component, OnInit, ViewChild, Injector, ViewEncapsulation, Input, ChangeDetectionStrategy } from '@angular/core';
import { TableColumn } from 'src/@core/models/common/response';
import { SelectionModel } from '@angular/cdk/collections';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { BaseService } from 'src/@core/services/base.service';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { InstrumentDto } from 'src/@core/models/product/Instrument';
import { InstrumentsController } from 'src/@core/APIs/InstrumentsController';
import { map, takeUntil } from 'rxjs/operators';
import { VendorDto } from 'src/@core/models/vendor/Vendor';
import { ThroughPutUnitEnum, ReagentSystemEnum, ControlRequirementUnitEnum } from 'src/@core/models/enum/Enums';
import { TestingAreaDto } from 'src/@core/models/lookup/TestingArea';
import { RxwebValidators } from '@rxweb/reactive-form-validators';
import { VendorsController } from 'src/@core/APIs/VendorsController';
import { TestingAreasController } from 'src/@core/APIs/TestingAreasController';
import { FormValidationError } from 'src/@core/models/common/SharedModel';
import { TableVirtualScrollDataSource } from 'ng-table-virtual-scroll';
import { combineLatest } from 'rxjs';

@Component({
  selector: 'import-instruments',
  templateUrl: './import-instruments.component.html',
  styleUrls: ['./import-instruments.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ImportInstrumentsComponent extends BaseService implements OnInit {
  // Drp
  vendors: VendorDto[] = [];
  testingAreas: TestingAreaDto[] = [];
  throughPutUnitEnum = ThroughPutUnitEnum;
  reagentSystemEnum = ReagentSystemEnum;
  controlRequirementUnitEnum = ControlRequirementUnitEnum;
  // Table
  @Input('data') data: InstrumentDto[] = [];
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort;
  columns: TableColumn<InstrumentDto>[] = [
    { label: 'Actions', property: 'actions', type: 'button', visible: true },
    { label: '#', property: 'index', type: 'custom', visible: true },
    { label: 'Name', property: 'name', type: 'text', visible: true },
    { label: 'Vendor', property: 'vendorId', type: 'select', visible: true },
    { label: 'Max Through Put', property: 'maxThroughPut', type: 'number', visible: true },
    { label: 'Through Put Unit', property: 'throughPutUnitId', type: 'select', visible: true },
    { label: 'Testing Area', property: 'testingAreaId', type: 'select', visible: true },
    { label: 'Reagent System', property: 'reagentSystemId', type: 'select', visible: true },
    { label: 'Control Requirement Unit', property: 'controlRequirementUnitId', type: 'select', visible: true },
  ];
  dataSource = new TableVirtualScrollDataSource();
  selection = new SelectionModel<InstrumentDto>(true, []);
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

    combineLatest([this.loadVendors(), this.loadTestingAreas()])
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(([vendors, testingAreas]) => {
        // Convert Enum to List
        const throughPutUnits = this.convertEnumToList(this.throughPutUnitEnum);
        const reagentSystems = this.convertEnumToList(this.reagentSystemEnum);
        const controlRequirementUnits = this.convertEnumToList(this.controlRequirementUnitEnum);
        // Set Drp data
        this.vendors = vendors;
        this.testingAreas = testingAreas;

        // Patch the Table Data
        this.formArry = Array.from(Array(this.data.length), (_, i) => i + 1).map(x => {
          let form = this.appendObject();

          // Vendor
          this.data[x - 1].vendorId = vendors.find(item => item.name.trim().toLowerCase() == this.data[x - 1].vendorName.trim().toLowerCase())?.id;
          // TestingArea
          this.data[x - 1].testingAreaId = testingAreas.find(item => item.name.trim().toLowerCase() == this.data[x - 1].testingAreaName.trim().toLowerCase())?.id;
          // ThroughPutUnit
          this.data[x - 1].throughPutUnitId = throughPutUnits.find(item => item.name.trim().toLowerCase() == this.data[x - 1].throughPutUnitName.trim().toLowerCase())?.id;
          // ReagentSystem
          this.data[x - 1].reagentSystemId = reagentSystems.find(item => item.name.trim().toLowerCase() == this.data[x - 1].reagentSystemName.trim().toLowerCase())?.id;
          // ControlRequirementUnit
          this.data[x - 1].controlRequirementUnitId = controlRequirementUnits.find(item => item.name.trim().toLowerCase() == this.data[x - 1].controlRequirementUnitName.trim().toLowerCase())?.id;

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
  loadTestingAreas() {
    return this.httpService.GET(TestingAreasController.GetAllAsDrp).pipe(map(res => res.data));
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
      name: new FormControl(null, [Validators.required, RxwebValidators.unique()]),
      vendorId: new FormControl(null, [Validators.required]),
      maxThroughPut: new FormControl(null, [Validators.required]),
      throughPutUnitId: new FormControl(null, [Validators.required]),
      reagentSystemId: new FormControl(null, [Validators.required]),
      controlRequirement: new FormControl(null, [Validators.required]),
      controlRequirementUnitId: new FormControl(null, [Validators.required]),
      testingAreaId: new FormControl(null, [Validators.required]),
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
    let objectDtos: InstrumentDto[] = [];

    objectDtos = this.formArry.map(element => {
      let val = new InstrumentDto();
      val.name = String(element.value.name);
      val.vendorId = Number(element.value.vendorId);
      val.maxThroughPut = Number(element.value.maxThroughPut);
      val.throughPutUnitId = Number(element.value.throughPutUnitId);
      val.reagentSystemId = Number(element.value.reagentSystemId);
      val.controlRequirement = Number(element.value.controlRequirement);
      val.controlRequirementUnitId = Number(element.value.controlRequirementUnitId);
      val.testingAreaId = Number(element.value.testingAreaId);
      return val;
    });

    this.httpService.POST(InstrumentsController.ImportInstruments, objectDtos)
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
