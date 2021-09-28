import { BaseURL } from '../config';

export const RegionProductPricesController = {
    GetAll: BaseURL + `/api/RegionProductPrices/GetAll`,
    GetAllAsDrp: BaseURL + `/api/RegionProductPrices/GetAllAsDrp`,
    GetRegionProductPriceDetails: BaseURL + `/api/RegionProductPrices/GetRegionProductPriceDetails`,
    CreateRegionProductPrice: BaseURL + `/api/RegionProductPrices/CreateRegionProductPrice`,
    UpdateRegionProductPrice: BaseURL + `/api/RegionProductPrices/UpdateRegionProductPrice`,
    UpdateIsActive: BaseURL + `/api/RegionProductPrices/UpdateIsActive`,
    RemoveRegionProductPrice: BaseURL + `/api/RegionProductPrices/RemoveRegionProductPrice`,
    ImportRegionProductPrices: BaseURL + `/api/RegionProductPrices/ImportRegionProductPrices`,
    ExportRegionProductPrices: BaseURL + `/api/RegionProductPrices/ExportRegionProductPrices`,
}