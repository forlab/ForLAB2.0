import { BaseURL } from '../config';

export const LaboratoryTestServicesController = {
    GetAll: BaseURL + `/api/LaboratoryTestServices/GetAll`,
    GetAllAsDrp: BaseURL + `/api/LaboratoryTestServices/GetAllAsDrp`,
    GetLaboratoryTestServiceDetails: BaseURL + `/api/LaboratoryTestServices/GetLaboratoryTestServiceDetails`,
    CreateLaboratoryTestService: BaseURL + `/api/LaboratoryTestServices/CreateLaboratoryTestService`,
    UpdateLaboratoryTestService: BaseURL + `/api/LaboratoryTestServices/UpdateLaboratoryTestService`,
    UpdateIsActive: BaseURL + `/api/LaboratoryTestServices/UpdateIsActive`,
    RemoveLaboratoryTestService: BaseURL + `/api/LaboratoryTestServices/RemoveLaboratoryTestService`,
    ImportLaboratoryTestServices: BaseURL + `/api/LaboratoryTestServices/ImportLaboratoryTestServices`,
    ExportLaboratoryTestServices: BaseURL + `/api/LaboratoryTestServices/ExportLaboratoryTestServices`,
}