import { Component, ElementRef, OnInit, ViewChild, Injector } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { fromEvent } from 'rxjs';
import { SelectionModel } from '@angular/cdk/collections';
import { QueryParamsDto } from 'src/@core/models/common/response';
import { ConfirmDeleteComponent } from 'src/@core/directives/confirm-delete/confirm-delete.component';
import { ArticleDto, ArticleFilterDto } from 'src/@core/models/CMS/Article';
import { TableColumn } from 'src/@core/models/common/response';
import { ArticlesController } from 'src/@core/APIs/ArticlesController';
import { ObjectDataSource } from 'src/@core/services/object.datasource';
import { takeUntil, tap, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ConfirmActiveComponent } from 'src/@core/directives/confirm-active/confirm-active.component';
import { BaseService } from 'src/@core/services/base.service';

@Component({
  selector: 'app-articles',
  templateUrl: './articles.component.html',
  styleUrls: ['./articles.component.scss']
})
export class ArticlesComponent extends BaseService implements OnInit {

  filterDto: ArticleFilterDto = new ArticleFilterDto();

  // Table
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort;
  @ViewChild('searchInput', { static: false }) searchInput: ElementRef;
  columns: TableColumn<ArticleDto>[] = [
    { label: 'Actions', property: 'actions', type: 'button', visible: true },
    // { label: 'Select', property: 'select', type: 'button', visible: false },
    { label: 'Status', property: 'isActive', type: 'button', visible: true },
    { label: 'Title', property: 'title', type: 'text', visible: true },
    { label: 'Provided By', property: 'providedBy', type: 'text', visible: true },
    { label: 'Provided Date', property: 'providedDate', type: 'date', visible: true },
    { label: 'Creator', property: 'creator', type: 'text', visible: false },
    { label: 'Created On', property: 'createdOn', type: 'datetime', visible: false },
  ];
  dataSource: ObjectDataSource;
  selection = new SelectionModel<ArticleDto>(true, []);

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
      { key: 'title', value: this.searchInput.nativeElement.value || this.filterDto.title },
    ];

    // Append FilterDto values to the paramaters
    params = params.concat(this.getFilterParamsDtos(this.filterDto, ['title']));

    this.dataSource.loadObjects(ArticlesController.GetAll, params);
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


  updateIsActive(article: ArticleDto) {
    this.dialog.open(ConfirmActiveComponent, {
      data: {
        url: ArticlesController.UpdateIsActive,
        objectInfo: [{ key: 'Title', value: article.title }, { key: 'Provided By', value: article.providedBy }],
        isActive: !article.isActive,
        queryParamsDto: { key: 'articleId', value: article.id },
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

  deleteObject(article: ArticleDto) {
    this.dialog.open(ConfirmDeleteComponent, {
      data: {
        url: ArticlesController.RemoveArticle,
        objectInfo: [{ key: 'Title', value: article.title }, { key: 'Provided By', value: article.providedBy }],
        queryParamsDto: { key: 'articleId', value: article.id },
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
