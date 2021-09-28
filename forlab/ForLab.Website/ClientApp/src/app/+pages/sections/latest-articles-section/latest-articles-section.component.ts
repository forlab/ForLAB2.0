import { Component, OnInit, Injector } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { ArticleDto } from 'src/@core/models/CMS/Article';
import { ArticlesController } from 'src/@core/APIs/ArticlesController';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { QueryParamsDto } from 'src/@core/models/common/response';

@Component({
  selector: 'app-latest-articles-section',
  templateUrl: './latest-articles-section.component.html',
  styleUrls: ['./latest-articles-section.component.scss']
})
export class LatestArticlesSectionComponent extends BaseService implements OnInit {

  articles$: Observable<ArticleDto[]>;

  constructor(public injector: Injector) {
    super(injector);
  }

  ngOnInit(): void {
    this.loadArticles();
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
