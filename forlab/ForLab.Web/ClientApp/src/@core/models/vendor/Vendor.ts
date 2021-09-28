import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';
import { VendorContactDto } from './VendorContact';

export class VendorDto extends BaseEntityDto {
  name: string = null;
  address: string = null;
  telephone: string = null;
  email: string = null;
  url: string = null;
  vendorContactDtos: VendorContactDto[] = [];
}


export class VendorFilterDto extends BaseFilter {
  name: string = null;
  address: string = null;
  telephone: string = null;
  email: string = null;
  url: string = null;
}