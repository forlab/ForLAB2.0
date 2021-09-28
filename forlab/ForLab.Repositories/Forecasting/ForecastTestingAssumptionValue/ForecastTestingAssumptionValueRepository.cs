using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.Forecasting.ForecastTestingAssumptionValue
{
    public class ForecastTestingAssumptionValueRepository : GRepository<Data.DbModels.ForecastingSchema.ForecastTestingAssumptionValue>, IForecastTestingAssumptionValueRepository
    {
        private readonly AppDbContext _appDbContext;
        public ForecastTestingAssumptionValueRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
