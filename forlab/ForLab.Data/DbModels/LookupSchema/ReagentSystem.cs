using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.ProductSchema;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.LookupSchema
{
    [Table("ReagentSystems", Schema = "Lookup")]
    public class ReagentSystem : StaticLookup
    {
        //open, closed, PartiallyOpen
        public ReagentSystem()
        {
            Instruments = new HashSet<Instrument>();
        }
        public virtual ICollection<Instrument> Instruments { get; set; }
    }
}
