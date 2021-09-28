import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';

export class TicketDto extends BaseEntityDto {
	companyId: number = null;
	ticketInfo: string = null;
	ticketTypeId: number = null;
	refNumber: number = 0;
	ticketAttachmentDtos: TicketAttachmentDto[] = [];
}

export class TicketDtoFilter extends BaseFilter {
	companyId?: number = null;
	ticketTypeId?: number = null;
	ticketStatusId?: number = null;
	refNumber: number = null;
}

export class TicketAttachmentDto extends BaseEntityDto {
  ticketId: number = 0;
  attachmentUrl: string = null;
  attachmentSize: number = 0;
  extensionFormat: string = null;
  attachmentName: string = null;
}
