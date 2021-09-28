import { BaseURL } from '../config';

export const InstrumentsController = {
    GetAll: BaseURL + `/api/Instruments/GetAll`,
    GetAllAsDrp: BaseURL + `/api/Instruments/GetAllAsDrp`,
    GetInstrumentDetails: BaseURL + `/api/Instruments/GetInstrumentDetails`,
    CreateInstrument: BaseURL + `/api/Instruments/CreateInstrument`,
    UpdateInstrument: BaseURL + `/api/Instruments/UpdateInstrument`,
    UpdateIsActive: BaseURL + `/api/Instruments/UpdateIsActive`,
    UpdateIsActiveForSelected: BaseURL + `/api/Instruments/UpdateIsActiveForSelected`,
    UpdateSharedForSelected: BaseURL + `/api/Instruments/UpdateSharedForSelected`,
    RemoveInstrument: BaseURL + `/api/Instruments/RemoveInstrument`,
    ImportInstruments: BaseURL + `/api/Instruments/ImportInstruments`,
    ExportInstruments: BaseURL + `/api/Instruments/ExportInstruments`,
}