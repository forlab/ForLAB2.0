using System;

namespace ForLab.DTO.Laboratory.LaboratoryConsumption
{
    public class ExportLaboratoryConsumptionDto
    {
        public DateTime ConsumptionDuration { get; set; }
        public decimal AmountUsed { get; set; }
        public string ProductName { get; set; }
        public string LaboratoryRegionName { get; set; }
        public string LaboratoryName { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}

