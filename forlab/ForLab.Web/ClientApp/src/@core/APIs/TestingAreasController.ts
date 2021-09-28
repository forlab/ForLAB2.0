import { BaseURL } from '../config';

export const TestingAreasController = {
    GetAll: BaseURL + `/api/TestingAreas/GetAll`,
    GetAllAsDrp: BaseURL + `/api/TestingAreas/GetAllAsDrp`,
    GetTestingAreaDetails: BaseURL + `/api/TestingAreas/GetTestingAreaDetails`,
    CreateTestingArea: BaseURL + `/api/TestingAreas/CreateTestingArea`,
    UpdateTestingArea: BaseURL + `/api/TestingAreas/UpdateTestingArea`,
    UpdateIsActive: BaseURL + `/api/TestingAreas/UpdateIsActive`,
    UpdateIsActiveForSelected: BaseURL + `/api/TestingAreas/UpdateIsActiveForSelected`,
    UpdateSharedForSelected: BaseURL + `/api/TestingAreas/UpdateSharedForSelected`,
    RemoveTestingArea: BaseURL + `/api/TestingAreas/RemoveTestingArea`,
    ImportTestingAreas: BaseURL + `/api/TestingAreas/ImportTestingAreas`,
    ExportTestingAreas: BaseURL + `/api/TestingAreas/ExportTestingAreas`,
}