using ForLab.DTO.Common;
using ForLab.DTO.Testing.TestingProtocol;

namespace ForLab.DTO.DiseaseProgram.ProgramTest
{
    public class ProgramTestDto : BaseEntityDto
    {
        public int ProgramId { get; set; }
        public int TestId { get; set; }
        public int TestingProtocolId { get; set; }

        // UI
        public TestingProtocolDto TestingProtocolDto { get; set; }
        public string ProgramName { get; set; }
        public string TestName { get; set; }
        public string TestingProtocolName { get; set; }
        public int? TestingProtocolPatientGroupId { get; set; }
        public string TestingProtocolPatientGroupName { get; set; }
        public string TestingProtocolCalculationPeriodName { get; set; }
        public int? TestingProtocolBaseLine { get; set; }
        public int? TestingProtocolTestAfterFirstYear { get; set; }
    }
    public class ProgramTestDrp : DropdownDrp
    {
        public string ProgramName { get; set; }
        public string TestName { get; set; }
        public string TestingProtocolPatientGroupName { get; set; }
        public int? TestingProtocolPatientGroupId { get; set; }
        public TestingProtocolDto TestingProtocolDto { get; set; }
    }
}
