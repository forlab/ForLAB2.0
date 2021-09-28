import { BaseURL } from '../config';

export const LaboratoriesController = {
    GetAll: BaseURL + `/api/Laboratories/GetAll`,
    GetAllAsDrp: BaseURL + `/api/Laboratories/GetAllAsDrp`,
    GetLaboratoryDetails: BaseURL + `/api/Laboratories/GetLaboratoryDetails`,
    CreateLaboratory: BaseURL + `/api/Laboratories/CreateLaboratory`,
    UpdateLaboratory: BaseURL + `/api/Laboratories/UpdateLaboratory`,
    UpdateIsActive: BaseURL + `/api/Laboratories/UpdateIsActive`,
    UpdateIsActiveForSelected: BaseURL + `/api/Laboratories/UpdateIsActiveForSelected`,
    UpdateSharedForSelected: BaseURL + `/api/Laboratories/UpdateSharedForSelected`,
    RemoveLaboratory: BaseURL + `/api/Laboratories/RemoveLaboratory`,
    ImportLaboratories: BaseURL + `/api/Laboratories/ImportLaboratories`,
    ExportLaboratories: BaseURL + `/api/Laboratories/ExportLaboratories`,
}