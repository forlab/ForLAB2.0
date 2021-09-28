using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.Lookup.Laboratory
{
    public class LaboratoryRepository : GRepository<Data.DbModels.LookupSchema.Laboratory>, ILaboratoryRepository
    {
        private readonly AppDbContext _appDbContext;
        public LaboratoryRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

    }
}
