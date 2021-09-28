import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';

export class FrequentlyAskedQuestionDto extends BaseEntityDto {
  question: string = null;
  answer: string = null;
}

export class FrequentlyAskedQuestionFilterDto extends BaseFilter {
  question: string = null;
  answer: string = null;
}