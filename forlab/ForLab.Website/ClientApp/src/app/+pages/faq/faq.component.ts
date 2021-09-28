import { Component, OnInit, Injector } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { FrequentlyAskedQuestionDto } from 'src/@core/models/CMS/FrequentlyAskedQuestion';
import { FrequentlyAskedQuestionsController } from 'src/@core/APIs/FrequentlyAskedQuestionsController';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { QueryParamsDto } from 'src/@core/models/common/response';


@Component({
  selector: 'app-faq',
  templateUrl: './faq.component.html',
  styleUrls: ['./faq.component.scss']
})
export class FaqComponent extends BaseService implements OnInit {

  faqs$: Observable<FrequentlyAskedQuestionDto[]>;
  
  constructor(public injector: Injector) {
    super(injector);
  }

  ngOnInit(): void {
    this.loadFAQs();
  }

  loadFAQs() {
    let params: QueryParamsDto[] = [
      { key: 'isActive', value: true },
    ];
    this.faqs$ = this.httpService.GET(FrequentlyAskedQuestionsController.GetAll, params).pipe(map(res => res.data.list));
  }

}
