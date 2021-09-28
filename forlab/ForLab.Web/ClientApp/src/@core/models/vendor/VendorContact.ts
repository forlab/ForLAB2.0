import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';

export class VendorContactDto extends BaseEntityDto {
  vendorId: number = null;
  name: string = null;
  telephone: string = null;
  email: string = null;
  
  // UI
  vendorName: string = null;
}


export class VendorContactFilterDto extends BaseFilter {
  vendorId: number = null;
  name: string = null;
  telephone: string = null;
  email: string = null;
}