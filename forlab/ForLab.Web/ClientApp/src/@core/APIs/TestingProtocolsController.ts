import { BaseURL } from '../config';

export const TestingProtocolsController = {
    GetAll: BaseURL + `/api/TestingProtocols/GetAll`,
    GetAllAsDrp: BaseURL + `/api/TestingProtocols/GetAllAsDrp`,
    GetTestingProtocolDetails: BaseURL + `/api/TestingProtocols/GetTestingProtocolDetails`,
    ExportTestingProtocols: BaseURL + `/api/TestingProtocols/ExportTestingProtocols`,
}