import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';

export class LaboratoryTestServiceDto extends BaseEntityDto {
  laboratoryId: number = 0;
  testId: number = null;
  serviceDuration: string = null;
  testPerformed: number = null;

  // UI
  laboratoryName: string = null;
  testName: string = null;
  laboratoryRegionName: string = null;
  laboratoryRegionCountryName: string = null;
  laboratoryRegionId: number = null;
  laboratoryRegionCountryId: number = null;
  // Import UI
  regionName: string = null;
  countryName: string = null;
}


export class LaboratoryTestServiceFilterDto extends BaseFilter {
  laboratoryId: number = null;
  testId: number = null;
  serviceDuration: string = null;
  testPerformed: number = null;
}