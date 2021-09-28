using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.Forecasting.ForecastResult
{
    public class ForecastResultRepository : GRepository<Data.DbModels.ForecastingSchema.ForecastResult>, IForecastResultRepository
    {
        private readonly AppDbContext _appDbContext;
        public ForecastResultRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
