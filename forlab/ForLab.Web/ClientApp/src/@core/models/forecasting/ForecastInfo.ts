import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';
import { ForecastInstrumentDto } from './ForecastInstrument';
import { ForecastPatientGroupDto } from './ForecastPatientGroup';
import { ForecastTestDto } from './ForecastTest';
import { ForecastCategoryDto } from './ForecastCategory';
import { ForecastLaboratoryConsumptionDto } from './ForecastLaboratoryConsumption';
import { ForecastLaboratoryTestServiceDto } from './ForecastLaboratoryTestService';
import { ForecastLaboratoryDto } from './ForecastLaboratory';
import { ForecastMorbidityWhoBaseDto } from './ForecastMorbidityWhoBase';
import { ForecastPatientAssumptionValueDto } from './ForecastPatientAssumptionValue';
import { ForecastProductAssumptionValueDto } from './ForecastProductAssumptionValue';
import { ForecastTestingAssumptionValueDto } from './ForecastTestingAssumptionValue';
import { ForecastMorbidityTestingProtocolMonthDto } from './ForecastMorbidityTestingProtocolMonth';
import { HistoicalServiceDataDto, HistoicalConsumptionDto, HistoicalTargetBaseDto, HistoicalWhoBaseDto } from './ImportedFileTemplate';
import { ForecastMorbidityProgramDto } from './ForecastMorbidityProgram';
import { ForecastMorbidityTargetBaseDto } from './ForecastMorbidityTargetBase';

export class ForecastInfoDto extends BaseEntityDto {
  forecastInfoLevelId: number = null;
  countryId: number = null;
  forecastMethodologyId: number = null;
  scopeOfTheForecastId: number = null;
  name: string = null;
  startDate: string = null;
  endDate: string = null;
  duration: number = null;
  isAggregate: boolean = false;
  isSiteBySite: boolean = false;
  isWorldHealthOrganization: boolean = false;
  isTargetBased: boolean = false;
  wastageRate: number = null;
  
  // Children
  forecastInstrumentDtos: ForecastInstrumentDto[] = [];
  forecastPatientGroupDtos: ForecastPatientGroupDto[] = [];
  forecastTestDtos: ForecastTestDto[] = [];
  forecastCategoryDtos: ForecastCategoryDto[] =[];
  forecastLaboratoryConsumptionDtos: ForecastLaboratoryConsumptionDto[] = [];
  forecastLaboratoryTestServiceDtos: ForecastLaboratoryTestServiceDto[] = [];
  forecastLaboratoryDtos: ForecastLaboratoryDto[] = [];
  forecastMorbidityWhoBaseDtos: ForecastMorbidityWhoBaseDto[] = [];
  forecastPatientAssumptionValueDtos: ForecastPatientAssumptionValueDto[] = [];
  forecastProductAssumptionValueDtos: ForecastProductAssumptionValueDto[] = [];
  forecastTestingAssumptionValueDtos: ForecastTestingAssumptionValueDto[] = [];
  forecastMorbidityTestingProtocolMonthDtos: ForecastMorbidityTestingProtocolMonthDto[] = [];
  forecastMorbidityProgramDtos: ForecastMorbidityProgramDto[] = [];
  forecastMorbidityTargetBaseDtos: ForecastMorbidityTargetBaseDto[] = [];

  // UI
  forecastInfoLevelName: string = null;
  countryName: string = null;
  forecastMethodologyName: string = null;
  forecastInfoStatusName: string = null;
  scopeOfTheForecastName: string = null;
  histoicalConsumptionDtos: HistoicalConsumptionDto[] = [];
  histoicalServiceDataDtos: HistoicalServiceDataDto[] = [];
  histoicalTargetBaseDtos: HistoicalTargetBaseDto[] = [];
  histoicalWhoBaseDtos: HistoicalWhoBaseDto[] = [];
}


export class ForecastInfoFilterDto extends BaseFilter {
  forecastInfoLevelId: number = null;
  countryId: number = null;
  forecastMethodologyId: number = null;
  scopeOfTheForecastId: number = null;
  name: string = null;
  isAggregate: boolean = null;
  isTargetBased: boolean = null;
}