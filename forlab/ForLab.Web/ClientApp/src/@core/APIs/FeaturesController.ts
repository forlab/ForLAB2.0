import { BaseURL } from '../config';

export const FeaturesController = {
    GetAll: BaseURL + `/api/Features/GetAll`,
    GetAllAsDrp: BaseURL + `/api/Features/GetAllAsDrp`,
    GetFeatureDetails: BaseURL + `/api/Features/GetFeatureDetails`,
    CreateFeature: BaseURL + `/api/Features/CreateFeature`,
    UpdateFeature: BaseURL + `/api/Features/UpdateFeature`,
    UpdateIsActive: BaseURL + `/api/Features/UpdateIsActive`,
    RemoveFeature: BaseURL + `/api/Features/RemoveFeature`,
}