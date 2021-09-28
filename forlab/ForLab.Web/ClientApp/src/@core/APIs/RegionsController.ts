import { BaseURL } from '../config';

export const RegionsController = {
    GetAll: BaseURL + `/api/Regions/GetAll`,
    GetAllAsDrp: BaseURL + `/api/Regions/GetAllAsDrp`,
    GetRegionDetails: BaseURL + `/api/Regions/GetRegionDetails`,
    CreateRegion: BaseURL + `/api/Regions/CreateRegion`,
    UpdateRegion: BaseURL + `/api/Regions/UpdateRegion`,
    UpdateIsActive: BaseURL + `/api/Regions/UpdateIsActive`,
    UpdateIsActiveForSelected: BaseURL + `/api/Regions/UpdateIsActiveForSelected`,
    RemoveRegion: BaseURL + `/api/Regions/RemoveRegion`,
    ImportRegions: BaseURL + `/api/Regions/ImportRegions`,
    ExportRegions: BaseURL + `/api/Regions/ExportRegions`,
}