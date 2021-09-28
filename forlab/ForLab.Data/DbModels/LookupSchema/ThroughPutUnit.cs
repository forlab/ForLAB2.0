using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.ProductSchema;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.LookupSchema
{
    [Table("ThroughPutUnits", Schema = "Lookup")]
    public class ThroughPutUnit: StaticLookup
    {
        // for example hourly, daily enums
        public ThroughPutUnit()
        {
            Instruments = new HashSet<Instrument>();
        }
        public virtual ICollection<Instrument> Instruments { get; set; }
    }
}
