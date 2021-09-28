import { BaseURL } from '../config';

export const ForecastResultsController = {
  GetAll: BaseURL + `/api/ForecastResults/GetAll`,
  ExportForecastResults: BaseURL + `/api/ForecastResults/ExportForecastResults`,
  ForecastResultsChart: BaseURL + `/api/ForecastResults/ForecastResultsChart`,
}