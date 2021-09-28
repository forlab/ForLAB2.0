using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.LookupSchema;
using ForLab.Data.DbModels.TestingSchema;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.ProductSchema
{
    [Table("ProductUsage", Schema = "Product")]
    public class ProductUsage : BaseEntity
    {
        public int? TestId { get; set; } // will be null in case (consumables, calibarators)
        public int ProductId { get; set; }
        public bool IsForControl { get; set; } // incase of test usage and product type is QC
        public decimal Amount { get; set; }
        public bool PerPeriod { get; set; } // incase of product usage
        public bool PerPeriodPerInstrument { get; set; } // incase of product usage
        public int? CountryPeriodId { get; set; } // incase of product usage, it should be active if per period
        public int? InstrumentId { get; set; } //  it should be active if PerPeriodPerInstrument

        public virtual Product Product { get; set; }
        public virtual Instrument Instrument { get; set; }
        public virtual Test Test { get; set; }
        public virtual CountryPeriod CountryPeriod { get; set; }
    }
}
