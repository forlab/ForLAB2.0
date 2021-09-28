using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.Product.CountryProductPrice
{
    public class CountryProductPriceRepository : GRepository<Data.DbModels.ProductSchema.CountryProductPrice>, ICountryProductPriceRepository
    {
        private readonly AppDbContext _appDbContext;
        public CountryProductPriceRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
