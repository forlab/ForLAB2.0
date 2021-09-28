using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.SecuritySchema;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.LookupSchema
{
    [Table("UserTransactionTypes", Schema = "lookup")]
    public class UserTransactionType : StaticLookup
    {
        public UserTransactionType()
        {
            UserTransactionHistories = new HashSet<UserTransactionHistory>();
        }
        public virtual ICollection<UserTransactionHistory> UserTransactionHistories { get; set; }
    }
}
