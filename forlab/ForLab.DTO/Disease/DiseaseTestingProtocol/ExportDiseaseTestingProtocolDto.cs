using System;

namespace ForLab.DTO.Disease.DiseaseTestingProtocol
{
    public class ExportDiseaseTestingProtocolDto
    {
        public string DiseaseName { get; set; }
        public string TestingProtocolName { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
