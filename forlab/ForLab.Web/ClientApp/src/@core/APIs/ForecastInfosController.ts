import { BaseURL } from '../config';

export const ForecastInfosController = {
  GetAll: BaseURL + `/api/ForecastInfos/GetAll`,
  GetAllAsDrp: BaseURL + `/api/ForecastInfos/GetAllAsDrp`,
  GetForecastInfoDetails: BaseURL + `/api/ForecastInfos/GetForecastInfoDetails`,
  GetForecastInfoDetailsForUpdate: BaseURL + `/api/ForecastInfos/GetForecastInfoDetailsForUpdate`,
  CreateForecastInfo: BaseURL + `/api/ForecastInfos/CreateForecastInfo`,
  UpdateForecastInfo: BaseURL + `/api/ForecastInfos/UpdateForecastInfo`,
  UpdateIsActive: BaseURL + `/api/ForecastInfos/UpdateIsActive`,
  RemoveForecastInfo: BaseURL + `/api/ForecastInfos/RemoveForecastInfo`,
  ImportForecastInfos: BaseURL + `/api/ForecastInfos/ImportForecastInfos`,
  ExportForecastInfos: BaseURL + `/api/ForecastInfos/ExportForecastInfos`,
}