import { BaseURL } from '../config';

export const LaboratoryWorkingDaysController = {
    GetAll: BaseURL + `/api/LaboratoryWorkingDays/GetAll`,
    GetAllAsDrp: BaseURL + `/api/LaboratoryWorkingDays/GetAllAsDrp`,
    GetLaboratoryWorkingDayDetails: BaseURL + `/api/LaboratoryWorkingDays/GetLaboratoryWorkingDayDetails`,
    CreateLaboratoryWorkingDay: BaseURL + `/api/LaboratoryWorkingDays/CreateLaboratoryWorkingDay`,
    UpdateLaboratoryWorkingDay: BaseURL + `/api/LaboratoryWorkingDays/UpdateLaboratoryWorkingDay`,
    UpdateIsActive: BaseURL + `/api/LaboratoryWorkingDays/UpdateIsActive`,
    RemoveLaboratoryWorkingDay: BaseURL + `/api/LaboratoryWorkingDays/RemoveLaboratoryWorkingDay`,
    ImportLaboratoryWorkingDays: BaseURL + `/api/LaboratoryWorkingDays/ImportLaboratoryWorkingDays`,
    ExportLaboratoryWorkingDays: BaseURL + `/api/LaboratoryWorkingDays/ExportLaboratoryWorkingDays`,
}