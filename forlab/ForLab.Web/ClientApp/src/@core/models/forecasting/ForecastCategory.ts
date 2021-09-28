import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';

export class ForecastCategoryDto extends BaseEntityDto {
  forecastInfoId: number = 0;
  name: string = null;
  
  // UI
  forecastInfoName: string = null;
}