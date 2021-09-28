import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';

export class TestingAssumptionParameterDto extends BaseEntityDto {
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


export class TestingAssumptionParameterFilterDto extends BaseFilter {
  programId: number = null;
  name: string = null;
  isNumeric: boolean = null;
  isPercentage: boolean = null;
  isPositive: boolean = null;
  isNegative: boolean = null;
}

export class GroupTestingAssumptionParameterDto {
  programName: string = null;
  testingAssumptionParameterDtos: TestingAssumptionParameterDto[] = [];
}