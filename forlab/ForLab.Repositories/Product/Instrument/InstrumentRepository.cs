using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;
using ForLab.Repositories.Product.Instrument;

namespace ForLab.Repositories.Product.Instrumet
{
   public class InstrumentRepository : GRepository<Data.DbModels.ProductSchema.Instrument>, IInstrumentRepository

    {
        private readonly AppDbContext _appDbContext;
    public InstrumentRepository(AppDbContext appDbContext) : base(appDbContext)
    {
        _appDbContext = appDbContext;
    }

}
}
