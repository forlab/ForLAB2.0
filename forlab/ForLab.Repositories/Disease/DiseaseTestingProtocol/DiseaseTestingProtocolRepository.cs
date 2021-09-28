using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.Disease.DiseaseTestingProtocol
{
    public class DiseaseTestingProtocolRepository : GRepository<Data.DbModels.DiseaseSchema.DiseaseTestingProtocol>, IDiseaseTestingProtocolRepository
    {
        private readonly AppDbContext _appDbContext;
        public DiseaseTestingProtocolRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

    }
}
