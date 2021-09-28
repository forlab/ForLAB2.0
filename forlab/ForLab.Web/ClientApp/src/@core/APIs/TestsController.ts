import { BaseURL } from '../config';

export const TestsController = {
    GetAll: BaseURL + `/api/Tests/GetAll`,
    GetAllAsDrp: BaseURL + `/api/Tests/GetAllAsDrp`,
    GetTestDetails: BaseURL + `/api/Tests/GetTestDetails`,
    CreateTest: BaseURL + `/api/Tests/CreateTest`,
    UpdateTest: BaseURL + `/api/Tests/UpdateTest`,
    UpdateIsActive: BaseURL + `/api/Tests/UpdateIsActive`,
    UpdateIsActiveForSelected: BaseURL + `/api/Tests/UpdateIsActiveForSelected`,
    UpdateSharedForSelected: BaseURL + `/api/Tests/UpdateSharedForSelected`,
    RemoveTest: BaseURL + `/api/Tests/RemoveTest`,
    ImportTests: BaseURL + `/api/Tests/ImportTests`,
    ExportTests: BaseURL + `/api/Tests/ExportTests`,
}