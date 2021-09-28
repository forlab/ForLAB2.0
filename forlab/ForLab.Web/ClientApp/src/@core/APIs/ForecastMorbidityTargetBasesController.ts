import { BaseURL } from '../config';

export const ForecastMorbidityTargetBasesController = {
  GetAll: BaseURL + `/api/ForecastMorbidityTargetBases/GetAll`,
  ExportForecastMorbidityTargetBases: BaseURL + `/api/ForecastMorbidityTargetBases/ExportForecastMorbidityTargetBases`,
  ForecastMorbidityTargetBasesChart: BaseURL + `/api/ForecastMorbidityTargetBases/ForecastMorbidityTargetBasesChart`,
}