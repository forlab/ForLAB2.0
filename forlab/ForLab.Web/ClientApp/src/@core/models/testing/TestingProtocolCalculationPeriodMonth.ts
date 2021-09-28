import { BaseEntityDto } from '../common/BaseEntityDto';

export class TestingProtocolCalculationPeriodMonthDto extends BaseEntityDto {
  testingProtocolId: number = 0;
  calculationPeriodMonthId: number = null;
  value: number = null;

  // UI
  testingProtocolName: string = null;
  calculationPeriodMonthName: string = null;
}