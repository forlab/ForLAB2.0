import { Component, OnInit, Injector } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { ArticleDto } from 'src/@core/models/CMS/Article';
import { ArticlesController } from 'src/@core/APIs/ArticlesController';
import { Observable } from 'rxjs';
import { map, takeUntil } from 'rxjs/operators';
import { QueryParamsDto } from 'src/@core/models/common/response';

@Component({
  selector: 'app-article-details',
  templateUrl: './article-details.component.html',
  styleUrls: ['./article-details.component.scss']
})
export class ArticleDetailsComponent extends BaseService implements OnInit {

  articleId: number;
  loadingArticle: boolean = false;
  articleDto: ArticleDto = new ArticleDto();
  articles$: Observable<ArticleDto[]>;

  constructor(public injector: Injector) {
    super(injector);

    if (this.router.url.includes('details')) {
      this.activatedRoute.paramMap.subscribe(params => {
        this.articleId = Number(params.get('articleId'));
        this.loadDataById(this.articleId);
      });
    }
  }

  ngOnInit(): void {
    this.loadArticles();
  }

  loadDataById(id: number) {
    this.loadingArticle = true;
    const url = ArticlesController.GetArticleDetails;
    let params: QueryParamsDto[] = [
      { key: 'articleId', value: id },
    ];

    this.httpService.GET(url, params)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(res => {
        if (res.isPassed) {
          this.articleDto = res.data;
          this.loadingArticle = false;
        } else {
          // this.alertService.error(res.message);
          this.loading = false;
          this._ref.detectChanges();
        }
      });
  }

  loadArticles() {
    let params: QueryParamsDto[] = [
      { key: 'isActive', value: true },
      { key: 'isAscending', value: false },
      { key: 'applySort', value: true },
      { key: 'sortProperty', value: 'ProvidedDate' },
      { key: 'pageIndex', value: 1 },
      { key: 'pageSize', value: 3 },
    ];
    this.articles$ = this.httpService.GET(ArticlesController.GetAll, params).pipe(map(res => res.data.list));
  }

}
