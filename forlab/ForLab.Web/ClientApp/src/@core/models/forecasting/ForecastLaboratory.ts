import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';

export class ForecastLaboratoryDto extends BaseEntityDto {
  forecastInfoId: number = 0;
  forecastCategoryId: number = 0;
  laboratoryId: number = null;
  
  // UI
  forecastInfoName: string = null;
  forecastCategoryName: string = null;
  laboratoryName: string = null;
}