import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';

export class ForecastLaboratoryTestServiceDto extends BaseEntityDto {
  forecastInfoId: number = 0;
  laboratoryId: number = null;
  testId: number = null;
  period: string = null;
  amountForecasted: number = null;
  
  // UI
  forecastInfoName: string = null;
  laboratoryName: string = null;
  testName: string = null;
}