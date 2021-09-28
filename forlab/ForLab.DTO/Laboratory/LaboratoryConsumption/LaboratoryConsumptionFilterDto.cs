using ForLab.DTO.Common;
using System;

namespace ForLab.DTO.Laboratory.LaboratoryConsumption
{
    public class LaboratoryConsumptionFilterDto : BaseFilterDto
    {
      
        public int LaboratoryId { get; set; }
        public int ProductId { get; set; }
        public DateTime? ConsumptionDuration { get; set; } // it will be period which will be added manually by the user like jan 2020
        public decimal? AmountUsed { get; set; }
    }
}
