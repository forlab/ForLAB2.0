using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.Vendor.VendorContact
{
    public class VendorContactRepository : GRepository<Data.DbModels.VendorSchema.VendorContact>, IVendorContactRepository
    {
        private readonly AppDbContext _appDbContext;
        public VendorContactRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
