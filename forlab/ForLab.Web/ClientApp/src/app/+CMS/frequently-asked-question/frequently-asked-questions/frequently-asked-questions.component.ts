import { Component, ElementRef, OnInit, ViewChild, Injector } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { fromEvent } from 'rxjs';
import { SelectionModel } from '@angular/cdk/collections';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { ConfirmDeleteComponent } from 'src/@core/directives/confirm-delete/confirm-delete.component';
import { FrequentlyAskedQuestionDto, FrequentlyAskedQuestionFilterDto } from 'src/@core/models/CMS/FrequentlyAskedQuestion';
import { TableColumn } from 'src/@core/models/common/response';
import { FrequentlyAskedQuestionsController } from 'src/@core/APIs/FrequentlyAskedQuestionsController';
import { ObjectDataSource } from 'src/@core/services/object.datasource';
import { takeUntil, tap, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ConfirmActiveComponent } from 'src/@core/directives/confirm-active/confirm-active.component';
import { BaseService } from 'src/@core/services/base.service';
import { AddEditFrequentlyAskedQuestionComponent } from '../add-edit-frequently-asked-question/add-edit-frequently-asked-question.component';

@Component({
  selector: 'app-frequently-asked-questions',
  templateUrl: './frequently-asked-questions.component.html',
  styleUrls: ['./frequently-asked-questions.component.scss']
})
export class FrequentlyAskedQuestionsComponent extends BaseService implements OnInit {

  filterDto: FrequentlyAskedQuestionFilterDto = new FrequentlyAskedQuestionFilterDto();

  // Table
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort;
  @ViewChild('searchInput', { static: false }) searchInput: ElementRef;
  columns: TableColumn<FrequentlyAskedQuestionDto>[] = [
    { label: 'Actions', property: 'actions', type: 'button', visible: true },
    // { label: 'Select', property: 'select', type: 'button', visible: false },
    { label: 'Status', property: 'isActive', type: 'button', visible: true },
    { label: 'Question', property: 'question', type: 'text', visible: true },
    { label: 'Creator', property: 'creator', type: 'text', visible: true },
    { label: 'Created On', property: 'createdOn', type: 'datetime', visible: true },
  ];
  dataSource: ObjectDataSource;
  selection = new SelectionModel<FrequentlyAskedQuestionDto>(true, []);

  constructor(public injector: Injector) {
    super(injector);
  }


  ngOnInit() {
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
      { key: 'question', value: this.searchInput.nativeElement.value },
    ];

    this.dataSource.loadObjects(FrequentlyAskedQuestionsController.GetAll, params);
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
    let skipped: string[] = [];

    if (this.filterDto != null) {
      Object.keys(this.filterDto).forEach(key => {
        if (!skipped.includes(key)) {
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
    this.dialog.open(AddEditFrequentlyAskedQuestionComponent)
      .afterClosed()
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res) {
          this.loadData();
        }
      });
  }

  updateObject(frequentlyAskedQuestion: FrequentlyAskedQuestionDto) {
    this.dialog.open(AddEditFrequentlyAskedQuestionComponent, {
      data: frequentlyAskedQuestion
    })
      .afterClosed()
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res) {
          this.loadData();
        }
      });
  }

  updateIsActive(frequentlyAskedQuestion: FrequentlyAskedQuestionDto) {
    this.dialog.open(ConfirmActiveComponent, {
      data: {
        url: FrequentlyAskedQuestionsController.UpdateIsActive,
        objectInfo: [{ key: 'Question', value: frequentlyAskedQuestion.question }],
        isActive: !frequentlyAskedQuestion.isActive,
        queryParamsDto: { key: 'frequentlyAskedQuestionId', value: frequentlyAskedQuestion.id },
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

  deleteObject(frequentlyAskedQuestion: FrequentlyAskedQuestionDto) {
    this.dialog.open(ConfirmDeleteComponent, {
      data: {
        url: FrequentlyAskedQuestionsController.RemoveFrequentlyAskedQuestion,
        objectInfo: [{ key: 'Question', value: frequentlyAskedQuestion.question }],
        queryParamsDto: { key: 'frequentlyAskedQuestionId', value: frequentlyAskedQuestion.id },
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


}