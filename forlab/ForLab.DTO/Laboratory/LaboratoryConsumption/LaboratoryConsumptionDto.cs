using ForLab.DTO.Common;
using System;

namespace ForLab.DTO.Laboratory.LaboratoryConsumption
{
    public class LaboratoryConsumptionDto : BaseEntityDto
    {
        public int LaboratoryId { get; set; }
        public int ProductId { get; set; }
        public DateTime ConsumptionDuration { get; set; } // it will be period which will be added manually by the user like jan 2020
        public decimal AmountUsed { get; set; }

        //UI
        public string ProductName { get; set; }
        public string LaboratoryName { get; set; }
        public int? LaboratoryRegionCountryId { get; set; }
        public int? LaboratoryRegionId { get; set; }
        public string LaboratoryRegionCountryName { get; set; }
        public string LaboratoryRegionName { get; set; }
    }
    public class LaboratoryConsumptionDrp : DropdownDrp
    {
        public string ProductName { get; set; }
        public string LaboratoryName { get; set; }
    }
}