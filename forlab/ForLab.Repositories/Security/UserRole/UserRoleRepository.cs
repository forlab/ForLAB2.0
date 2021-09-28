using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;


namespace ForLab.Repositories.Security.UserRole
{
    public class UserRoleRepository : GRepository<Data.DbModels.SecuritySchema.ApplicationUserRole>, IUserRoleRepository
    {
        private readonly AppDbContext _appDbContext;
        public UserRoleRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
