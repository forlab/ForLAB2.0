import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';

export class ForecastTestingAssumptionValueDto extends BaseEntityDto {
  forecastInfoId: number = 0;
  testingAssumptionParameterId: number = null;
  value: number = null;
  
  // UI
  forecastInfoName: string = null;
  testingAssumptionParameterName: string = null;
}