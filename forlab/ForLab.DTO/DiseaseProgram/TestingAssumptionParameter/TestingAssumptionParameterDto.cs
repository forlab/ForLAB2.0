using ForLab.DTO.Common;
using System.Collections.Generic;

namespace ForLab.DTO.DiseaseProgram.TestingAssumptionParameter
{
    public class TestingAssumptionParameterDto : BaseEntityDto
    {
        public int ProgramId { get; set; }
        public string Name { get; set; }

        // forecast method
        public bool IsNumeric { get; set; }
        public bool IsPercentage { get; set; }

        // variable effect
        public bool IsPositive { get; set; }
        public bool IsNegative { get; set; }

        //UI
        public string ProgramName { get; set; }
    }
    public class TestingAssumptionParameterDrp : DropdownDrp
    {
    }

    public class GroupTestingAssumptionParameterDto
    {
        public string ProgramName { get; set; }
        public List<TestingAssumptionParameterDto> TestingAssumptionParameterDtos { get; set; }
    }
}
