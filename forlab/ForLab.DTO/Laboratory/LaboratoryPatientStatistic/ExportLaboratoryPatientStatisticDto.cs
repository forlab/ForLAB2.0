using System;

namespace ForLab.DTO.Laboratory.LaboratoryPatientStatistic
{
    public class ExportLaboratoryPatientStatisticDto
    {
        public DateTime Period { get; set; }
        public int Count { get; set; }
        public string LaboratoryRegionName { get; set; }
        public string LaboratoryName { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
