using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.Lookup.ProductBasicUnit
{
    public class ProductBasicUnitRepository : GRepository<Data.DbModels.LookupSchema.ProductBasicUnit>, IProductBasicUnitRepository
    {
        private readonly AppDbContext _appDbContext;
        public ProductBasicUnitRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
