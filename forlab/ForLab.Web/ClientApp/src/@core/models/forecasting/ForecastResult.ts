import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';

export class ForecastResultDto extends BaseEntityDto {
  forecastInfoId: number = null;
  laboratoryId: number = null;
  testId: number = null;
  productId: number = null;
  amountForecasted: number = null;
  totalValue: number = null;
  durationDateTime: number = null;
  period: number = null;
  packSize: number = null;
  packQty: number = null;
  packPrice: number = null;
  totalPrice: number = null;
  productTypeId: number = null;
  
  // UI
  forecastInfoName: string = null;
  laboratoryName: string = null;
  testName: string = null;
  productName: string = null;
  productTypeName: string = null;
}

export class ForecastResultFilterDto extends BaseFilter {
  forecastInfoId: number = null;
  laboratoryId: number = null;
  testId: number = null;
  productId: number = null;
  productTypeId: number = null;
}