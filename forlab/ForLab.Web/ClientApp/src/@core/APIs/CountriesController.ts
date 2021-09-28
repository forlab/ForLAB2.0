import { BaseURL } from '../config';

export const CountriesController = {
    GetAll: BaseURL + `/api/Countries/GetAll`,
    GetAllAsDrp: BaseURL + `/api/Countries/GetAllAsDrp`,
    GetCountryDetails: BaseURL + `/api/Countries/GetCountryDetails`,
    CreateCountry: BaseURL + `/api/Countries/CreateCountry`,
    UpdateCountry: BaseURL + `/api/Countries/UpdateCountry`,
    UpdateIsActive: BaseURL + `/api/Countries/UpdateIsActive`,
    UpdateIsActiveForSelected: BaseURL + `/api/Countries/UpdateIsActiveForSelected`,
    RemoveCountry: BaseURL + `/api/Countries/RemoveCountry`,
    ImportCountries: BaseURL + `/api/Countries/ImportCountries`,
    ExportCountries: BaseURL + `/api/Countries/ExportCountries`,
}