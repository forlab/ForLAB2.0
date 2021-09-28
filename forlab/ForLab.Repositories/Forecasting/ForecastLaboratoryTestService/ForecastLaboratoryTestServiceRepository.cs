using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.Forecasting.ForecastLaboratoryTestService
{
    public class ForecastLaboratoryTestServiceRepository : GRepository<Data.DbModels.ForecastingSchema.ForecastLaboratoryTestService>, IForecastLaboratoryTestServiceRepository
    {
        private readonly AppDbContext _appDbContext;
        public ForecastLaboratoryTestServiceRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
