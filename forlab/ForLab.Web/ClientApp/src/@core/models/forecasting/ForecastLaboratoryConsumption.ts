import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';

export class ForecastLaboratoryConsumptionDto extends BaseEntityDto {
  forecastInfoId: number = 0;
  laboratoryId: number = null;
  productId: number = null;
  period: string = null;
  amountForecasted: number = null;
  
  // UI
  forecastInfoName: string = null;
  laboratoryName: string = null;
  productName: string = null;
}