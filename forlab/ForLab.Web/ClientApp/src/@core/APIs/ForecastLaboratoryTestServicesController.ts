import { BaseURL } from '../config';

export const ForecastLaboratoryTestServicesController = {
  GetAll: BaseURL + `/api/ForecastLaboratoryTestServices/GetAll`,
  ExportForecastLaboratoryTestServices: BaseURL + `/api/ForecastLaboratoryTestServices/ExportForecastLaboratoryTestServices`,
  ForecastLaboratoryTestServicesChart: BaseURL + `/api/ForecastLaboratoryTestServices/ForecastLaboratoryTestServicesChart`,
}