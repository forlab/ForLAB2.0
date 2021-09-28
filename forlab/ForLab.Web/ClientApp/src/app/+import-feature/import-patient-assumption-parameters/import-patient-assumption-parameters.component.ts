import { Component, OnInit, ViewChild, Injector, ViewEncapsulation, Input, ChangeDetectionStrategy } from '@angular/core';
import { TableColumn } from 'src/@core/models/common/response';
import { SelectionModel } from '@angular/cdk/collections';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { BaseService } from 'src/@core/services/base.service';
import { MatPaginator } from '@angular/material/paginator';
import { PatientAssumptionParametersController } from 'src/@core/APIs/PatientAssumptionParametersController';
import { takeUntil, map } from 'rxjs/operators';
import { ProgramDto } from 'src/@core/models/program/Program';
import { ProgramsController } from 'src/@core/APIs/ProgramsController';
import { PatientAssumptionParameterDto } from 'src/@core/models/program/PatientAssumptionParameter';
import { FormValidationError } from 'src/@core/models/common/SharedModel';
import { TableVirtualScrollDataSource } from 'ng-table-virtual-scroll';
import { combineLatest } from 'rxjs';

@Component({
  selector: 'import-patient-assumption-parameters',
  templateUrl: './import-patient-assumption-parameters.component.html',
  styleUrls: ['./import-patient-assumption-parameters.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ImportPatientAssumptionParametersComponent extends BaseService implements OnInit {
  @Input('fromCreatingProgram') fromCreatingProgram: boolean = false;
  // Drp
  programs: ProgramDto[] = [];
  // Table
  @Input('data') data: PatientAssumptionParameterDto[] = [];
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  columns: TableColumn<PatientAssumptionParameterDto>[] = [
    { label: 'Actions', property: 'actions', type: 'button', visible: true },
    { label: '#', property: 'index', type: 'custom', visible: true },
    { label: 'Program', property: 'programId', type: 'select', visible: true },
    { label: 'Name', property: 'name', type: 'text', visible: true },
    { label: 'Type', property: 'isPercentage', type: 'select', visible: true },
    { label: 'Sign', property: 'isPositive', type: 'select', visible: true },
  ];
  dataSource = new TableVirtualScrollDataSource();
  selection = new SelectionModel<PatientAssumptionParameterDto>(true, []);
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
    this.checkFromCreatingProgram();

    combineLatest([this.loadPrograms()])
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(([programs]) => {
        // Set Drp data
        this.programs = programs;

        // Patch the Table Data
        this.formArry = Array.from(Array(this.data.length), (_, i) => i + 1).map(x => {
          let form = this.appendObject();

          // Program
          this.data[x - 1].programId = programs.find(item => item.name.trim().toLowerCase() == this.data[x - 1].programName.trim().toLowerCase())?.id;

          // Patch FormGroup Value
          form.patchValue(this.data[x - 1]);
          return form;
        });

        this.loadingData = false;
        this.dataSource = new TableVirtualScrollDataSource(this.formArry);
        this.dataSource.paginator = this.paginator;
      });

  }

  loadPrograms() {
    return this.httpService.GET(ProgramsController.GetAllAsDrp).pipe(map(res => res.data));
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
      name: [null, Validators.compose([Validators.required])],
      isPercentage: [false, Validators.compose([Validators.required])],
      isPositive: [true, Validators.compose([Validators.required])],
      programId: [0, Validators.compose([Validators.required])],
    })
  }

  addObject() {
    this.formArry.unshift(this.appendObject());
    this.dataSource = new TableVirtualScrollDataSource(this.formArry);
    this.dataSource.paginator = this.paginator;
    this.checkFromCreatingProgram();
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
    let objectDtos: PatientAssumptionParameterDto[] = [];

    objectDtos = this.formArry.map(element => {
      let val = new PatientAssumptionParameterDto();
      val.name = element.value.name;
      val.isPercentage = element.value.isPercentage;
      val.isNumeric = !element.value.isPercentage;
      val.isPositive = element.value.isPositive;
      val.isNegative = !element.value.isPositive;
      val.programId = Number(element.value.programId);
      return val;
    });

    this.httpService.POST(PatientAssumptionParametersController.ImportPatientAssumptionParameters, objectDtos)
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

  checkFromCreatingProgram() {
    if (this.fromCreatingProgram) {
      this.columns = this.columns.filter(x => x.property != 'programId');
      this.dataSource.paginator = this.paginator;
      this._ref.detectChanges();
    }
  }

  get isValid(): boolean {
    if (this.formArry?.length == 0) return true;
    /** check form */
    const invalidFormGroups = this.formArry?.filter(x => x.invalid);
    if (invalidFormGroups?.length > 0) {
      invalidFormGroups.forEach(x => x.markAllAsTouched());
      return false;
    }

    return true;
  }
  setErrors() {
    const invalidFormGroups = this.formArry?.filter(x => x.invalid);
    this.errors = this.getFormGroupsErrors(invalidFormGroups, this.formArry);
  }

  getRowNumber(i: number): number {
    const rowNumber = (this.paginator.pageIndex * this.paginator.pageSize) + (i+1);
    return this.formatInt(rowNumber , false)
  }

}
