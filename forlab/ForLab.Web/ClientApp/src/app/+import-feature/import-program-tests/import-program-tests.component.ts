import { Component, OnInit, ViewChild, Injector, ViewEncapsulation, Input, ChangeDetectionStrategy } from '@angular/core';
import { TableColumn } from 'src/@core/models/common/response';
import { SelectionModel } from '@angular/cdk/collections';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { BaseService } from 'src/@core/services/base.service';
import { MatPaginator } from '@angular/material/paginator';
import { ProgramTestDto } from 'src/@core/models/program/ProgramTest';
import { ProgramTestsController } from 'src/@core/APIs/ProgramTestsController';
import { map, takeUntil } from 'rxjs/operators';
import { ProgramDto } from 'src/@core/models/program/Program';
import { ProgramsController } from 'src/@core/APIs/ProgramsController';
import { TestDto } from 'src/@core/models/testing/Test';
import { TestsController } from 'src/@core/APIs/TestsController';
import { RxwebValidators } from '@rxweb/reactive-form-validators';
import { PatientGroupDto } from 'src/@core/models/lookup/PatientGroup';
import { CalculationPeriodEnum } from 'src/@core/models/enum/Enums';
import { PatientGroupsController } from 'src/@core/APIs/PatientGroupsController';
import { PeriodMonthsPopupComponent } from './period-months-popup/period-months-popup.component';
import { FormValidationError } from 'src/@core/models/common/SharedModel';
import { TableVirtualScrollDataSource } from 'ng-table-virtual-scroll';
import { combineLatest } from 'rxjs';

@Component({
  selector: 'import-program-tests',
  templateUrl: './import-program-tests.component.html',
  styleUrls: ['./import-program-tests.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ImportProgramTestsComponent extends BaseService implements OnInit {
  @Input('fromCreatingProgram') fromCreatingProgram: boolean = false;
  // Drp
  tests: TestDto[] = [];
  patientGroups: PatientGroupDto[] = [];
  programs: ProgramDto[] = [];
  calculationPeriodEnum = CalculationPeriodEnum;
  // Table
  @Input('data') data: ProgramTestDto[] = [];
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  columns: TableColumn<ProgramTestDto>[] = [
    { label: 'Actions', property: 'actions', type: 'button', visible: true },
    { label: '#', property: 'index', type: 'custom', visible: true },
    { label: 'Patient Group', property: 'patientGroupId', type: 'select', visible: true },
    { label: 'Test', property: 'testId', type: 'select', visible: true },
    { label: 'Testing Protocol', property: 'testingProtocolName', type: 'text', visible: true },
    { label: 'Base Line', property: 'baseLine', type: 'number', visible: true },
    { label: 'Test After First Year', property: 'testAfterFirstYear', type: 'number', visible: true },
  ];
  dataSource = new TableVirtualScrollDataSource();
  selection = new SelectionModel<ProgramTestDto>(true, []);
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

    combineLatest([this.loadTests(), this.loadPatientGroups(), this.loadPrograms()])
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(([tests, patientGroups, programs]) => {
        // Convert Enum to List
        const calculationPeriods = this.convertEnumToList(this.calculationPeriodEnum);
        // Set Drp data
        this.tests = tests;
        this.patientGroups = patientGroups;
        this.programs = programs;

        // Patch the Table Data
        this.formArry = Array.from(Array(this.data.length), (_, i) => i + 1).map(x => {
          let form = this.appendObject();

          // CalculationPeriods
          this.data[x - 1].testingProtocolDto.calculationPeriodId = calculationPeriods.find(item => item.name.trim().toLowerCase() == this.data[x - 1].testingProtocolDto?.calculationPeriodName.trim().toLowerCase())?.id;
          // PatientGroup
          this.data[x - 1].testingProtocolDto.patientGroupId = this.patientGroups.find(item => item.name.trim().toLowerCase() == this.data[x - 1].testingProtocolDto?.patientGroupName.trim().toLowerCase())?.id;
          // Test
          this.data[x - 1].testId = this.tests.find(item => item.name.trim().toLowerCase() == this.data[x - 1].testName.trim().toLowerCase())?.id;
          // Programs
          this.data[x - 1].programId = this.programs.find(item => item.name.trim().toLowerCase() == this.data[x - 1].programName.trim().toLowerCase())?.id;

          // Patch FormGroup Value
          form.patchValue(this.data[x - 1]);
          return form;
        });

        this.loadingData = false;
        this.dataSource = new TableVirtualScrollDataSource(this.formArry);
        this.dataSource.paginator = this.paginator;
      });

  }

  loadTests() {
    return this.httpService.GET(TestsController.GetAllAsDrp).pipe(map(res => res.data));
  }
  loadPatientGroups() {
    return this.httpService.GET(PatientGroupsController.GetAllAsDrp).pipe(map(res => res.data));
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
      programId: [0, Validators.compose([Validators.required])],
      testId: [null, Validators.compose([Validators.required])],
      // Testing Protocol
      testingProtocolName: [null, Validators.compose([Validators.required, RxwebValidators.unique()])],
      patientGroupId: [null, Validators.compose([Validators.required, RxwebValidators.unique()])],
      baseLine: [null, Validators.compose([Validators.required])],
      testAfterFirstYear: [null, Validators.compose([Validators.required])],
      // calculationPeriodId: [null, Validators.compose([Validators.required])],
    })
  }

  addObject() {
    this.formArry.unshift(this.appendObject());
    this.dataSource = new TableVirtualScrollDataSource(this.formArry);
    this.dataSource.paginator = this.paginator;
    // Check
    this.data.unshift(new ProgramTestDto());
    this.checkFromCreatingProgram();
  }

  deleteObject(row) {
    const index = this.formArry.indexOf(row);
    this.formArry.splice(index, 1);
    this.dataSource = new TableVirtualScrollDataSource(this.formArry);
    this.dataSource.paginator = this.paginator;
    // Check
    this.data.splice(index, 1);
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
    let objectDtos: ProgramTestDto[] = [];

    objectDtos = this.formArry.map(element => {
      let val = new ProgramTestDto();
      val.testId = Number(element.value.testId);
      val.testingProtocolId = Number(element.value.testingProtocolId);
      val.programId = Number(element.value.programId);
      return val;
    });

    this.httpService.POST(ProgramTestsController.ImportProgramTests, objectDtos)
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

  // Open months popup
  openPeriodMonthPopup(form: FormGroup, index: number) {
    this.dialog.open(PeriodMonthsPopupComponent, { data: this.data[index] })
      .afterClosed()
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe((newProgramTest: ProgramTestDto) => {
        if (newProgramTest) {
          this.data[index].testingProtocolDto = newProgramTest.testingProtocolDto;
        }
      });
  }

  getRowNumber(i: number): number {
    const rowNumber = (this.paginator.pageIndex * this.paginator.pageSize) + (i+1);
    return this.formatInt(rowNumber , false)
  }

}
