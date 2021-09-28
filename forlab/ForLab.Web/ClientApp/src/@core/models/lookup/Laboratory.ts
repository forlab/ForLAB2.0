import { Lookup } from '../common/Lookup';
import { BaseFilter } from '../common/BaseFilter';

export class LaboratoryDto extends Lookup {
  regionId: number = null;
  laboratoryCategoryId: number = null;
  laboratoryLevelId: number = null;
  latitude: string = null;
  longitude: string = null;

  // UI
  regionCountryId: number = null;
  regionCountryName: string = null;
  regionName: string = null;
  laboratoryCategoryName: string = null;
  laboratoryLevelName: string = null;
}


export class LaboratoryFilterDto extends BaseFilter {
  name: string = null;
  regionId: number = null;
  laboratoryCategoryId: number = null;
  laboratoryLevelId: number = null;
  latitude: string = null;
  longitude: string = null;
}