import { BaseURL } from '../config';

export const ConfigurationsController = {
  UpdateConfiguration: BaseURL + `/api/Configurations/UpdateConfiguration`,
  GetConfigurationDetails: BaseURL + `/api/Configurations/GetConfigurationDetails`,
  GetAllConfigurationAudits: BaseURL + `/api/Configurations/GetAllConfigurationAudits`,
  ExportConfigurationAudits: BaseURL + `/api/Configurations/ExportConfigurationAudits`,
}