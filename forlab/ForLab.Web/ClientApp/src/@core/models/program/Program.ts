import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';
import { ProgramTestDto } from './ProgramTest';
import { TestingAssumptionParameterDto } from './TestingAssumptionParameter';
import { PatientAssumptionParameterDto } from './PatientAssumptionParameter';
import { ProductAssumptionParameterDto } from './ProductAssumptionParameter';

export class ProgramDto extends BaseEntityDto {
  diseaseId: number = null;
  name: string = null;
  numberOfYears: number = null;
  programTestDtos: ProgramTestDto[] = [];
  testingAssumptionParameterDtos: TestingAssumptionParameterDto[] = [];
  productAssumptionParameterDtos: ProductAssumptionParameterDto[] = [];
  patientAssumptionParameterDtos: PatientAssumptionParameterDto[] = [];

  // UI
  diseaseName: string = null;
}


export class ProgramFilterDto extends BaseFilter {
  diseaseId: number = null;
  name: string = null;
  numberOfYears: number = null;
}