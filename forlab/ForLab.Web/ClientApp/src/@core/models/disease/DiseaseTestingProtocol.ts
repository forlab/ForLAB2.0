import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';

export class DiseaseTestingProtocolDto extends BaseEntityDto {
  diseaseId: number = null;
  testingProtocolId: number = null;
  
  // UI
  diseaseName: string = null;
  testingProtocolName: string = null;
}


export class DiseaseTestingProtocolFilterDto extends BaseFilter {
  diseaseId: number = null;
  testingProtocolId: number = null;
}