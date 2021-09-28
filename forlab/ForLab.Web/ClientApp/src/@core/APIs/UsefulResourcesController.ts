import { BaseURL } from '../config';

export const UsefulResourcesController = {
    GetAll: BaseURL + `/api/UsefulResources/GetAll`,
    GetAllAsDrp: BaseURL + `/api/UsefulResources/GetAllAsDrp`,
    GetUsefulResourceDetails: BaseURL + `/api/UsefulResources/GetUsefulResourceDetails`,
    CreateUsefulResource: BaseURL + `/api/UsefulResources/CreateUsefulResource`,
    UpdateUsefulResource: BaseURL + `/api/UsefulResources/UpdateUsefulResource`,
    UpdateIsActive: BaseURL + `/api/UsefulResources/UpdateIsActive`,
    RemoveUsefulResource: BaseURL + `/api/UsefulResources/RemoveUsefulResource`,
    IncrementDownloadCount: BaseURL + `/api/UsefulResources/IncrementDownloadCount`,
}