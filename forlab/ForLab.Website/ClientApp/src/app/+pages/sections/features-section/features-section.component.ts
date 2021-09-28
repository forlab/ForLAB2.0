import { Component, OnInit, Injector } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { FeatureDto } from 'src/@core/models/CMS/Feature';
import { FeaturesController } from 'src/@core/APIs/FeaturesController';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { QueryParamsDto } from 'src/@core/models/common/response';

@Component({
  selector: 'app-features-section',
  templateUrl: './features-section.component.html',
  styleUrls: ['./features-section.component.scss']
})
export class FeaturesSectionComponent extends BaseService implements OnInit {

  features$: Observable<FeatureDto[]>;
  
  constructor(public injector: Injector) {
    super(injector);
  }

  ngOnInit(): void {
    this.loadFeatures();
  }

  loadFeatures() {
    let params: QueryParamsDto[] = [
      { key: 'isActive', value: true },
    ];
    this.features$ = this.httpService.GET(FeaturesController.GetAll, params).pipe(map(res => res.data.list));
  }

}
