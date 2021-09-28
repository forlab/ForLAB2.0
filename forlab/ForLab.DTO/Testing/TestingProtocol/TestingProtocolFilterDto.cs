using ForLab.DTO.Common;

namespace ForLab.DTO.Testing.TestingProtocol
{
    public class TestingProtocolFilterDto : BaseFilterDto
    {
        public string Name { get; set; }
        public int TestId { get; set; }
        public int PatientGroupId { get; set; }
        public int CalculationPeriodId { get; set; }
        public int BaseLine { get; set; }
        public int TestAfterFirstYear { get; set; }
    }
}
