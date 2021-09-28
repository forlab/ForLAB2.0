import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';

export class CountryProductPriceDto extends BaseEntityDto {
  productId: number = null;
  countryId: number = null;
  price: number = null;
  packSize: number = null;
  fromDate: string = null;
  
  // UI
  countryName: string = null;
  productName: string = null;
}


export class CountryProductPriceFilterDto extends BaseFilter {
  productId: number = null;
  countryId: number = null;
  price: number = null;
  packSize: number = null;
  fromDate: string = null;
}