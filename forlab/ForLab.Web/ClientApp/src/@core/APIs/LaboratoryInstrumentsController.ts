import { BaseURL } from '../config';

export const LaboratoryInstrumentsController = {
    GetAll: BaseURL + `/api/LaboratoryInstruments/GetAll`,
    GetAllAsDrp: BaseURL + `/api/LaboratoryInstruments/GetAllAsDrp`,
    GetLaboratoryInstrumentDetails: BaseURL + `/api/LaboratoryInstruments/GetLaboratoryInstrumentDetails`,
    CreateLaboratoryInstrument: BaseURL + `/api/LaboratoryInstruments/CreateLaboratoryInstrument`,
    UpdateLaboratoryInstrument: BaseURL + `/api/LaboratoryInstruments/UpdateLaboratoryInstrument`,
    UpdateIsActive: BaseURL + `/api/LaboratoryInstruments/UpdateIsActive`,
    RemoveLaboratoryInstrument: BaseURL + `/api/LaboratoryInstruments/RemoveLaboratoryInstrument`,
    ImportLaboratoryInstruments: BaseURL + `/api/LaboratoryInstruments/ImportLaboratoryInstruments`,
    ExportLaboratoryInstruments: BaseURL + `/api/LaboratoryInstruments/ExportLaboratoryInstruments`,
}