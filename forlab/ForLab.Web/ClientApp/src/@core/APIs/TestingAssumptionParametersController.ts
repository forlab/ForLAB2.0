import { BaseURL } from '../config';

export const TestingAssumptionParametersController = {
  GetAll: BaseURL + `/api/TestingAssumptionParameters/GetAll`,
  GetAllAsDrp: BaseURL + `/api/TestingAssumptionParameters/GetAllAsDrp`,
  GetAllTestingAssumptionsForForcast: BaseURL + `/api/TestingAssumptionParameters/GetAllTestingAssumptionsForForcast`,
  GetTestingAssumptionParameterDetails: BaseURL + `/api/TestingAssumptionParameters/GetTestingAssumptionParameterDetails`,
  CreateTestingAssumptionParameter: BaseURL + `/api/TestingAssumptionParameters/CreateTestingAssumptionParameter`,
  UpdateTestingAssumptionParameter: BaseURL + `/api/TestingAssumptionParameters/UpdateTestingAssumptionParameter`,
  UpdateIsActive: BaseURL + `/api/TestingAssumptionParameters/UpdateIsActive`,
  RemoveTestingAssumptionParameter: BaseURL + `/api/TestingAssumptionParameters/RemoveTestingAssumptionParameter`,
  ImportTestingAssumptionParameters: BaseURL + `/api/TestingAssumptionParameters/ImportTestingAssumptionParameters`,
  ExportTestingAssumptionParameters: BaseURL + `/api/TestingAssumptionParameters/ExportTestingAssumptionParameters`,
}