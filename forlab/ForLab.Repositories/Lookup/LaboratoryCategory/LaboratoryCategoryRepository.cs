using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.Lookup.LaboratoryCategory
{
    public class LaboratoryCategoryRepository : GRepository<Data.DbModels.LookupSchema.LaboratoryCategory>, ILaboratoryCategoryRepository
    {
        private readonly AppDbContext _appDbContext;
        public LaboratoryCategoryRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}