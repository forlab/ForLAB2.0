using ForLab.Data.BaseModeling;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.LookupSchema
{
    [Table("LaboratoryLevels", Schema = "Lookup")]
    public class LaboratoryLevel : DynamicLookup
    {
        public LaboratoryLevel()
        {
            Laboratories = new HashSet<Laboratory>();
        }
        public bool Shared { get; set; }
        public virtual ICollection<Laboratory> Laboratories { get; set; }
    }
}
