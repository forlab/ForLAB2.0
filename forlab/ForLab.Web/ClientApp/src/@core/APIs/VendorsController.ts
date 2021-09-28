import { BaseURL } from '../config';

export const VendorsController = {
    GetAll: BaseURL + `/api/Vendors/GetAll`,
    GetAllAsDrp: BaseURL + `/api/Vendors/GetAllAsDrp`,
    GetVendorDetails: BaseURL + `/api/Vendors/GetVendorDetails`,
    CreateVendor: BaseURL + `/api/Vendors/CreateVendor`,
    UpdateVendor: BaseURL + `/api/Vendors/UpdateVendor`,
    UpdateIsActive: BaseURL + `/api/Vendors/UpdateIsActive`,
    UpdateIsActiveForSelected: BaseURL + `/api/Vendors/UpdateIsActiveForSelected`,
    RemoveVendor: BaseURL + `/api/Vendors/RemoveVendor`,
    ImportVendors: BaseURL + `/api/Vendors/ImportVendors`,
    ExportVendors: BaseURL + `/api/Vendors/ExportVendors`,
}