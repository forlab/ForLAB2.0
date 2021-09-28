using System;

namespace ForLab.DTO.DiseaseProgram.Program
{
    public class ExportProgramDto
    {
        public string DiseaseName { get; set; }
        public string Name { get; set; }
        public int NumberOfYears { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
