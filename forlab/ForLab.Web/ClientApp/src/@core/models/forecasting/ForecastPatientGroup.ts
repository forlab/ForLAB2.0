import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';

export class ForecastPatientGroupDto extends BaseEntityDto {
  forecastInfoId: number = 0;
  patientGroupId: number = null;
  programId: number = null;
  percentage: number = null;
  
  // UI
  forecastInfoName: string = null;
  patientGroupName: string = null;
  programName: string = null;
}