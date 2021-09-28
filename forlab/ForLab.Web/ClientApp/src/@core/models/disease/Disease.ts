import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';

export class DiseaseDto extends BaseEntityDto {
  name: string = null;
  description: string = null;
}


export class DiseaseFilterDto extends BaseFilter {
  name: string = null;
  description: string = null;
}