import { Component, OnInit, ViewChild, Injector, ViewEncapsulation, Input, ChangeDetectionStrategy } from '@angular/core';
import { TableColumn } from 'src/@core/models/common/response';
import { SelectionModel } from '@angular/cdk/collections';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { BaseService } from 'src/@core/services/base.service';
import { MatPaginator } from '@angular/material/paginator';
import { ProgramDto } from 'src/@core/models/program/Program';
import { ProgramsController } from 'src/@core/APIs/ProgramsController';
import { map, takeUntil } from 'rxjs/operators';
import { DiseaseDto } from 'src/@core/models/disease/Disease';
import { DiseasesController } from 'src/@core/APIs/DiseasesController';
import { FormValidationError } from 'src/@core/models/common/SharedModel';
import { TableVirtualScrollDataSource } from 'ng-table-virtual-scroll';
import { RxwebValidators } from '@rxweb/reactive-form-validators';
import { combineLatest } from 'rxjs';

@Component({
  selector: 'import-programs',
  templateUrl: './import-programs.component.html',
  styleUrls: ['./import-programs.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ImportProgramsComponent extends BaseService implements OnInit {
  // Drp
  diseases: DiseaseDto[] = [];
  // Table
  @Input('data') data: ProgramDto[] = [];
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  columns: TableColumn<ProgramDto>[] = [
    { label: 'Actions', property: 'actions', type: 'button', visible: true },
    { label: '#', property: 'index', type: 'custom', visible: true },
    { label: 'Disease', property: 'diseaseId', type: 'select', visible: true },
    { label: 'Name', property: 'name', type: 'text', visible: true },
    { label: 'Number of Years', property: 'numberOfYears', type: 'select', visible: true },
  ];
  dataSource = new TableVirtualScrollDataSource();
  selection = new SelectionModel<ProgramDto>(true, []);
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

    combineLatest([this.loadDiseases()])
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(([diseases]) => {
        // Set Drp data
        this.diseases = diseases;

        // Patch the Table Data
        this.formArry = Array.from(Array(this.data.length), (_, i) => i + 1).map(x => {
          let form = this.appendObject();

          // Disease
          this.data[x - 1].diseaseId = diseases.find(item => item.name.trim().toLowerCase() == this.data[x - 1].diseaseName.trim().toLowerCase())?.id;

          // Patch FormGroup Value
          form.patchValue(this.data[x - 1]);
          return form;
        });

        this.loadingData = false;
        this.dataSource = new TableVirtualScrollDataSource(this.formArry);
        this.dataSource.paginator = this.paginator;
      });

  }

  loadDiseases() {
    return this.httpService.GET(DiseasesController.GetAllAsDrp).pipe(map(res => res.data));
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
      diseaseId: [null, Validators.compose([Validators.required])],
      name: [null, Validators.compose([Validators.required, RxwebValidators.unique()])],
      numberOfYears: [null, Validators.compose([Validators.required])],
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
    let objectDtos: ProgramDto[] = [];

    objectDtos = this.formArry.map(element => {
      let val = new ProgramDto();
      val.name = String(element.value.name);
      val.diseaseId = Number(element.value.diseaseId);
      val.numberOfYears = Number(element.value.numberOfYears);
      return val;
    });

    this.httpService.POST(ProgramsController.ImportPrograms, objectDtos)
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
    const rowNumber = (this.paginator.pageIndex * this.paginator.pageSize) + (i+1);
    return this.formatInt(rowNumber , false)
  }
  
}
