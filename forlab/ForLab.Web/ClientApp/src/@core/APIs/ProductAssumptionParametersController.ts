import { BaseURL } from '../config';

export const ProductAssumptionParametersController = {
  GetAll: BaseURL + `/api/ProductAssumptionParameters/GetAll`,
  GetAllAsDrp: BaseURL + `/api/ProductAssumptionParameters/GetAllAsDrp`,
  GetAllProductAssumptionsForForcast: BaseURL + `/api/ProductAssumptionParameters/GetAllProductAssumptionsForForcast`,
  GetProductAssumptionParameterDetails: BaseURL + `/api/ProductAssumptionParameters/GetProductAssumptionParameterDetails`,
  CreateProductAssumptionParameter: BaseURL + `/api/ProductAssumptionParameters/CreateProductAssumptionParameter`,
  UpdateProductAssumptionParameter: BaseURL + `/api/ProductAssumptionParameters/UpdateProductAssumptionParameter`,
  UpdateIsActive: BaseURL + `/api/ProductAssumptionParameters/UpdateIsActive`,
  RemoveProductAssumptionParameter: BaseURL + `/api/ProductAssumptionParameters/RemoveProductAssumptionParameter`,
  ImportProductAssumptionParameters: BaseURL + `/api/ProductAssumptionParameters/ImportProductAssumptionParameters`,
  ExportProductAssumptionParameters: BaseURL + `/api/ProductAssumptionParameters/ExportProductAssumptionParameters`,
}