using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.SecuritySchema;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.LookupSchema
{
    [Table("UserSubscriptionLevels", Schema = "Lookup")]
    public class UserSubscriptionLevel : StaticLookup
    {
        // country ,region, laboratory
        public UserSubscriptionLevel()
        {
            ApplicationUsers = new HashSet<ApplicationUser>();
        }
        public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }
    }
}
