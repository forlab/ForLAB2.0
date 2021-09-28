import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';

export class TestDto extends BaseEntityDto {
  testingAreaId: number = null;
  name: string = null;
  shortName: string = null;

  // UI
  testingAreaName: string = null;
}


export class TestFilterDto extends BaseFilter {
  testingAreaId: number = null;
  name: string = null;
  shortName: string = null;
}