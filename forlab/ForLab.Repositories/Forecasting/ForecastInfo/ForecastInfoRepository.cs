using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.Forecasting.ForecastInfo
{
    public class ForecastInfoRepository : GRepository<Data.DbModels.ForecastingSchema.ForecastInfo>, IForecastInfoRepository
    {
        private readonly AppDbContext _appDbContext;
        public ForecastInfoRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
