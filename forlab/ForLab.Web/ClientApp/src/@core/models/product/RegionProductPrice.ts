import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';

export class RegionProductPriceDto extends BaseEntityDto {
  productId: number = null;
  regionId: number = null;
  price: number = null;
  packSize: number = null;
  fromDate: string = null;
  
  // UI
  regionName: string = null;
  productName: string = null;
  regionCountryId: number = null;
  regionCountryName: string = null;
  // Import
  countryName: string = null;
}


export class RegionProductPriceFilterDto extends BaseFilter {
  productId: number = null;
  regionId: number = null;
  price: number = null;
  packSize: number = null;
  fromDate: string = null;

  // UI
  countryId: number = null;
}