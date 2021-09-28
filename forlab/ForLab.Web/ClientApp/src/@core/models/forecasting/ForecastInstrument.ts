import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';

export class ForecastInstrumentDto extends BaseEntityDto {
  forecastInfoId: number = 0;
  instrumentId: number = null;
  
  // UI
  forecastInfoName: string = null;
  instrumentTestingAreaName: string = null;
  instrumentName: string = null;
}