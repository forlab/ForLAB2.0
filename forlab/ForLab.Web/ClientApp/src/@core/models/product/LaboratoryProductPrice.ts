import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';

export class LaboratoryProductPriceDto extends BaseEntityDto {
  productId: number = null;
  laboratoryId: number = null;
  price: number = null;
  packSize: number = null;
  fromDate: string = null;
  
  // UI
  laboratoryName: string = null;
  productName: string = null;
  laboratoryRegionName: string = null;
  laboratoryRegionCountryName: string = null;
  laboratoryRegionId: number = null;
  laboratoryRegionCountryId: number = null;
  // Import UI
  regionName: string = null;
  countryName: string = null;
}


export class LaboratoryProductPriceFilterDto extends BaseFilter {
  productId: number = null;
  laboratoryId: number = null;
  price: number = null;
  packSize: number = null;
  fromDate: string = null;

  // UI
  countryId: number = null;
  regionId: number = null;
}