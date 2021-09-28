import { BaseURL } from '../config';

export const ForecastCategoriesController = {
  GetAll: BaseURL + `/api/ForecastCategories/GetAll`,
  ExportForecastCategories: BaseURL + `/api/ForecastCategories/ExportForecastCategories`,
}