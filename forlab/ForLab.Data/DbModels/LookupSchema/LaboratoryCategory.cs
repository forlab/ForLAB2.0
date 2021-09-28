using ForLab.Data.BaseModeling;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace ForLab.Data.DbModels.LookupSchema
{
    [Table("LaboratoryCategories", Schema = "Lookup")]
    public class LaboratoryCategory : DynamicLookup
    {
        public LaboratoryCategory()
        {
            Laboratories = new HashSet<Laboratory>();
        }
        public bool Shared { get; set; }
        public virtual ICollection<Laboratory> Laboratories { get; set; }
    }
}
