import { BaseURL } from '../config';

export const LaboratoryLevelsController = {
    GetAll: BaseURL + `/api/LaboratoryLevels/GetAll`,
    GetAllAsDrp: BaseURL + `/api/LaboratoryLevels/GetAllAsDrp`,
    GetLaboratoryLevelDetails: BaseURL + `/api/LaboratoryLevels/GetLaboratoryLevelDetails`,
    CreateLaboratoryLevel: BaseURL + `/api/LaboratoryLevels/CreateLaboratoryLevel`,
    UpdateLaboratoryLevel: BaseURL + `/api/LaboratoryLevels/UpdateLaboratoryLevel`,
    UpdateIsActive: BaseURL + `/api/LaboratoryLevels/UpdateIsActive`,
    UpdateIsActiveForSelected: BaseURL + `/api/LaboratoryLevels/UpdateIsActiveForSelected`,
    UpdateSharedForSelected: BaseURL + `/api/LaboratoryLevels/UpdateSharedForSelected`,
    RemoveLaboratoryLevel: BaseURL + `/api/LaboratoryLevels/RemoveLaboratoryLevel`,
    ImportLaboratoryLevels: BaseURL + `/api/LaboratoryLevels/ImportLaboratoryLevels`,
    ExportLaboratoryLevels: BaseURL + `/api/LaboratoryLevels/ExportLaboratoryLevels`,
}