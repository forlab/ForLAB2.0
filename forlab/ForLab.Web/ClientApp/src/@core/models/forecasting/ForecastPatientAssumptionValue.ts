import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';

export class ForecastPatientAssumptionValueDto extends BaseEntityDto {
  forecastInfoId: number = 0;
  patientAssumptionParameterId: number = null;
  value: number = null;
  
  // UI
  forecastInfoName: string = null;
  patientAssumptionParameterName: string = null;
}