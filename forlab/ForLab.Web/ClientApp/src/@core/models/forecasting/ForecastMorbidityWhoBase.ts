import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';

export class ForecastMorbidityWhoBaseDto extends BaseEntityDto {
  forecastInfoId: number = 0;
  diseaseId: number = null;
  countryId: number = null;
  period: string = null;
  count: number = null;
  
  // UI
  forecastInfoName: string = null;
  diseaseName: string = null;
  countryName: string = null;
}