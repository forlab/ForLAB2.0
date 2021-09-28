using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.Product.RegionProductPrice
{
    public class RegionProductPriceRepository : GRepository<Data.DbModels.ProductSchema.RegionProductPrice>, IRegionProductPriceRepository
    {
        private readonly AppDbContext _appDbContext;
        public RegionProductPriceRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
