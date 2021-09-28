using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.Vendor.Vendor
{
    public class VendorRepository : GRepository<Data.DbModels.VendorSchema.Vendor>, IVendorRepository
    {
        private readonly AppDbContext _appDbContext;
        public VendorRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
