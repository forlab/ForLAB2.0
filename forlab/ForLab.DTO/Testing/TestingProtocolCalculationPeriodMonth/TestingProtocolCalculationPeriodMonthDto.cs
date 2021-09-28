using ForLab.DTO.Common;

namespace ForLab.DTO.Testing.TestingProtocolCalculationPeriodMonth
{
    public class TestingProtocolCalculationPeriodMonthDto : BaseEntityDto
    {
        public int TestingProtocolId { get; set; }
        public int CalculationPeriodMonthId { get; set; }
        public decimal? Value { get; set; }

        //UI
        public string TestingProtocolName { get; set; }
        public string CalculationPeriodMonthName { get; set; }
    }
    public class TestingProtocolCalculationPeriodMonthDrp : DropdownDrp
    {
    }
}
