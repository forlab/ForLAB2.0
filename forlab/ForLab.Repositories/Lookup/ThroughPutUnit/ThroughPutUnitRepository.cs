using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.Lookup.ThroughPutUnit
{
    public class ThroughPutUnitRepository : GRepository<Data.DbModels.LookupSchema.ThroughPutUnit>, IThroughPutUnitRepository
    {
        private readonly AppDbContext _appDbContext;
        public ThroughPutUnitRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
