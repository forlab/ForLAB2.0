using Microsoft.AspNetCore.Identity;

namespace ForLab.Data.DbModels.SecuritySchema
{
    public class ApplicationRoleClaim: IdentityRoleClaim<int>
    {
        public virtual ApplicationRole Role { get; set; }
    }
}
