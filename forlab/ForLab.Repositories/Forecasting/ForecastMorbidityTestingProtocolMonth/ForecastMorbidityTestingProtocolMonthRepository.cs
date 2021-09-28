using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.Forecasting.ForecastMorbidityTestingProtocolMonth
{
    public class ForecastMorbidityTestingProtocolMonthRepository : GRepository<Data.DbModels.ForecastingSchema.ForecastMorbidityTestingProtocolMonth>, IForecastMorbidityTestingProtocolMonthRepository
    {
        private readonly AppDbContext _appDbContext;
        public ForecastMorbidityTestingProtocolMonthRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
