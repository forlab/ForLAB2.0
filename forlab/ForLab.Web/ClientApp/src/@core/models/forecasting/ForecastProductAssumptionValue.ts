import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';

export class ForecastProductAssumptionValueDto extends BaseEntityDto {
  forecastInfoId: number = 0;
  productAssumptionParameterId: number = null;
  value: number = null;
  
  // UI
  forecastInfoName: string = null;
  productAssumptionParameterName: string = null;
}