using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.Disease.Disease
{
    public class DiseaseRepository : GRepository<Data.DbModels.DiseaseSchema.Disease>, IDiseaseRepository
    {
        private readonly AppDbContext _appDbContext;
        public DiseaseRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

    }
}
