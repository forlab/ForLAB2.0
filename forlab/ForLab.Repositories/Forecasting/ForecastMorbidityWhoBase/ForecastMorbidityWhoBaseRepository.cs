using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.Forecasting.ForecastMorbidityWhoBase
{
    public class ForecastMorbidityWhoBaseRepository : GRepository<Data.DbModels.ForecastingSchema.ForecastMorbidityWhoBase>, IForecastMorbidityWhoBaseRepository
    {
        private readonly AppDbContext _appDbContext;
        public ForecastMorbidityWhoBaseRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
