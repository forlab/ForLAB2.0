import { BaseURL } from '../config';

export const LaboratoryProductPricesController = {
    GetAll: BaseURL + `/api/LaboratoryProductPrices/GetAll`,
    GetAllAsDrp: BaseURL + `/api/LaboratoryProductPrices/GetAllAsDrp`,
    GetLaboratoryProductPriceDetails: BaseURL + `/api/LaboratoryProductPrices/GetLaboratoryProductPriceDetails`,
    CreateLaboratoryProductPrice: BaseURL + `/api/LaboratoryProductPrices/CreateLaboratoryProductPrice`,
    UpdateLaboratoryProductPrice: BaseURL + `/api/LaboratoryProductPrices/UpdateLaboratoryProductPrice`,
    UpdateIsActive: BaseURL + `/api/LaboratoryProductPrices/UpdateIsActive`,
    RemoveLaboratoryProductPrice: BaseURL + `/api/LaboratoryProductPrices/RemoveLaboratoryProductPrice`,
    ImportLaboratoryProductPrices: BaseURL + `/api/LaboratoryProductPrices/ImportLaboratoryProductPrices`,
    ExportLaboratoryProductPrices: BaseURL + `/api/LaboratoryProductPrices/ExportLaboratoryProductPrices`,
}