import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';

export class ProductAssumptionParameterDto extends BaseEntityDto {
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


export class ProductAssumptionParameterFilterDto extends BaseFilter {
  programId: number = null;
  name: string = null;
  isNumeric: boolean = null;
  isPercentage: boolean = null;
  isPositive: boolean = null;
  isNegative: boolean = null;
}

export class GroupProductAssumptionParameterDto {
  programName: string = null;
  productAssumptionParameterDtos: ProductAssumptionParameterDto[] = [];
}