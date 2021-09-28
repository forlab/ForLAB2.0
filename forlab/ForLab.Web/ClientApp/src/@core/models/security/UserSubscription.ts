import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';

export class UserCountrySubscriptionDto extends BaseEntityDto {
  applicationUserId: number = 0;
  countryId: number = null;

  // UI
  countryName: string = null;
}

export class UserRegionSubscriptionDto extends BaseEntityDto {
  applicationUserId: number = 0;
  regionId: number = null;

  // UI
  regionName: string = null;
}

export class UserLaboratorySubscriptionDto extends BaseEntityDto {
  applicationUserId: number = 0;
  laboratoryId: number = null;

  // UI
  laboratoryName: string = null;
}

export class UserSubscriptionFilterDto extends BaseFilter {
  applicationUserId: number = null;
  countryId: number = null;
  regionId: number = null;
  laboratoryId: number = null;
  name: string = null;
}
