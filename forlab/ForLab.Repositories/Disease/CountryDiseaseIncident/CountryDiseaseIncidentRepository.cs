using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.Disease.CountryDiseaseIncident
{
    public class CountryDiseaseIncidentRepository : GRepository<Data.DbModels.DiseaseSchema.CountryDiseaseIncident>, ICountryDiseaseIncidentRepository
    {
        private readonly AppDbContext _appDbContext;
        public CountryDiseaseIncidentRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

    }
}