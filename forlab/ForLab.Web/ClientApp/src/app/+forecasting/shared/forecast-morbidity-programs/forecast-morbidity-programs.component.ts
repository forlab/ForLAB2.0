import { Component, OnInit, ViewChild, Injector, ElementRef, Input, EventEmitter, Output } from '@angular/core';
import { TableColumn } from 'src/@core/models/common/response';
import { MatTableDataSource } from '@angular/material/table';
import { SelectionModel } from '@angular/cdk/collections';
import { BaseService } from 'src/@core/services/base.service';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { takeUntil } from 'rxjs/operators';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { ForecastMorbidityProgramDto } from 'src/@core/models/forecasting/ForecastMorbidityProgram';
import { ProgramDto } from 'src/@core/models/program/Program';
import { ProgramsController } from 'src/@core/APIs/ProgramsController';
import { RxwebValidators } from '@rxweb/reactive-form-validators';
import { FormValidationError } from 'src/@core/models/common/SharedModel';

@Component({
  selector: 'forecast-morbidity-programs',
  templateUrl: './forecast-morbidity-programs.component.html',
  styleUrls: ['./forecast-morbidity-programs.component.scss']
})
export class ForecastMorbidityProgramsComponent extends BaseService implements OnInit {

  // Input & Outputs
  @Output('notify') notify = new EventEmitter<ProgramDto[]>();
  // Table
  @Input('data') data: ForecastMorbidityProgramDto[] = [];
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort;
  // @ViewChild('searchInput', { static: false }) searchInput: ElementRef;
  columns: TableColumn<ForecastMorbidityProgramDto>[] = [
    { label: 'Actions', property: 'actions', type: 'button', visible: true },
    { label: 'Program', property: 'programId', type: 'select', visible: true },
  ];
  dataSource: MatTableDataSource<any> | null = new MatTableDataSource([]);
  selection = new SelectionModel<ForecastMorbidityProgramDto>(true, []);
  // Form
  searchForm: FormGroup;
  formItem = this.fb.group({});
  // vars
  errors: FormValidationError[] = [];
  successRes: any;
  done: boolean = false;
  arrayBuffer: any;
  file: File;
  lengthOfArray: number;
  // Drp
  programs: ProgramDto[] = [];

  constructor(private fb: FormBuilder, public injector: Injector) {
    super(injector);
    this.searchForm = this.createFormItem();
  }

  get visibleColumns() {
    if (!this.columns) {
      return [];
    }
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  ngOnInit() {

    // Load Drp
    this.loadPrograms();

    if (this.columns.length > 0) {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    }
    this.patch(this.data);
    setTimeout(() => this.dataSource.paginator = this.paginator);

    this.searchForm.valueChanges
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => this.notify.emit(this.getAddedPrograms));
  }

  loadPrograms() {
    this.httpService.GET(ProgramsController.GetAllAsDrp)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res.isPassed) {
          this.loading = false;
          this.programs = res.data;
          // Refresh Table
          this.getObjects().clear();
          this.dataSource.paginator = this.paginator;
          this.patch(this.data);
          this._ref.detectChanges();
        } else {
          this.alertService.error(res.message);
          this.loading = false;
          this._ref.detectChanges();
        }
      }, err => {
        this.alertService.exception();
        this.loading = false;
        this._ref.detectChanges();
      });
  }

  onFilterChange(filterValue: string) {
    filterValue = filterValue.trim();
    filterValue = filterValue.toLowerCase();
    this.dataSource.filter = filterValue;
  }

  toggleColumnVisibility(column, event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    column.visible = !column.visible;
  }

  trackByProperty<T>(index: number, column: TableColumn<T>) {
    return column.property;
  }

  patch(data: ForecastMorbidityProgramDto[]) {
    for (let index = 0; index < data.length; index++) {
      this.addObject();
    }

    // fill dropdown
    if (data.length > 0) {
      data.forEach((x, i) => {
        // Programs
        if (this.programs && this.programs.length > 0 && this.data[i].programName) {
          const program = this.programs.find(x => x.name.trim().toLowerCase() == this.data[i].programName.trim().toLowerCase());
          this.data[i].programId = program ? program.id : null;
        }
      });
    }

    this.searchForm.get('objects').patchValue(this.data);
    this._ref.detectChanges();
  }

  createFormItem(): FormGroup {
    return this.formItem = this.fb.group({
      objects: this.fb.array([])
    });
  }

  appendObject() {
    return this.formItem = this.fb.group({
      programId: [null, Validators.compose([Validators.required, RxwebValidators.unique()])],
    })
  }

  getObjects(): FormArray {
    return this.searchForm.get('objects') as FormArray;
  }

  addObject() {
    this.getObjects().insert(0, this.appendObject());
    this.dataSource = new MatTableDataSource((this.searchForm.get('objects') as FormArray).controls);
    this.dataSource.paginator = this.paginator;
    this.notify.emit(this.getAddedPrograms);
  }

  deleteObject(objectIndex: number) {
    this.getObjects().removeAt(objectIndex);
    this.dataSource = new MatTableDataSource((this.searchForm.get('objects') as FormArray).controls);
    this.dataSource.paginator = this.paginator;
    this.notify.emit(this.getAddedPrograms);
  }

  get getAddedPrograms(): ProgramDto[] {
    if (this.getObjects().length == 0) return [];
    let programIds = this.getObjects().controls.filter(x => Number(x.value.programId) > 0).map(x => Number(x.value.programId));
    return this.programs.filter(x => programIds.includes(x.id));
  }

  get isValid(): boolean {
    if (this.getObjects().controls.length == 0) return false;
    /** check form */
    if (this.searchForm.invalid) {
      this.searchForm.markAllAsTouched();
      return false;
    }
    return true;
  }
  setErrors() {
    this.errors = this.getFormValidationErrors(this.getObjects());
  }

}
