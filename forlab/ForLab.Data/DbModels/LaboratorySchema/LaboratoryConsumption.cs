using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.LookupSchema;
using ForLab.Data.DbModels.ProductSchema;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.LaboratorySchema
{
    [Table("LaboratoryConsumptions", Schema = "Laboratory")]
    public class LaboratoryConsumption : BaseEntity
    {
        public int LaboratoryId { get; set; }
        public int ProductId { get; set; }
        public DateTime ConsumptionDuration { get; set; } // it will be period which will be added manually by the user like jan 2020
        public decimal AmountUsed { get; set; }

        public virtual Product Product { get; set; }
        public virtual Laboratory Laboratory { get; set; }
    }
}
