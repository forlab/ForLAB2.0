import { Lookup } from '../common/Lookup';
import { BaseFilter } from '../common/BaseFilter';

export class PatientGroupDto extends Lookup {
}


export class PatientGroupFilterDto extends BaseFilter {
  name: string = null;
}