import { BaseURL } from '../config';

export const ProductUsagesController = {
  GetAll: BaseURL + `/api/ProductUsages/GetAll`,
  GetAllAsDrp: BaseURL + `/api/ProductUsages/GetAllAsDrp`,
  GetProductUsageDetails: BaseURL + `/api/ProductUsages/GetProductUsageDetails`,
  CreateProductUsage: BaseURL + `/api/ProductUsages/CreateProductUsage`,
  UpdateProductUsage: BaseURL + `/api/ProductUsages/UpdateProductUsage`,
  UpdateIsActive: BaseURL + `/api/ProductUsages/UpdateIsActive`,
  RemoveProductUsage: BaseURL + `/api/ProductUsages/RemoveProductUsage`,
  ImportProductUsages: BaseURL + `/api/ProductUsages/ImportProductUsages`,
  ExportProductUsages: BaseURL + `/api/ProductUsages/ExportProductUsages`,
}