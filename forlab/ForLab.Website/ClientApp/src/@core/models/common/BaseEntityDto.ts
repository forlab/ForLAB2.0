export class BaseEntityDto {
  id?: number = 0;
  isDeleted: boolean = false;
  isActive: boolean = true;
  createdBy: number;
  createdOn?: string = new Date().toISOString();
  updatedBy?: number;
  updatedOn?: string;

  // UI
  creator: string;
  updator: string;
}
