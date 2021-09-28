using ForLab.DTO.Common;

namespace ForLab.DTO.DiseaseProgram.Program
{
    public class ProgramFilterDto : BaseFilterDto
    {
        public int DiseaseId { get; set; }
        public string Name { get; set; }
        public int NumberOfYears { get; set; }

    }
}
