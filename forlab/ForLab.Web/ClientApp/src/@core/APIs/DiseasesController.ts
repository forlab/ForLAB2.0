import { BaseURL } from '../config';

export const DiseasesController = {
  GetAll: BaseURL + `/api/Diseases/GetAll`,
  GetAllAsDrp: BaseURL + `/api/Diseases/GetAllAsDrp`,
  GetDiseaseDetails: BaseURL + `/api/Diseases/GetDiseaseDetails`,
  CreateDisease: BaseURL + `/api/Diseases/CreateDisease`,
  UpdateDisease: BaseURL + `/api/Diseases/UpdateDisease`,
  UpdateIsActive: BaseURL + `/api/Diseases/UpdateIsActive`,
  UpdateIsActiveForSelected: BaseURL + `/api/Diseases/UpdateIsActiveForSelected`,
  RemoveDisease: BaseURL + `/api/Diseases/RemoveDisease`,
  ImportDiseases: BaseURL + `/api/Diseases/ImportDiseases`,
  ExportDiseases: BaseURL + `/api/Diseases/ExportDiseases`,
}