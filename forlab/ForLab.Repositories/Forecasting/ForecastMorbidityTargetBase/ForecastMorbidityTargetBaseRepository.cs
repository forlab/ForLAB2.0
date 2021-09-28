using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.Forecasting.ForecastMorbidityTargetBase
{
    public class ForecastMorbidityTargetBaseRepository : GRepository<Data.DbModels.ForecastingSchema.ForecastMorbidityTargetBase>, IForecastMorbidityTargetBaseRepository
    {
        private readonly AppDbContext _appDbContext;
        public ForecastMorbidityTargetBaseRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
