using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.CMS.Feature
{
    public class FeatureRepository : GRepository<Data.DbModels.CMSSchema.Feature>, IFeatureRepository
    {
        private readonly AppDbContext _appDbContext;
        public FeatureRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
