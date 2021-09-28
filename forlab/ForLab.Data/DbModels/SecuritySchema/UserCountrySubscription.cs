using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.LookupSchema;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.SecuritySchema
{
    [Table("UserCountrySubscriptions", Schema = "Security")]
    public class UserCountrySubscription: BaseEntity
    {
        public int ApplicationUserId { get; set; }
        public int CountryId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual Country Country { get; set; }
    }
}
