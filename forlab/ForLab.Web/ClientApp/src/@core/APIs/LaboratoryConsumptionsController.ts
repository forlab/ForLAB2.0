import { BaseURL } from '../config';

export const LaboratoryConsumptionsController = {
    GetAll: BaseURL + `/api/LaboratoryConsumptions/GetAll`,
    GetAllAsDrp: BaseURL + `/api/LaboratoryConsumptions/GetAllAsDrp`,
    GetLaboratoryConsumptionDetails: BaseURL + `/api/LaboratoryConsumptions/GetLaboratoryConsumptionDetails`,
    CreateLaboratoryConsumption: BaseURL + `/api/LaboratoryConsumptions/CreateLaboratoryConsumption`,
    UpdateLaboratoryConsumption: BaseURL + `/api/LaboratoryConsumptions/UpdateLaboratoryConsumption`,
    UpdateIsActive: BaseURL + `/api/LaboratoryConsumptions/UpdateIsActive`,
    RemoveLaboratoryConsumption: BaseURL + `/api/LaboratoryConsumptions/RemoveLaboratoryConsumption`,
    ImportLaboratoryConsumptions: BaseURL + `/api/LaboratoryConsumptions/ImportLaboratoryConsumptions`,
    ExportLaboratoryConsumptions: BaseURL + `/api/LaboratoryConsumptions/ExportLaboratoryConsumptions`,
}