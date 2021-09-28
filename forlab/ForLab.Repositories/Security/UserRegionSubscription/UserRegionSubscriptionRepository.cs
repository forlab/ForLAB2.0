using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.Security.UserRegionSubscription
{
    public class UserRegionSubscriptionRepository : GRepository<Data.DbModels.SecuritySchema.UserRegionSubscription>, IUserRegionSubscriptionRepository
    {
        private readonly AppDbContext _appDbContext;
        public UserRegionSubscriptionRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
