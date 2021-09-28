import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';

export class ForecastMorbidityProgramDto extends BaseEntityDto {
  forecastInfoId: number = 0;
  programId: number = null;
  
  // UI
  forecastInfoName: string = null;
  programName: string = null;
}