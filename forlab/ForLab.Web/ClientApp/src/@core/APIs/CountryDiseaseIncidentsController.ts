import { BaseURL } from '../config';

export const CountryDiseaseIncidentsController = {
  GetAll: BaseURL + `/api/CountryDiseaseIncidents/GetAll`,
  GetAllAsDrp: BaseURL + `/api/CountryDiseaseIncidents/GetAllAsDrp`,
  GetCountryDiseaseIncidentDetails: BaseURL + `/api/CountryDiseaseIncidents/GetCountryDiseaseIncidentDetails`,
  CreateCountryDiseaseIncident: BaseURL + `/api/CountryDiseaseIncidents/CreateCountryDiseaseIncident`,
  UpdateCountryDiseaseIncident: BaseURL + `/api/CountryDiseaseIncidents/UpdateCountryDiseaseIncident`,
  UpdateIsActive: BaseURL + `/api/CountryDiseaseIncidents/UpdateIsActive`,
  UpdateIsActiveForSelected: BaseURL + `/api/CountryDiseaseIncidents/UpdateIsActiveForSelected`,
  RemoveCountryDiseaseIncident: BaseURL + `/api/CountryDiseaseIncidents/RemoveCountryDiseaseIncident`,
  ImportCountryDiseaseIncidents: BaseURL + `/api/CountryDiseaseIncidents/ImportCountryDiseaseIncidents`,
  ExportCountryDiseaseIncidents: BaseURL + `/api/CountryDiseaseIncidents/ExportCountryDiseaseIncidents`,
}