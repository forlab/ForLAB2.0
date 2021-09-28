import { BaseURL } from '../config';

export const ProgramsController = {
  GetAll: BaseURL + `/api/Programs/GetAll`,
  GetAllAsDrp: BaseURL + `/api/Programs/GetAllAsDrp`,
  GetProgramDetails: BaseURL + `/api/Programs/GetProgramDetails`,
  GetProgramDetailsForForcast: BaseURL + `/api/Programs/GetProgramDetailsForForcast`,
  CreateProgram: BaseURL + `/api/Programs/CreateProgram`,
  UpdateProgram: BaseURL + `/api/Programs/UpdateProgram`,
  UpdateIsActive: BaseURL + `/api/Programs/UpdateIsActive`,
  RemoveProgram: BaseURL + `/api/Programs/RemoveProgram`,
  ImportPrograms: BaseURL + `/api/Programs/ImportPrograms`,
  ExportPrograms: BaseURL + `/api/Programs/ExportPrograms`,
}