import { BaseURL } from '../config';

export const CountryProductPricesController = {
    GetAll: BaseURL + `/api/CountryProductPrices/GetAll`,
    GetAllAsDrp: BaseURL + `/api/CountryProductPrices/GetAllAsDrp`,
    GetCountryProductPriceDetails: BaseURL + `/api/CountryProductPrices/GetCountryProductPriceDetails`,
    CreateCountryProductPrice: BaseURL + `/api/CountryProductPrices/CreateCountryProductPrice`,
    UpdateCountryProductPrice: BaseURL + `/api/CountryProductPrices/UpdateCountryProductPrice`,
    UpdateIsActive: BaseURL + `/api/CountryProductPrices/UpdateIsActive`,
    RemoveCountryProductPrice: BaseURL + `/api/CountryProductPrices/RemoveCountryProductPrice`,
    ImportCountryProductPrices: BaseURL + `/api/CountryProductPrices/ImportCountryProductPrices`,
    ExportCountryProductPrices: BaseURL + `/api/CountryProductPrices/ExportCountryProductPrices`,
}