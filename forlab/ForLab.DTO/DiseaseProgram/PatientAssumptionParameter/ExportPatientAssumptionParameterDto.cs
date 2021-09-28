using System;

namespace ForLab.DTO.DiseaseProgram.PatientAssumptionParameter
{
    public class ExportPatientAssumptionParameterDto
    {
        public string Name { get; set; }
        public string ProgramName { get; set; }

        // forecast method
        public bool IsNumeric { get; set; }
        public bool IsPercentage { get; set; }

        // variable effect
        public bool IsPositive { get; set; }
        public bool IsNegative { get; set; }

        public string Creator { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
