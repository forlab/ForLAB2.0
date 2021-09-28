import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';

export class FeatureDto extends BaseEntityDto {
  title: string = null;
  description: string = null;
  logoPath: string = null;
}


export class FeatureFilterDto extends BaseFilter {
  title: string = null;
}