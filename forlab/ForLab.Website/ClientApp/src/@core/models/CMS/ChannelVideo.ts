import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';

export class ChannelVideoDto extends BaseEntityDto {
  title: string = null;
  description: string = null;
  attachmentUrl: any = null;
  attachmentSize: number = null;
  attachmentName: string = null;
  extensionFormat: string = null;
  isDefault: boolean = false;
  isExternalResource: boolean = false;
}


export class ChannelVideoFilterDto extends BaseFilter {
  title: string = null;
  attachmentUrl: string = null;
  attachmentSize: number = null;
  attachmentName: string = null;
  extensionFormat: string = null;
  isDefault: boolean = null;
  isExternalResource: boolean = null;
}