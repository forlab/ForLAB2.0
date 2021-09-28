

using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.Security.UserTransactionHistory
{
    public class UserTransactionHistoryRepository : GRepository<ForLab.Data.DbModels.SecuritySchema.UserTransactionHistory>, IUserTransactionHistoryRepository
    {
        private readonly AppDbContext _appDbContext;
        public UserTransactionHistoryRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
