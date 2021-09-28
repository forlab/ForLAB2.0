using ForLab.DTO.Common;

namespace ForLab.DTO.DiseaseProgram.ProductAssumptionParameter
{
    public class ProductAssumptionParameterFilterDto : BaseFilterDto
    {
        public int ProgramId { get; set; }
        public string Name { get; set; }

        // forecast method
        public bool? IsNumeric { get; set; }
        public bool? IsPercentage { get; set; }

        // variable effect
        public bool? IsPositive { get; set; }
        public bool? IsNegative { get; set; }
    }
}
