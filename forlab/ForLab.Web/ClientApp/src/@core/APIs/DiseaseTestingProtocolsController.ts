import { BaseURL } from '../config';

export const DiseaseTestingProtocolsController = {
    GetAll: BaseURL + `/api/DiseaseTestingProtocols/GetAll`,
    GetAllAsDrp: BaseURL + `/api/DiseaseTestingProtocols/GetAllAsDrp`,
    GetDiseaseTestingProtocolDetails: BaseURL + `/api/DiseaseTestingProtocols/GetDiseaseTestingProtocolDetails`,
    CreateDiseaseTestingProtocol: BaseURL + `/api/DiseaseTestingProtocols/CreateDiseaseTestingProtocol`,
    UpdateDiseaseTestingProtocol: BaseURL + `/api/DiseaseTestingProtocols/UpdateDiseaseTestingProtocol`,
    UpdateIsActive: BaseURL + `/api/DiseaseTestingProtocols/UpdateIsActive`,
    RemoveDiseaseTestingProtocol: BaseURL + `/api/DiseaseTestingProtocols/RemoveDiseaseTestingProtocol`,
    ImportDiseaseTestingProtocols: BaseURL + `/api/DiseaseTestingProtocols/ImportDiseaseTestingProtocols`,
    ExportDiseaseTestingProtocols: BaseURL + `/api/DiseaseTestingProtocols/ExportDiseaseTestingProtocols`,
}