import { BaseURL } from '../config';

export const ProgramTestsController = {
    GetAll: BaseURL + `/api/ProgramTests/GetAll`,
    GetAllAsDrp: BaseURL + `/api/ProgramTests/GetAllAsDrp`,
    GetProgramTestDetails: BaseURL + `/api/ProgramTests/GetProgramTestDetails`,
    CreateProgramTest: BaseURL + `/api/ProgramTests/CreateProgramTest`,
    UpdateProgramTest: BaseURL + `/api/ProgramTests/UpdateProgramTest`,
    UpdateIsActive: BaseURL + `/api/ProgramTests/UpdateIsActive`,
    RemoveProgramTest: BaseURL + `/api/ProgramTests/RemoveProgramTest`,
    ImportProgramTests: BaseURL + `/api/ProgramTests/ImportProgramTests`,
    ExportProgramTests: BaseURL + `/api/ProgramTests/ExportProgramTests`,
}