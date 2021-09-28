using ForLab.DTO.Common;

namespace ForLab.DTO.DiseaseProgram.ProgramTest
{
    public class ProgramTestFilterDto : BaseFilterDto
    {
        public int ProgramId { get; set; }
        public int TestId { get; set; }
        public int TestingProtocolId { get; set; }
        public bool? FilterByProgramIds { get; set; }
        public string ProgramIds { get; set; }
        public string Name { get; set; }
    }
}
