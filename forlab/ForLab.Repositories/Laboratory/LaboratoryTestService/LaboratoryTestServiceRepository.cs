using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.Laboratory.LaboratoryTestService
{
    public class LaboratoryTestServiceRepository : GRepository<Data.DbModels.LaboratorySchema.LaboratoryTestService>, ILaboratoryTestServiceRepository
    {
        private readonly AppDbContext _appDbContext;
        public LaboratoryTestServiceRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
