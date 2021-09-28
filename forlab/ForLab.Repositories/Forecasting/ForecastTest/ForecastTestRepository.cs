using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.Forecasting.ForecastTest
{
    public class ForecastTestRepository : GRepository<Data.DbModels.ForecastingSchema.ForecastTest>, IForecastTestRepository
    {
        private readonly AppDbContext _appDbContext;
        public ForecastTestRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
