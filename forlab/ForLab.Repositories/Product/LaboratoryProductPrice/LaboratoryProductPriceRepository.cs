using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.Product.LaboratoryProductPrice
{
    public class LaboratoryProductPriceRepository : GRepository<Data.DbModels.ProductSchema.LaboratoryProductPrice>, ILaboratoryProductPriceRepository
    {
        private readonly AppDbContext _appDbContext;
        public LaboratoryProductPriceRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
