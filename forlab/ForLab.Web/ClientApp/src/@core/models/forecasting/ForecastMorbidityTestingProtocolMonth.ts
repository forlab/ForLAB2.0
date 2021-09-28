import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';
import { TestingProtocolCalculationPeriodMonthDto } from '../testing/TestingProtocolCalculationPeriodMonth';

export class ForecastMorbidityTestingProtocolMonthDto extends BaseEntityDto {
  forecastInfoId: number = 0;
  programId: number = null;
  testId: number = null;
  patientGroupId: number = null;
  testingProtocolId: number = null;
  calculationPeriodMonthId: number = null;
  value: number = null;
  
  // UI
  forecastInfoName: string = null;
  programName: string = null;
  testName: string = null;
  testingProtocolName: string = null;
  patientGroupName: string = null;
  calculationPeriodMonthName: string = null;
  testingProtocolCalculationPeriodMonthDtos: TestingProtocolCalculationPeriodMonthDto[] = [];
}