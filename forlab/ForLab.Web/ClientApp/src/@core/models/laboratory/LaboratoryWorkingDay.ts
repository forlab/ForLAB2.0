import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';

export class LaboratoryWorkingDayDto extends BaseEntityDto {
  laboratoryId: number = null;
  day: string = null;
  fromTime: string = null;
  toTime: string = null;
  formatedFromTime: string = null;
  formatedToTime: string = null;
  
  // UI
  laboratoryName: string = null;
  laboratoryRegionName: string = null;
  laboratoryRegionCountryName: string = null;
  laboratoryRegionId: number = null;
  laboratoryRegionCountryId: number = null;
}


export class LaboratoryWorkingDayFilterDto extends BaseFilter {
  laboratoryId: number = null;
  day: string = null;
  fromTime: string = null;
  toTime: string = null;
}