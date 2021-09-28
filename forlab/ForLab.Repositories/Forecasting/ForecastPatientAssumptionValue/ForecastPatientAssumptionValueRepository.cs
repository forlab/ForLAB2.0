using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.Forecasting.ForecastPatientAssumptionValue
{
    public class ForecastPatientAssumptionValueRepository : GRepository<Data.DbModels.ForecastingSchema.ForecastPatientAssumptionValue>, IForecastPatientAssumptionValueRepository
    {
        private readonly AppDbContext _appDbContext;
        public ForecastPatientAssumptionValueRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
