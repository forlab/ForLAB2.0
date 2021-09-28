using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.DiseaseProgram.ProductAssumptionParameter
{
    public class ProductAssumptionParameterRepository : GRepository<Data.DbModels.DiseaseProgramSchema.ProductAssumptionParameter>, IProductAssumptionParameterRepository
    {
        private readonly AppDbContext _appDbContext;
        public ProductAssumptionParameterRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

    }
}
