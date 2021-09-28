using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.Laboratory.LaboratoryInstrument
{
    public class LaboratoryInstrumentRepository : GRepository<Data.DbModels.LaboratorySchema.LaboratoryInstrument>, ILaboratoryInstrumentRepository
    {
        private readonly AppDbContext _appDbContext;
        public LaboratoryInstrumentRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
