import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';

export class LaboratoryPatientStatisticDto extends BaseEntityDto {
  laboratoryId: number = 0;
  period: string = null;
  count: number = null;

  // UI
  laboratoryName: string = null;
  laboratoryRegionName: string = null;
  laboratoryRegionCountryName: string = null;
  laboratoryRegionId: number = null;
  laboratoryRegionCountryId: number = null;
  // Import UI
  regionName: string = null;
  countryName: string = null;
}


export class LaboratoryPatientStatisticFilterDto extends BaseFilter {
  laboratoryId: number = null;
  period: string = null;
  count: number = null;
}