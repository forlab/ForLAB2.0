import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';

export class PatientAssumptionParameterDto extends BaseEntityDto {
  programId: number = null;
  name: string = null;
  isNumeric: boolean = false;
  isPercentage: boolean = false;
  isPositive: boolean = false;
  isNegative: boolean = false;

  // UI
  programName: string = null;
  value: number; // when creating the forecast
}


export class PatientAssumptionParameterFilterDto extends BaseFilter {
  programId: number = null;
  name: string = null;
  isNumeric: boolean = null;
  isPercentage: boolean = null;
  isPositive: boolean = null;
  isNegative: boolean = null;
}

export class GroupPatientAssumptionParameterDto {
  programName: string = null;
  patientAssumptionParameterDtos: PatientAssumptionParameterDto[] = [];
}