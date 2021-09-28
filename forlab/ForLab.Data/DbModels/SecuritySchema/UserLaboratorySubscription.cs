using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.LookupSchema;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.SecuritySchema
{
    [Table("UserLaboratorySubscriptions", Schema = "Security")]
    public class UserLaboratorySubscription : BaseEntity
    {
        public int ApplicationUserId { get; set; }
        public int LaboratoryId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual Laboratory Laboratory { get; set; }
    }
}
