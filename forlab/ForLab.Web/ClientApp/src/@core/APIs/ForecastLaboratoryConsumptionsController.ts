import { BaseURL } from '../config';

export const ForecastLaboratoryConsumptionsController = {
  GetAll: BaseURL + `/api/ForecastLaboratoryConsumptions/GetAll`,
  ExportForecastLaboratoryConsumptions: BaseURL + `/api/ForecastLaboratoryConsumptions/ExportForecastLaboratoryConsumptions`,
  ForecastLaboratoryConsumptionsChart: BaseURL + `/api/ForecastLaboratoryConsumptions/ForecastLaboratoryConsumptionsChart`,
}