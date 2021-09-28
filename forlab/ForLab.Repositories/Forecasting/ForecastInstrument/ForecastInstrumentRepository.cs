using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.Forecasting.ForecastInstrument
{
    public class ForecastInstrumentRepository : GRepository<Data.DbModels.ForecastingSchema.ForecastInstrument>, IForecastInstrumentRepository
    {
        private readonly AppDbContext _appDbContext;
        public ForecastInstrumentRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
