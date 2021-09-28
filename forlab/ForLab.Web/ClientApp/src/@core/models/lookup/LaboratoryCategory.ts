import { Lookup } from '../common/Lookup';
import { BaseFilter } from '../common/BaseFilter';

export class LaboratoryCategoryDto extends Lookup {
}


export class LaboratoryCategoryFilterDto extends BaseFilter {
  name: string = null;
}