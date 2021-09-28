import { Lookup } from '../common/Lookup';
import { BaseFilter } from '../common/BaseFilter';

export class LaboratoryLevelDto extends Lookup {
}


export class LaboratoryLevelFilterDto extends BaseFilter {
  name: string = null;
}