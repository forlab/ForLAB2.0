using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.Forecasting.ForecastCategory
{
    public class ForecastCategoryRepository : GRepository<Data.DbModels.ForecastingSchema.ForecastCategory>, IForecastCategoryRepository
    {
        private readonly AppDbContext _appDbContext;
        public ForecastCategoryRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
