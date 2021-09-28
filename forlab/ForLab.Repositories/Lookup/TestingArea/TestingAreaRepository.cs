using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.Lookup.TestingArea
{
    public class TestingAreaRepository : GRepository<Data.DbModels.LookupSchema.TestingArea>, ITestingAreaRepository
    {
        private readonly AppDbContext _appDbContext;
        public TestingAreaRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

    }
}
