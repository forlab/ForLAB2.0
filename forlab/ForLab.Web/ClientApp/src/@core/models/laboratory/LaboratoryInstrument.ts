import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';

export class LaboratoryInstrumentDto extends BaseEntityDto {
  laboratoryId: number = null;
  instrumentId: number = null;
  quantity: number = null;
  testRunPercentage: number = null;

  // UI
  instrumentName: string = null;
  laboratoryName: string = null;
  laboratoryRegionName: string = null;
  laboratoryRegionCountryName: string = null;
  laboratoryRegionId: number = null;
  laboratoryRegionCountryId: number = null;
  // Import UI
  regionName: string = null;
  countryName: string = null;
}


export class LaboratoryInstrumentFilterDto extends BaseFilter {
  laboratoryId: number = null;
  instrumentId: number = null;
  quantity: number = null;
  testRunPercentage: number = null;
}