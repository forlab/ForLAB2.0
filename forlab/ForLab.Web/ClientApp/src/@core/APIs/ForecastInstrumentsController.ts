import { BaseURL } from '../config';

export const ForecastInstrumentsController = {
  GetAll: BaseURL + `/api/ForecastInstruments/GetAll`,
  ExportForecastInstruments: BaseURL + `/api/ForecastInstruments/ExportForecastInstruments`,
}