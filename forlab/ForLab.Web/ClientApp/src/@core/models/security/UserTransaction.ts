import { BaseFilter } from '../common/BaseFilter';
import { LocationDto } from '../common/Location';
import { Auditable } from '../common/Auditable';


export class UserTarnsactionDto extends Auditable{
  userId: number = 0;
  description: string = null;
  from: string = null;
  to: string = null;
  userTrnasctionTypeId: number = 0;

  locationDto?: LocationDto = new LocationDto();
}


export class UserTarnsactionFilterDto extends BaseFilter{
  userId: number = 0;
  ip?: string = null;
  from?: string = null;
  to?: string = null;
  userTrnasctionTypeId?: number = null;
}
