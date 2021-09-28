using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.Product.ProductUsage
{
    public class ProductUsageRepository : GRepository<Data.DbModels.ProductSchema.ProductUsage>, IProductUsageRepository
    {
        private readonly AppDbContext _appDbContext;
        public ProductUsageRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
