using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.Forecasting.ForecastLaboratory
{
    public class ForecastLaboratoryRepository : GRepository<Data.DbModels.ForecastingSchema.ForecastLaboratory>, IForecastLaboratoryRepository
    {
        private readonly AppDbContext _appDbContext;
        public ForecastLaboratoryRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
