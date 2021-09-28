using System;

namespace ForLab.DTO.DiseaseProgram.ProgramTest
{
    public class ExportProgramTestDto
    {
        public string ProgramName { get; set; }
        public string TestName { get; set; }
        public int? TestingProtocolBaseLine { get; set; }
        public int? TestingProtocolTestAfterFirstYear { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
