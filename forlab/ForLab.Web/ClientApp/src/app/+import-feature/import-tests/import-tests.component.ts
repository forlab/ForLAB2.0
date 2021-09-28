import { Component, OnInit, ViewChild, Injector, ViewEncapsulation, Input, ChangeDetectionStrategy } from '@angular/core';
import { TableColumn } from 'src/@core/models/common/response';
import { SelectionModel } from '@angular/cdk/collections';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { BaseService } from 'src/@core/services/base.service';
import { MatPaginator } from '@angular/material/paginator';
import { TestDto } from 'src/@core/models/testing/Test';
import { TestsController } from 'src/@core/APIs/TestsController';
import { map, takeUntil } from 'rxjs/operators';
import { TestingAreaDto } from 'src/@core/models/lookup/TestingArea';
import { TestingAreasController } from 'src/@core/APIs/TestingAreasController';
import { FormValidationError } from 'src/@core/models/common/SharedModel';
import { TableVirtualScrollDataSource } from 'ng-table-virtual-scroll';
import { combineLatest } from 'rxjs';

@Component({
  selector: 'import-tests',
  templateUrl: './import-tests.component.html',
  styleUrls: ['./import-tests.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ImportTestsComponent extends BaseService implements OnInit {
  // Drp
  testingAreas: TestingAreaDto[] = [];
  // Table
  @Input('data') data: TestDto[] = [];
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  columns: TableColumn<TestDto>[] = [
    { label: 'Actions', property: 'actions', type: 'button', visible: true },
    { label: '#', property: 'index', type: 'custom', visible: true },
    { label: 'Name', property: 'name', type: 'text', visible: true },
    { label: 'Short Name', property: 'shortName', type: 'text', visible: true },
    { label: 'Testing Area', property: 'testingAreaId', type: 'select', visible: true },
  ];
  dataSource = new TableVirtualScrollDataSource();
  selection = new SelectionModel<TestDto>(true, []);
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

    combineLatest([this.loadTestingAreas()])
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(([testingAreas]) => {
        // Set Drp data
        this.testingAreas = testingAreas;

        // Patch the Table Data
        this.formArry = Array.from(Array(this.data.length), (_, i) => i + 1).map(x => {
          let form = this.appendObject();

          // TestingArea
          this.data[x - 1].testingAreaId = testingAreas.find(item => item.name.trim().toLowerCase() == this.data[x - 1].testingAreaName.trim().toLowerCase())?.id;

          // Patch FormGroup Value
          form.patchValue(this.data[x - 1]);
          return form;
        });

        this.loadingData = false;
        this.dataSource = new TableVirtualScrollDataSource(this.formArry);
        this.dataSource.paginator = this.paginator;
      });

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
      name: [null, Validators.compose([Validators.required])],
      testingAreaId: [null, Validators.compose([Validators.required])],
      shortName: [null, Validators.compose([Validators.required])],
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
    let objectDtos: TestDto[] = [];

    objectDtos = this.formArry.map(element => {
      let val = new TestDto();
      val.name = String(element.value.name);
      val.testingAreaId = Number(element.value.testingAreaId);
      val.shortName = String(element.value.shortName);
      return val;
    });

    this.httpService.POST(TestsController.ImportTests, objectDtos)
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
