import { BaseURL } from '../config';

export const VendorContactsController = {
    GetAll: BaseURL + `/api/VendorContacts/GetAll`,
    GetAllAsDrp: BaseURL + `/api/VendorContacts/GetAllAsDrp`,
    GetVendorContactDetails: BaseURL + `/api/VendorContacts/GetVendorContactDetails`,
    CreateVendorContact: BaseURL + `/api/VendorContacts/CreateVendorContact`,
    UpdateVendorContact: BaseURL + `/api/VendorContacts/UpdateVendorContact`,
    UpdateIsActive: BaseURL + `/api/VendorContacts/UpdateIsActive`,
    RemoveVendorContact: BaseURL + `/api/VendorContacts/RemoveVendorContact`,
    ImportVendorContacts: BaseURL + `/api/VendorContacts/ImportVendorContacts`,
    ExportVendorContacts: BaseURL + `/api/VendorContacts/ExportVendorContacts`,
}