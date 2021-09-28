import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';

export class ForecastTestDto extends BaseEntityDto {
  forecastInfoId: number = 0;
  testId: number = null;
  
  // UI
  forecastInfoName: string = null;
  testTestingAreaName: string = null;
  testName: string = null;
}