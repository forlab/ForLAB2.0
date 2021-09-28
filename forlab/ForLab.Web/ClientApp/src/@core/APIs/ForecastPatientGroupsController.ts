import { BaseURL } from '../config';

export const ForecastPatientGroupsController = {
  GetAll: BaseURL + `/api/ForecastPatientGroups/GetAll`,
  ExportForecastPatientGroups: BaseURL + `/api/ForecastPatientGroups/ExportForecastPatientGroups`,
}