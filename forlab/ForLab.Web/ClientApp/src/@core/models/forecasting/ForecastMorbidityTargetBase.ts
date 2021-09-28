import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';

export class ForecastMorbidityTargetBaseDto extends BaseEntityDto {
  forecastLaboratoryId: number = 0;
  forecastMorbidityProgramId: number = 0;
  currentPatient: number = null;
  targetPatient: number = null;
  
  // UI
  forecastLaboratoryLaboratoryName: string = null;
  forecastMorbidityProgramName: string = null;
}