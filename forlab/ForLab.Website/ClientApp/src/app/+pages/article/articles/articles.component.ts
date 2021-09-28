import { Component, OnInit, Injector } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { ArticleDto } from 'src/@core/models/CMS/Article';
import { ArticlesController } from 'src/@core/APIs/ArticlesController';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { QueryParamsDto } from 'src/@core/models/common/response';

@Component({
  selector: 'app-articles',
  templateUrl: './articles.component.html',
  styleUrls: ['./articles.component.scss']
})
export class ArticlesComponent extends BaseService implements OnInit {

  articles$: Observable<ArticleDto[]>;
  pageIndex: number = 1;
  pageSize: number = 9;

  constructor(public injector: Injector) {
    super(injector);
  }

  ngOnInit(): void {
    this.loadArticles();
  }

  loadArticles() {
    let params: QueryParamsDto[] = [
      { key: 'pageIndex', value: this.pageIndex },
      { key: 'pageSize', value: this.pageSize },
      { key: 'isActive', value: true },
    ];
    this.articles$ = this.httpService.GET(ArticlesController.GetAll, params).pipe(map(res => res.data));
  }

}
