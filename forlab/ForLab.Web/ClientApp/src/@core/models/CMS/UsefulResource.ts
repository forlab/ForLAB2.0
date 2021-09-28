import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';

export class UsefulResourceDto extends BaseEntityDto {
  title: string = null;
  attachmentUrl: string = null;
  attachmentSize: number = null;
  attachmentName: string = null;
  extensionFormat: string = null;
  isExternalResource: boolean = false;
  downloadCount: number = null;
}

export class UsefulResourceFilterDto extends BaseFilter {
  title: string = null;
  attachmentUrl: string = null;
  attachmentSize: number = null;
  attachmentName: string = null;
  extensionFormat: string = null;
  isExternalResource: boolean = null;
  downloadCount: number = null;
}