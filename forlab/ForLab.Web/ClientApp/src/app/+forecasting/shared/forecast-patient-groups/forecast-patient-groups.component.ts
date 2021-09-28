import { Component, OnInit, ViewChild, Injector, ElementRef, Input } from '@angular/core';
import { TableColumn, QueryParamsDto } from 'src/@core/models/common/response';
import { MatTableDataSource } from '@angular/material/table';
import { SelectionModel } from '@angular/cdk/collections';
import { BaseService } from 'src/@core/services/base.service';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { ForecastPatientGroupDto } from 'src/@core/models/forecasting/ForecastPatientGroup';
import { RxwebValidators } from '@rxweb/reactive-form-validators';
import { takeUntil } from 'rxjs/operators';
import { ProgramDto } from 'src/@core/models/program/Program';
import { ProgramTestsController } from 'src/@core/APIs/ProgramTestsController';
import { ProgramTestDto } from 'src/@core/models/program/ProgramTest';

@Component({
  selector: 'forecast-patient-groups',
  templateUrl: './forecast-patient-groups.component.html',
  styleUrls: ['./forecast-patient-groups.component.scss']
})
export class ForecastPatientGroupsComponent extends BaseService implements OnInit {
  // Inputs
  @Input('programDtos') programDtos: ProgramDto[] = [];
  // Table
  @Input('data') data: ForecastPatientGroupDto[] = [];
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort;
  // @ViewChild('searchInput', { static: false }) searchInput: ElementRef;
  columns: TableColumn<ForecastPatientGroupDto>[] = [
    // { label: 'Actions', property: 'actions', type: 'button', visible: true },
    { label: 'Program', property: 'programName', type: 'text', visible: true },
    { label: 'Patient Group', property: 'patientGroupName', type: 'text', visible: true },
    { label: 'Percentage', property: 'percentage', type: 'number', visible: true },
  ];
  dataSource: MatTableDataSource<any> | null = new MatTableDataSource([]);
  selection = new SelectionModel<ForecastPatientGroupDto>(true, []);
  // Form
  searchForm: FormGroup;
  formItem = this.fb.group({});
  // vars
  successRes: any;
  done: boolean = false;
  arrayBuffer: any;
  file: File;
  lengthOfArray: number;
  // Drp
  programPatientGroups: any[] = [];

  constructor(private fb: FormBuilder, public injector: Injector) {
    super(injector);
    this.searchForm = this.createFormItem();
  }


  ngOnInit() {
    if (this.columns.length > 0) {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    }
    this.patch(this.data);
    setTimeout(() => this.dataSource.paginator = this.paginator);
  }


  ngOnChanges() {
    this.loadProgramPatientGroups();
  }

  loadProgramPatientGroups() {
    const programIds = this.programDtos.map(x => x.id);
    if (programIds.length == 0) {
      return;
    }

    let params: QueryParamsDto[] = [
      { key: 'filterByProgramIds', value: true },
      { key: 'programIds', value: programIds.join(',') },
    ];

    this.httpService.GET(ProgramTestsController.GetAllAsDrp, params)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(
        (res) => {
          if (res.isPassed) {
            this.loading = false;
            let resData = res.data as ProgramTestDto[];
            // handle new data
            resData.forEach(x => {
              if (this.data.findIndex(y => y.patientGroupId == x.testingProtocolPatientGroupId && y.programId == x.programId) == -1) {
                let val = new ForecastPatientGroupDto();
                val.programId = x.programId;
                val.patientGroupId = x.testingProtocolPatientGroupId;
                val.programName = x.programName;
                val.patientGroupName = x.testingProtocolPatientGroupName;
                this.data.push(val)
              }
            });
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

  get visibleColumns() {
    if (!this.columns) {
      return [];
    }
    return this.columns.filter(column => column.visible).map(column => column.property);
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

  patch(data: ForecastPatientGroupDto[]) {
    for (let index = 0; index < data.length; index++) {
      this.addObject();
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
      programName: [null, Validators.compose([])],
      patientGroupName: [null, Validators.compose([])],
      percentage: [null, Validators.compose([RxwebValidators.maxNumber({ value: 100 }), RxwebValidators.minNumber({ value: 0 })])],
    })
  }

  getObjects(): FormArray {
    return this.searchForm.get('objects') as FormArray;
  }

  addObject() {
    this.getObjects().insert(0, this.appendObject());
    this.dataSource = new MatTableDataSource((this.searchForm.get('objects') as FormArray).controls);
    this.dataSource.paginator = this.paginator;
  }

  deleteObject(objectIndex: number) {
    this.getObjects().removeAt(objectIndex);
    this.dataSource = new MatTableDataSource((this.searchForm.get('objects') as FormArray).controls);
    this.dataSource.paginator = this.paginator;
  }

  get isValid(): boolean {
    if (this.getObjects().controls.length == 0) return false;
    return this.searchForm.valid;
  }

  get getFinalForecastPatientGroup(): ForecastPatientGroupDto[] {
    return this.getObjects().controls.map(element => {
      let val = new ForecastPatientGroupDto();
      val.patientGroupId = this.data.find(x => x.patientGroupName == element.value.patientGroupName).patientGroupId;
      val.programId = this.data.find(x => x.programName == element.value.programName).programId;
      val.percentage = Number(element.value.percentage);
      return val;
    });
  }

}
