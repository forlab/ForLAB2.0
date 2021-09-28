import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';

export class TicketReplyDto extends BaseEntityDto {
	ticketId?: number = 0;
	replyInfo?: string = null;
	repliedBySupport?: boolean = false;
	ticketReplyAttachmentDtos: TicketReplyAttachmentDto[] = [];
}

export class TicketReplyDtoFilter extends BaseFilter {
	ticketId?: number = 0;
	replyInfo?: string = null;
	repliedBySupport?: boolean = false;
}

export class TicketReplyAttachmentDto extends BaseEntityDto {
	ticketReplyId: number = 0;
	attachmentUrl: string = null;
	attachmentSize: number = 0;
	extensionFormat: string = null;
	attachmentName: string = null;
}