using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.LookupSchema;
using ForLab.Data.DbModels.ProductSchema;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.LaboratorySchema
{
    [Table("LaboratoryInstruments", Schema = "Laboratory")]
    public class LaboratoryInstrument : BaseEntity
    {
        public int InstrumentId { get; set; }
        public int LaboratoryId { get; set; }
        public int Quantity { get; set; }
        public decimal TestRunPercentage { get; set; }
        public virtual Instrument Instrument { get; set; }
        public virtual Laboratory Laboratory { get; set; }
    }
}
