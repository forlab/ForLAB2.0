import { BaseFilter } from '../common/BaseFilter';
import { LocationDto } from '../common/Location';
import { BaseEntityDto } from '../common/BaseEntityDto';


export class UserRoleHistoryDto extends BaseEntityDto{
  userId: number = 0;
  CompanyId: number = 0;
  description: string = null;
  from: string = null;
  to: string = null;

  locationDto?: LocationDto = new LocationDto();
}


export class UserRoleHistoryFilterDto extends BaseFilter{
  userId: number = 0;
  companyId?: number = 0;
  ip?: string = null;
  from?: string = null;
  to?: string = null;
}
