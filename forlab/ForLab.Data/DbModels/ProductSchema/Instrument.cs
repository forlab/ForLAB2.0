using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.ForecastingSchema;
using ForLab.Data.DbModels.LaboratorySchema;
using ForLab.Data.DbModels.LookupSchema;
using ForLab.Data.DbModels.VendorSchema;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.ProductSchema
{
    [Table("Instruments", Schema = "Product")]
    public class Instrument: BaseEntity
    {
        public Instrument()
        {
            LaboratoryInstruments = new HashSet<LaboratoryInstrument>();
            ProductUsages = new HashSet<ProductUsage>();
            ForecastInstruments = new HashSet<ForecastInstrument>();
        }
        public int VendorId { get; set; }
        public string Name { get; set; }
        public int MaxThroughPut { get; set; } // max daily or hourly
        public int ThroughPutUnitId { get; set; }
        public int ReagentSystemId { get; set; }
        public int ControlRequirement { get; set; }
        public int ControlRequirementUnitId { get; set; }
        public int TestingAreaId { get; set; }
        public bool Shared { get; set; }

        public virtual Vendor Vendor { get; set; }
        public virtual ThroughPutUnit ThroughPutUnit { get; set; }
        public virtual ReagentSystem ReagentSystem { get; set; }
        public virtual ControlRequirementUnit ControlRequirementUnit { get; set; }
        public virtual TestingArea TestingArea { get; set; }
        public virtual ICollection<LaboratoryInstrument> LaboratoryInstruments { get; set; }
        public virtual ICollection<ProductUsage> ProductUsages { get; set; }
        public virtual ICollection<ForecastInstrument> ForecastInstruments { get; set; }
    }
}
