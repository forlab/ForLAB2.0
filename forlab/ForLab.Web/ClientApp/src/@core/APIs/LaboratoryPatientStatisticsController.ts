import { BaseURL } from '../config';

export const LaboratoryPatientStatisticsController = {
    GetAll: BaseURL + `/api/LaboratoryPatientStatistics/GetAll`,
    GetAllAsDrp: BaseURL + `/api/LaboratoryPatientStatistics/GetAllAsDrp`,
    GetLaboratoryPatientStatisticDetails: BaseURL + `/api/LaboratoryPatientStatistics/GetLaboratoryPatientStatisticDetails`,
    CreateLaboratoryPatientStatistic: BaseURL + `/api/LaboratoryPatientStatistics/CreateLaboratoryPatientStatistic`,
    UpdateLaboratoryPatientStatistic: BaseURL + `/api/LaboratoryPatientStatistics/UpdateLaboratoryPatientStatistic`,
    UpdateIsActive: BaseURL + `/api/LaboratoryPatientStatistics/UpdateIsActive`,
    RemoveLaboratoryPatientStatistic: BaseURL + `/api/LaboratoryPatientStatistics/RemoveLaboratoryPatientStatistic`,
    ImportLaboratoryPatientStatistics: BaseURL + `/api/LaboratoryPatientStatistics/ImportLaboratoryPatientStatistics`,
    ExportLaboratoryPatientStatistics: BaseURL + `/api/LaboratoryPatientStatistics/ExportLaboratoryPatientStatistics`,
}