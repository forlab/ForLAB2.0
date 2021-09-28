using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.Lookup.LaboratoryLevel
{
    public class LaboratoryLevelRepository : GRepository<Data.DbModels.LookupSchema.LaboratoryLevel>, ILaboratoryLevelRepository
    {
        private readonly AppDbContext _appDbContext;
        public LaboratoryLevelRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}