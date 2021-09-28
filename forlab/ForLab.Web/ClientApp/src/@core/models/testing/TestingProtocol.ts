import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';
import { TestingProtocolCalculationPeriodMonthDto } from './TestingProtocolCalculationPeriodMonth';

export class TestingProtocolDto extends BaseEntityDto {
  name: string = null;
  testId: number = null;
  patientGroupId: number = null;
  calculationPeriodId: number = null;
  baseLine: number = null;
  testAfterFirstYear: number = null;
  testingProtocolCalculationPeriodMonthDtos: TestingProtocolCalculationPeriodMonthDto[] = [];

  // UI
  testName: string = null;
  patientGroupName: string = null;
  calculationPeriodName: string = null;
}


export class TestingProtocolFilterDto extends BaseFilter {
  name: string = null;
  testId: number = null;
  patientGroupId: number = null;
  calculationPeriodId: number = null;
  baseLine: number = null;
  testAfterFirstYear: number = null;
}