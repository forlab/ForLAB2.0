using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.Testing.Test
{
    public class TestRepository : GRepository<Data.DbModels.TestingSchema.Test>, ITestRepository
    {
        private readonly AppDbContext _appDbContext;
        public TestRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

    }
}
