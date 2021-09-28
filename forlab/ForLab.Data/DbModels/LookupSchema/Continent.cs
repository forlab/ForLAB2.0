using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.SecuritySchema;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.LookupSchema
{
    [Table("Continents", Schema = "Lookup")]
    public class Continent: StaticLookup
    {
        public Continent()
        {
            Countries = new HashSet<Country>();
            ApplicationUsers = new HashSet<ApplicationUser>();
        }
        public string ShortCode { get; set; }

        public virtual ICollection<Country> Countries { get; set; }
        public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }
    }
}
