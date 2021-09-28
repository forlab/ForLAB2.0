import { Lookup } from '../common/Lookup';
import { BaseFilter } from '../common/BaseFilter';

export class RegionDto extends Lookup {
  countryId: number = null;
  shortName: string = null;

  // UI
  countryName: string = null;
  countryFlag: string = null;
}


export class RegionFilterDto extends BaseFilter {
  name: string = null;
  countryId: number = null;
  shortName: string = null;
}