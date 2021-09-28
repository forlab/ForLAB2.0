using System;

namespace ForLab.DTO.Laboratory.LaboratoryWorkingDay
{
    public class ExportLaboratoryWorkingDayDto
    {
        public string Day { get; set; }
        public TimeSpan FromTime { get; set; }
        public TimeSpan ToTime { get; set; }
        public string LaboratoryRegionName { get; set; }
        public string LaboratoryName { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
