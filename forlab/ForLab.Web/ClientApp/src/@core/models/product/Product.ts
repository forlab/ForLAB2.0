import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';

export class ProductDto extends BaseEntityDto {
  vendorId: number = null;
  manufacturerPrice: number = null;
  productTypeId: number = null;
  name: string = null;
  catalogNo: string = null;
  productBasicUnitId: number = null;
  packSize: number = null;
  
  // UI
  vendorName: string = null;
  productTypeName: string = null;
  productBasicUnitName: string = null;
}


export class ProductFilterDto extends BaseFilter {
  vendorId: number = null;
  manufacturerPrice: number = null;
  productTypeId: number = null;
  name: string = null;
  catalogNo: string = null;
  productBasicUnitId: number = null;
}