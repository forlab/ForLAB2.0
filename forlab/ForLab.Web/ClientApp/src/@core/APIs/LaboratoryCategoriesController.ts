import { BaseURL } from '../config';

export const LaboratoryCategoriesController = {
    GetAll: BaseURL + `/api/LaboratoryCategories/GetAll`,
    GetAllAsDrp: BaseURL + `/api/LaboratoryCategories/GetAllAsDrp`,
    GetLaboratoryCategoryDetails: BaseURL + `/api/LaboratoryCategories/GetLaboratoryCategoryDetails`,
    CreateLaboratoryCategory: BaseURL + `/api/LaboratoryCategories/CreateLaboratoryCategory`,
    UpdateLaboratoryCategory: BaseURL + `/api/LaboratoryCategories/UpdateLaboratoryCategory`,
    UpdateIsActive: BaseURL + `/api/LaboratoryCategories/UpdateIsActive`,
    UpdateIsActiveForSelected: BaseURL + `/api/LaboratoryCategories/UpdateIsActiveForSelected`,
    UpdateSharedForSelected: BaseURL + `/api/LaboratoryCategories/UpdateSharedForSelected`,
    RemoveLaboratoryCategory: BaseURL + `/api/LaboratoryCategories/RemoveLaboratoryCategory`,
    ImportLaboratoryCategories: BaseURL + `/api/LaboratoryCategories/ImportLaboratoryCategories`,
    ExportLaboratoryCategories: BaseURL + `/api/LaboratoryCategories/ExportLaboratoryCategories`,
}