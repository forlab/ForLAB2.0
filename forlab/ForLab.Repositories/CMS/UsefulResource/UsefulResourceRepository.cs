using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.CMS.UsefulResource
{
    public class UsefulResourceRepository : GRepository<Data.DbModels.CMSSchema.UsefulResource>, IUsefulResourceRepository
    {
        private readonly AppDbContext _appDbContext;
        public UsefulResourceRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
