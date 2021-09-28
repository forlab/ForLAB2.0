using ForLab.DTO.Common;
using ForLab.DTO.DiseaseProgram.PatientAssumptionParameter;
using ForLab.DTO.DiseaseProgram.ProductAssumptionParameter;
using ForLab.DTO.DiseaseProgram.ProgramTest;
using ForLab.DTO.DiseaseProgram.TestingAssumptionParameter;
using System.Collections.Generic;

namespace ForLab.DTO.DiseaseProgram.Program
{
    public class ProgramDto : BaseEntityDto
    {
        public int DiseaseId { get; set; }
        public string Name { get; set; }
        public int NumberOfYears { get; set; }

        public List<PatientAssumptionParameterDto> PatientAssumptionParameterDtos { get; set; }
        public List<ProductAssumptionParameterDto> ProductAssumptionParameterDtos { get; set; }
        public List<TestingAssumptionParameterDto> TestingAssumptionParameterDtos { get; set; }
        public List<ProgramTestDto> ProgramTestDtos { get; set; }

        //UI
        public string DiseaseName { get; set; }

    }
    public class ProgramDrp : DropdownDrp
    {
    }
}
