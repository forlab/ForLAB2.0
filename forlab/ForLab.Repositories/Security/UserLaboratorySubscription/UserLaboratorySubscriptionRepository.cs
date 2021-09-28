using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.Security.UserLaboratorySubscription
{
    public class UserLaboratorySubscriptionRepository : GRepository<Data.DbModels.SecuritySchema.UserLaboratorySubscription>, IUserLaboratorySubscriptionRepository
    {
        private readonly AppDbContext _appDbContext;
        public UserLaboratorySubscriptionRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
