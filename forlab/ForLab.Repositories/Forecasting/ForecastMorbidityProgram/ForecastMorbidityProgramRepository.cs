using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.Forecasting.ForecastMorbidityProgram
{
    public class ForecastMorbidityProgramRepository : GRepository<Data.DbModels.ForecastingSchema.ForecastMorbidityProgram>, IForecastMorbidityProgramRepository
    {
        private readonly AppDbContext _appDbContext;
        public ForecastMorbidityProgramRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
