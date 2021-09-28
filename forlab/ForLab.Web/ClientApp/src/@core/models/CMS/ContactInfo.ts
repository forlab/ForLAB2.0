import { BaseEntityDto } from '../common/BaseEntityDto';

export class ContactInfoDto extends BaseEntityDto {
  phone: string = null;
  email: string = null;
  address: string = null;
  latitude: string = null;
  longitude: string = null;
  // Social Links
  facebook: string = null;
  twitter: string = null;
  linkedIn: string = null;
}