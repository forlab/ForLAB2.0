import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';

export class ArticleDto extends BaseEntityDto {
  title: string = null;
  content: string = null;
  providedBy: string = null;
  providedDate: string = null;
  articleImageDtos: ArticleImageDto[] = [];
}

export class ArticleImageDto extends BaseEntityDto {
  articleId: number = 0;
  attachmentUrl: any = null;
  attachmentSize: number = null;
  attachmentName: string = null;
  extensionFormat: string = null;
  isDefault: boolean = false;
  isExternalResource: boolean = false;

  // UI
  articleName: string = null;
}


export class ArticleFilterDto extends BaseFilter {
  title: string = null;
  content: string = null;
  providedBy: string = null;
  providedDate: string = null;
}