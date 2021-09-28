import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';

export class ProductUsageDto extends BaseEntityDto {
  productId: number = null;
  testId: number = null;
  instrumentId: number = null;
  amount: number = null;
  isForControl: boolean = false;

  perPeriod: boolean = false;
  perPeriodPerInstrument: boolean = false;
  countryPeriodId: number = null;
  
  // UI
  productName: string = null;
  instrumentName: string = null;
  testName: string = null;
  countryPeriodName: string = null;
}


export class ProductUsageFilterDto extends BaseFilter {
  productId: number = null;
  testId: number = null;
  instrumentId: number = null;
  amount: number = null;
  isForControl: boolean = null;
  perPeriod: boolean = null;
  perPeriodPerInstrument: boolean = null;
  countryPeriodId: number = null;
  productTypeIds: string = null;
}