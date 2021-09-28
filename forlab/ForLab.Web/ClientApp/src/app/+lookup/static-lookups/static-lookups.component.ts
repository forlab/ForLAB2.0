import { Component, OnInit, Injector } from '@angular/core';
import { BaseService } from 'src/@core/services/base.service';
import { ThroughPutUnitsController } from 'src/@core/APIs/ThroughPutUnitsController';
import { ProductBasicUnitsController } from 'src/@core/APIs/ProductBasicUnitsController';
import {
  ContinentEnum,
  ControlRequirementUnitEnum,
  CountryPeriodEnum,
  EntityTypeEnum,
  ForecastConsumableUsagePeriodEnum,
  ForecastInfoLevelEnum,
  ForecastInfoStatusEnum,
  ForecastMethodologyEnum,
  ProductBasicUnitEnum,
  ReagentSystemEnum,
  ScopeOfTheForecastEnum,
  ThroughPutUnitEnum,
  VariableTypeEnum,
  ProductTypeEnum
} from 'src/@core/models/enum/Enums';
import { map, takeUntil } from 'rxjs/operators';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-static-lookups',
  templateUrl: './static-lookups.component.html',
  styleUrls: ['./static-lookups.component.sass']
})
export class StaticLookupsComponent extends BaseService implements OnInit {

  continentEnum = ContinentEnum;
  controlRequirementUnitEnum = ControlRequirementUnitEnum;
  countryPeriodEnum = CountryPeriodEnum;
  entityTypeEnum = EntityTypeEnum;
  forecastConsumableUsagePeriodEnum = ForecastConsumableUsagePeriodEnum;
  forecastInfoLevelEnum = ForecastInfoLevelEnum;
  forecastInfoStatusEnum = ForecastInfoStatusEnum;
  forecastMethodologyEnum = ForecastMethodologyEnum;
  productBasicUnitEnum = ProductBasicUnitEnum;
  reagentSystemEnum = ReagentSystemEnum;
  scopeOfTheForecastEnum = ScopeOfTheForecastEnum;
  throughPutUnitEnum = ThroughPutUnitEnum;
  variableTypeEnum = VariableTypeEnum;
  productTypeEnum = ProductTypeEnum;
  // From DB
  productBasicUnits$: Observable<any[]>;
  throughPutUnits$: Observable<any[]>;

  constructor(public injector: Injector) {
    super(injector);
  }

  ngOnInit(): void {
    this.getThroughPutUnits();
    this.getProductBasicUnits();
  }

  getThroughPutUnits() {
    this.throughPutUnits$ = this.httpService.GET(ThroughPutUnitsController.GetThroughPutUnits).pipe(map(res => res.data));
  }

  getProductBasicUnits() {
    this.productBasicUnits$ = this.httpService.GET(ProductBasicUnitsController.GetProductBasicUnits).pipe(map(res => res.data));
  }

}
