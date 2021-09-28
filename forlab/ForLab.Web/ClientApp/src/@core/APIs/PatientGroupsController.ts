import { BaseURL } from '../config';

export const PatientGroupsController = {
    GetAll: BaseURL + `/api/PatientGroups/GetAll`,
    GetAllAsDrp: BaseURL + `/api/PatientGroups/GetAllAsDrp`,
    GetPatientGroupDetails: BaseURL + `/api/PatientGroups/GetPatientGroupDetails`,
    CreatePatientGroup: BaseURL + `/api/PatientGroups/CreatePatientGroup`,
    UpdatePatientGroup: BaseURL + `/api/PatientGroups/UpdatePatientGroup`,
    UpdateIsActive: BaseURL + `/api/PatientGroups/UpdateIsActive`,
    UpdateIsActiveForSelected: BaseURL + `/api/PatientGroups/UpdateIsActiveForSelected`,
    UpdateSharedForSelected: BaseURL + `/api/PatientGroups/UpdateSharedForSelected`,
    RemovePatientGroup: BaseURL + `/api/PatientGroups/RemovePatientGroup`,
    ImportPatientGroups: BaseURL + `/api/PatientGroups/ImportPatientGroups`,
    ExportPatientGroups: BaseURL + `/api/PatientGroups/ExportPatientGroups`,
}