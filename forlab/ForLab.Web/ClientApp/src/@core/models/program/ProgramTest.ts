import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';
import { TestingProtocolDto } from '../testing/TestingProtocol';

export class ProgramTestDto extends BaseEntityDto {
  programId: number = null;
  testId: number = null;
  testingProtocolId: number = 0;

  // UI
  testingProtocolDto: TestingProtocolDto = new TestingProtocolDto();
  programName: string = null;
  testName: string = null;
  testingProtocolPatientGroupName: string = null;
  testingProtocolPatientGroupId: number = null;
}


export class ProgramTestFilterDto extends BaseFilter {
  programId: number = null;
  testId: number = null;
  testingProtocolId: number = null;
  filterByProgramIds: boolean = null;
  programIds: string = null;
}