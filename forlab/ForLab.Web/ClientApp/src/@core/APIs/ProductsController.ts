import { BaseURL } from '../config';

export const ProductsController = {
    GetAll: BaseURL + `/api/Products/GetAll`,
    GetAllAsDrp: BaseURL + `/api/Products/GetAllAsDrp`,
    GetProductDetails: BaseURL + `/api/Products/GetProductDetails`,
    CreateProduct: BaseURL + `/api/Products/CreateProduct`,
    UpdateProduct: BaseURL + `/api/Products/UpdateProduct`,
    UpdateIsActive: BaseURL + `/api/Products/UpdateIsActive`,
    UpdateIsActiveForSelected: BaseURL + `/api/Products/UpdateIsActiveForSelected`,
    UpdateSharedForSelected: BaseURL + `/api/Products/UpdateSharedForSelected`,
    RemoveProduct: BaseURL + `/api/Products/RemoveProduct`,
    ImportProducts: BaseURL + `/api/Products/ImportProducts`,
    ExportProducts: BaseURL + `/api/Products/ExportProducts`,
}