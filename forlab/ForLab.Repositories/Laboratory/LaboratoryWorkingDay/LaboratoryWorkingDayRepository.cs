using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.Laboratory.LaboratoryWorkingDay
{
    public class LaboratoryWorkingDayeRepository : GRepository<Data.DbModels.LaboratorySchema.LaboratoryWorkingDay>, ILaboratoryWorkingDayRepository
    {
        private readonly AppDbContext _appDbContext;
        public LaboratoryWorkingDayeRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
