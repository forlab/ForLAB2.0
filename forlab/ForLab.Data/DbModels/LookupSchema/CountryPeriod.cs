using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.ProductSchema;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.LookupSchema
{
    [Table("CountryPeriods", Schema = "Lookup")]
    public class CountryPeriod : StaticLookup
    {
        // Weekly, Monthly, Quarterly, Annualy
        public CountryPeriod()
        {
            Countries = new HashSet<Country>();
            ProductUsages = new HashSet<ProductUsage>();
        }
        public virtual ICollection<Country> Countries { get; set; }
        public virtual ICollection<ProductUsage> ProductUsages { get; set; }
    }
}
