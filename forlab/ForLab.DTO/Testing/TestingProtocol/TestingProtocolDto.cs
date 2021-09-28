using ForLab.DTO.Common;
using ForLab.DTO.Testing.TestingProtocolCalculationPeriodMonth;
using System.Collections.Generic;

namespace ForLab.DTO.Testing.TestingProtocol
{
    public class TestingProtocolDto : BaseEntityDto
    {
        public string Name { get; set; }
        public int TestId { get; set; }
        public int PatientGroupId { get; set; }
        public int CalculationPeriodId { get; set; }
        public int BaseLine { get; set; }
        public int TestAfterFirstYear { get; set; }
        public List<TestingProtocolCalculationPeriodMonthDto> TestingProtocolCalculationPeriodMonthDtos { get; set; }

        // UI
        public string TestName { get; set; }
        public string PatientGroupName { get; set; }
        public string CalculationPeriodName { get; set; }
    }
    public class TestingProtocolDrp : DropdownDrp
    {
        public string TestName { get; set; }
        public string PatientGroupName { get; set; }
        public string CalculationPeriodName { get; set; }
        public int BaseLine { get; set; }
    }
}
