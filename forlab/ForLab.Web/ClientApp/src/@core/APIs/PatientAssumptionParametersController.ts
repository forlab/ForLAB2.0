import { BaseURL } from '../config';

export const PatientAssumptionParametersController = {
  GetAll: BaseURL + `/api/PatientAssumptionParameters/GetAll`,
  GetAllAsDrp: BaseURL + `/api/PatientAssumptionParameters/GetAllAsDrp`,
  GetAllPatientAssumptionsForForcast: BaseURL + `/api/PatientAssumptionParameters/GetAllPatientAssumptionsForForcast`,
  GetPatientAssumptionParameterDetails: BaseURL + `/api/PatientAssumptionParameters/GetPatientAssumptionParameterDetails`,
  CreatePatientAssumptionParameter: BaseURL + `/api/PatientAssumptionParameters/CreatePatientAssumptionParameter`,
  UpdatePatientAssumptionParameter: BaseURL + `/api/PatientAssumptionParameters/UpdatePatientAssumptionParameter`,
  UpdateIsActive: BaseURL + `/api/PatientAssumptionParameters/UpdateIsActive`,
  RemovePatientAssumptionParameter: BaseURL + `/api/PatientAssumptionParameters/RemovePatientAssumptionParameter`,
  ImportPatientAssumptionParameters: BaseURL + `/api/PatientAssumptionParameters/ImportPatientAssumptionParameters`,
  ExportPatientAssumptionParameters: BaseURL + `/api/PatientAssumptionParameters/ExportPatientAssumptionParameters`,
}