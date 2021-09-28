import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';

export class InstrumentDto extends BaseEntityDto {
  vendorId: number = null;
  name: string = null;
  maxThroughPut: number = null;
  throughPutUnitId: number = null;
  reagentSystemId: number = null;
  controlRequirement: number = null;
  controlRequirementUnitId: number = null;
  testingAreaId: number = null;

  // UI
  vendorName: string = null;
  throughPutUnitName: string = null;
  testingAreaName: string = null;
  reagentSystemName: string = null;
  controlRequirementUnitName: string = null;
}


export class InstrumentFilterDto extends BaseFilter {
  vendorId: number = null;
  name: string = null;
  maxThroughPut: number = null;
  throughPutUnitId: number = null;
  reagentSystemId: number = null;
  controlRequirement: number = null;
  controlRequirementUnitId: number = null;
  testingAreaId: number = null;
}