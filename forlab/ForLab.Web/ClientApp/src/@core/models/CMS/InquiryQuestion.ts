import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';

export class InquiryQuestionDto extends BaseEntityDto {
  name: string = null;
  email: string = null;
  message: string = null;
  replyProvided: boolean = false;

  inquiryQuestionReplyDtos: InquiryQuestionReplyDto[] = [];
}

export class InquiryQuestionReplyDto extends BaseEntityDto {
  inquiryQuestionId: number = 0;
  message: string = null;

  // UI
  inquiryQuestionName: string = null;
}


export class InquiryQuestionFilterDto extends BaseFilter {
  name: string = null;
  email: string = null;
  message: string = null;
  replyProvided: boolean = null;
}