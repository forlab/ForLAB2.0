using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.Laboratory.LaboratoryConsumption
{
    public class LaboratoryConsumptionRepository : GRepository<Data.DbModels.LaboratorySchema.LaboratoryConsumption>, ILaboratoryConsumptionRepository
    {
        private readonly AppDbContext _appDbContext;
        public LaboratoryConsumptionRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
