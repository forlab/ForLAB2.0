using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.Forecasting.ForecastProductAssumptionValue
{
    public class ForecastProductAssumptionValueRepository : GRepository<Data.DbModels.ForecastingSchema.ForecastProductAssumptionValue>, IForecastProductAssumptionValueRepository
    {
        private readonly AppDbContext _appDbContext;
        public ForecastProductAssumptionValueRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
