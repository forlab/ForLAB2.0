using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.Forecasting.ForecastLaboratoryConsumption
{
    public class ForecastLaboratoryConsumptionRepository : GRepository<Data.DbModels.ForecastingSchema.ForecastLaboratoryConsumption>, IForecastLaboratoryConsumptionRepository
    {
        private readonly AppDbContext _appDbContext;
        public ForecastLaboratoryConsumptionRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
