using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.Laboratory.LaboratoryPatientStatistic
{
    public class LaboratoryPatientStatisticRepository : GRepository<Data.DbModels.LaboratorySchema.LaboratoryPatientStatistic>, ILaboratoryPatientStatisticRepository
    {
        private readonly AppDbContext _appDbContext;
        public LaboratoryPatientStatisticRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
