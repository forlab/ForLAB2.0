using Microsoft.AspNetCore.Identity;

namespace ForLab.Data.DbModels.SecuritySchema
{
    public class ApplicationUserToken: IdentityUserToken<int>
    {
        public virtual ApplicationUser User { get; set; }
    }
}
