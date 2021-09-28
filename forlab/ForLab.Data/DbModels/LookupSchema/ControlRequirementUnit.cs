using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.ProductSchema;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.LookupSchema
{
    [Table("ControlRequirementUnits", Schema = "Lookup")]
    public class ControlRequirementUnit : StaticLookup
    {
        // Daily, Weekly, Monthly
        public ControlRequirementUnit()
        {
            Instruments = new HashSet<Instrument>();
        }

        public virtual ICollection<Instrument> Instruments { get; set; }
    }
}
