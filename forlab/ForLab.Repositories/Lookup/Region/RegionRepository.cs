using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.Lookup.Region
{
    public class RegionRepository : GRepository<Data.DbModels.LookupSchema.Region>, IRegionRepository
    {
        private readonly AppDbContext _appDbContext;
        public RegionRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}