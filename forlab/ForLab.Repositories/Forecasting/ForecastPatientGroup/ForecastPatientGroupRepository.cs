using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.Forecasting.ForecastPatientGroup
{
    public class ForecastPatientGroupRepository : GRepository<Data.DbModels.ForecastingSchema.ForecastPatientGroup>, IForecastPatientGroupRepository
    {
        private readonly AppDbContext _appDbContext;
        public ForecastPatientGroupRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
