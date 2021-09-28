import { Lookup } from '../common/Lookup';
import { BaseFilter } from '../common/BaseFilter';

export class TestingAreaDto extends Lookup {
}


export class TestingAreaFilterDto extends BaseFilter {
  name: string = null;
}