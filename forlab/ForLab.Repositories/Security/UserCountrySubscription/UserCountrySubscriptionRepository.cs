using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.Security.UserCountrySubscription
{
    public class UserCountrySubscriptionRepository : GRepository<Data.DbModels.SecuritySchema.UserCountrySubscription>, IUserCountrySubscriptionRepository
    {
        private readonly AppDbContext _appDbContext;
        public UserCountrySubscriptionRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
