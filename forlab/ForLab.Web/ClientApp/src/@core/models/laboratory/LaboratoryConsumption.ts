import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';

export class LaboratoryConsumptionDto extends BaseEntityDto {
  laboratoryId: number = null;
  productId: number = null;
  consumptionDuration: string = null;
  amountUsed: number = null;

  // UI
  productName: string = null;
  laboratoryName: string = null;
  laboratoryRegionName: string = null;
  laboratoryRegionCountryName: string = null;
  laboratoryRegionId: number = null;
  laboratoryRegionCountryId: number = null;
  // Import UI
  regionName: string = null;
  countryName: string = null;
}


export class LaboratoryConsumptionFilterDto extends BaseFilter {
  laboratoryId: number = null;
  productId: number = null;
  consumptionDuration: string = null;
  amountUsed: number = null;
}