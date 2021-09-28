using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.LookupSchema;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.SecuritySchema
{
    [Table("UserRegionSubscriptions", Schema = "Security")]
    public class UserRegionSubscription : BaseEntity
    {
        public int ApplicationUserId { get; set; }
        public int RegionId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual Region Region { get; set; }
    }
}
