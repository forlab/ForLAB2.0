using System;

namespace ForLab.DTO.Laboratory.LaboratoryTestService
{
    public class ExportLaboratoryTestServiceDto
    {
        public string TestName { get; set; }
        public string LaboratoryRegionName { get; set; }
        public string LaboratoryName { get; set; }
        public DateTime ServiceDuration { get; set; }
        public decimal TestPerformed { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
